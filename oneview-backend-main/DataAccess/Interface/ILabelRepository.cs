using DTO.Label;

namespace DataAccess.Interface
{
    public interface ILabelRepository
    {
        Task<bool> CreateNewLabelAsync(DTOLabel labelDto);
        Task<bool> DeleteLabelAsync(DTOLabel labelDto);
        Task<bool> UpdateLabelAsync(DTOLabel labelDto);
        Task<List<DTOLabel>> GetAllLabelsByContextId(int contextId);
        Task<List<DTOLabel>> GetAllLabelsByCustomerId(string customerId);
    }
}