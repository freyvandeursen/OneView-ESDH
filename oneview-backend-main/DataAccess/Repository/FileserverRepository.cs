using Amazon.S3;
using Amazon.S3.Model;
using DataAccess.Context;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DataAccess.Repository
{
    internal class FileServerRepository : IFileServerRepository
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucket = string.Empty;
        private readonly Database _db;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<FileServerRepository> _logger;

        public FileServerRepository(
            IAmazonS3 client,
            IConfiguration configuration,
            Database db,
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<FileServerRepository> logger
        )
        {
            _client = client;
            _bucket = configuration["AWS:BucketName"]!;
            _db = db;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<string> GeneratePreSignedUrlAsync(string fileKey)
        {
            _logger.LogInformation("Generating pre-signed URL for file {fileKey}", fileKey);

            var redis = _connectionMultiplexer.GetDatabase();
            var cacheKey = $"presigned:{fileKey}";

            var cacheUrl = await redis.StringGetAsync(cacheKey);
            if (!cacheUrl.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache hit for file {fileKey}", fileKey);
                return cacheUrl.ToString();
            }

            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucket,
                    Key = fileKey,
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    Verb = HttpVerb.GET,
                };

                var url = await _client.GetPreSignedURLAsync(request);

                await redis.StringSetAsync(cacheKey, url, TimeSpan.FromMinutes(14));
                _logger.LogInformation("Cached pre-signed URL for file {fileKey}", fileKey);

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to generate pre-signed URL for file {fileKey}",
                    fileKey
                );
                throw;
            }
        }

        public async Task SyncAllContextsAsync()
        {
            var contexts = await _db.Context.Include(c => c.ContextData).ToListAsync();

            var data = await _client.ListObjectsV2Async(
                new ListObjectsV2Request
                {
                    BucketName = _bucket,
                    Prefix = "Vitani Security/Kunder/",
                }
            );

            var contextType = await _db.ContextType.FirstOrDefaultAsync(ct =>
                ct.TypeName == "File"
            );

            if (contextType == null)
            {
                throw new Exception("Could not find ContextType 'File'");
            }

            var contextDataTypes = await _db.ContextDataType.ToDictionaryAsync(cdt => cdt.Name);

            if (
                !contextDataTypes.ContainsKey("FileExtension")
                || !contextDataTypes.ContainsKey("FileSize")
                || !contextDataTypes.ContainsKey("LastModified")
            )
            {
                throw new Exception("Missing expected ContextDataType(s)");
            }

            foreach (var file in data.S3Objects)
            {
                var customerId = ParseCustomerIdFromKey(file.Key);
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogInformation("Skipping file with invalid key: {key}", file.Key);
                    continue;
                }

                var customer = await _db.Customers.FirstOrDefaultAsync(c =>
                    c.CustomerId.Equals(customerId)
                );

                if (customer == null)
                {
                    _logger.LogInformation("Customer doesnt exists: {customerId}", customerId);
                    continue;
                }

                var context = contexts.FirstOrDefault(c => c.ReferenceLink == file.Key);

                if (context == null)
                {
                    _logger.LogInformation("Creating new context");
                    context = new Model.Context
                    {
                        Name = Path.GetFileName(file.Key),
                        CustomerId = customer.Id,
                        ReferenceLink = file.Key,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        ContextType = contextType,
                        ContextTypeId = contextType.Id,
                        ContextData = new List<Model.ContextData>(),
                    };
                    _db.Context.Add(context);
                }
                else if (context.LastUpdated >= file.LastModified)
                {
                    _logger.LogInformation("Context already up to date, skipping...");
                    continue;
                }

                _logger.LogInformation("Updating context metadata...");
                await UpdateContextMetadataAsync(context, file, contextDataTypes);
            }

            await _db.SaveChangesAsync();
        }

        private string ParseCustomerIdFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            var parts = key.Split('/');
            if (parts.Length < 3)
            {
                _logger.LogWarning("Invalid S3 key format, cannot parse customerId: {key}", key);
                return string.Empty;
            }

            return parts[2];
        }

        private async Task UpdateContextMetadataAsync(
            Model.Context context,
            S3Object file,
            Dictionary<string, Model.ContextDataType> contextDataTypes
        )
        {
            context.LastUpdated = DateTime.UtcNow;
            context.Name = Path.GetFileName(file.Key);

            UpsertContextData(
                context,
                "FileExtension",
                Path.GetExtension(file.Key),
                contextDataTypes["FileExtension"]
            );
            UpsertContextData(
                context,
                "FileSize",
                file.Size.ToString()!,
                contextDataTypes["FileSize"]
            );
            UpsertContextData(
                context,
                "LastModified",
                file.LastModified.ToString()!,
                contextDataTypes["LastModified"]
            );

            await Task.CompletedTask;
        }

        private void UpsertContextData(
            Model.Context context,
            string key,
            string value,
            Model.ContextDataType contextDataType
        )
        {
            var existing = context.ContextData.FirstOrDefault(cd => cd.KeyName == key);
            if (existing != null)
            {
                existing.Value = value;
                existing.ContextDataType = contextDataType;
                existing.ContextDataTypeId = contextDataType.Id;
            }
            else
            {
                context.ContextData.Add(
                    new Model.ContextData
                    {
                        KeyName = key,
                        Value = value,
                        Context = context,
                        ContextId = context.Id,
                        ContextDataType = contextDataType,
                        ContextDataTypeId = contextDataType.Id,
                    }
                );
            }
        }
    }
}
