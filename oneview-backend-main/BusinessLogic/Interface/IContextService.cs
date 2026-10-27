using DTO.Context;

namespace BusinessLogic.Interface
{
    public interface IContextService
    {
        Task<List<DTOContext>> GetAllContextByCustomerIdAsync(string CustomerId);
    }
}
