using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Mapper;
using DTO.Context;
using DTO.Label;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repository {
    internal class ContextRepository : IContextRepository {
        private readonly Database _db;
        private readonly ILogger<ContextRepository> _logger;
        private readonly IFileServerRepository _filserverRepository;

        public ContextRepository(
            Database db,
            ILogger<ContextRepository> logger,
            IFileServerRepository fileServerRepository
        ) {
            _db = db;
            _logger = logger;
            _filserverRepository = fileServerRepository;
        }

        public async Task<List<DTOContext>> GetAllContextByCustomerIdAsync(string customerId) {
            _logger.LogInformation("Finding context for customer {customerId}", customerId);

            var customer = await _db.Customers.FirstOrDefaultAsync(c =>
                c.CustomerId.ToUpper().Equals(customerId.ToUpper())
            );

            if (customer == null) {
                _logger.LogInformation("Customer: {customerId} not found", customerId);
                return new List<DTOContext>();
            }

            var contexts = await _db
                .Context.Where(cx => cx.CustomerId == customer.Id)
                .Include(cx => cx.ContextType)
                .Include(cx => cx.ContextData)
                    .ThenInclude(cd => cd.ContextDataType)
                .ToListAsync();

            return contexts.ToDtoList(customerId);
        }

        public Task<bool> AddLabelToContextAsync(string contextId, DTOLabel labelDto) {
            throw new NotImplementedException();
        }

        public Task<List<DTOLabel>> GetAllLabelsByContextIdAsync(string contextId) {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveLabelFromContextAsync(string contextId, DTOLabel labelDto) {
            throw new NotImplementedException();
        }
    }
}
