using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Mapper;
using DTO.Label;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repository {
    internal class LabelRepository : ILabelRepository {

        private readonly Database _db;
        private readonly ILogger<LabelRepository> _logger;

        public LabelRepository(Database db, ILogger<LabelRepository> logger) {
            _db = db;
            _logger = logger;
        }
        public async Task<bool> CreateNewLabelAsync(DTOLabel dtoLabel) {
            var rows = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO vw_Label (CustomerId, ContextId, Name, Description, [Value])
                VALUES (
                    {dtoLabel.CustomerId},
                    {dtoLabel.ContextId},
                    {dtoLabel.Name},
                    {dtoLabel.Description},
                    {dtoLabel.Value})");
            return rows == 1;
        }
        public async Task<bool> DeleteLabelAsync(DTOLabel dtoLabel) {
            if (dtoLabel.Id == null) 
            { 
                throw new ArgumentException("Label ID cannot be null for deletion.");
            }
            var rows = await _db.Database.ExecuteSqlRawAsync(
                    @"DELETE FROM vw_Label
                    WHERE Id = {0}",
                dtoLabel.Id
            );
            return rows == 1;
        }
        public async Task<bool> UpdateLabelAsync(DTOLabel dtoLabel) {
            if (dtoLabel.Id == null) 
            { 
                throw new ArgumentException("Label ID cannot be null for update.");
            }
            var rows = await _db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE vw_Label
            SET CustomerId = {dtoLabel.CustomerId},
                ContextId = {dtoLabel.ContextId},
                Name = {dtoLabel.Name},
                Description = {dtoLabel.Description},
                Value = {dtoLabel.Value}
            WHERE Id = {dtoLabel.Id}");
            return rows == 1;
        }
        public async Task<List<DTOLabel>> GetAllLabelsByContextId(int contextId) {
            var labels = await (from l in _db.Labels
                                where l.ContextId == contextId
                                select l).ToListAsync();
            return labels.ToDtoList();
        }
        public async Task<List<DTOLabel>> GetAllLabelsByCustomerId(string customerId) {
            var labels = await (from l in _db.Labels
                    where l.CustomerId == customerId
                    select l).ToListAsync();
            return labels.ToDtoList();
        }
    }
}
