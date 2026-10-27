using BusinessLogic.Interface;
using DataAccess.Interface;
using DTO.Context;

namespace BusinessLogic.Service
{
    public class ContextService : IContextService
    {
        private readonly IContextRepository _contextRepository;

        public ContextService(IContextRepository contextRepository)
        {
            _contextRepository = contextRepository;
        }

        public Task<List<DTOContext>> GetAllContextByCustomerIdAsync(string customerId)
        {
            return _contextRepository.GetAllContextByCustomerIdAsync(customerId);
        }
    }
}
