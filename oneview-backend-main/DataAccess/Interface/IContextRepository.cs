using DTO.Context;
using DTO.Label;

namespace DataAccess.Interface
{
    public interface IContextRepository
    {
        Task<List<DTOContext>> GetAllContextByCustomerIdAsync(string CustomerId);
        Task<List<DTOLabel>> GetAllLabelsByContextIdAsync(string contextId);
        Task<bool> AddLabelToContextAsync(string contextId, DTOLabel labelDto);
        Task<bool> RemoveLabelFromContextAsync(string contextId, DTOLabel labelDto);
    }
}