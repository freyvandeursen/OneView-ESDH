using DataAccess.Model;
using DTO.Context;

internal static class ContextDataTypeMapper {
    internal static DTOContextDataType ToDto(this ContextDataType dataType) {

        return dataType == null
            ? throw new ArgumentNullException(nameof(dataType))
            : new DTOContextDataType {
                Name = dataType.Name,
                Description = dataType.Description
            };
    }
}