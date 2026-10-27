using DTO.Label;

namespace BusinessLogic.Interface
{
    public interface ILabelService
    {
        Task<bool> CreateNewLabelAsync(DTOLabel labelDto);
        Task<bool> DeleteLabelAsync(DTOLabel labelDto);
        Task<bool> UpdateLabelAsync(DTOLabel labelDto);
        Task<List<DTOLabel>> GetAllLabelsByContextIdAsync(int contextId);
        Task<List<DTOLabel>> GetAllLabelsByCustomerIdAsync(string customerId);
    }
}