using BusinessLogic.Interface;
using DataAccess.Interface;
using DTO.Label;

namespace BusinessLogic.Service
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public Task<List<DTOLabel>> GetAllLabelsByContextIdAsync(int contextId)
        {
            return _labelRepository.GetAllLabelsByContextId(contextId);
        }

        public Task<List<DTOLabel>> GetAllLabelsByCustomerIdAsync(string customerId)
        {
            return _labelRepository.GetAllLabelsByCustomerId(customerId);
        }

        public Task<bool> CreateNewLabelAsync(DTOLabel labelDTO)
        {
            return _labelRepository.CreateNewLabelAsync(labelDTO);
        }

        public Task<bool> DeleteLabelAsync(DTOLabel labelDTO)
        {
            return _labelRepository.DeleteLabelAsync(labelDTO);
        }

        public async Task<bool> UpdateLabelAsync(DTOLabel labelDTO)
        {
            return await _labelRepository.UpdateLabelAsync(labelDTO);
        }

    }
}
