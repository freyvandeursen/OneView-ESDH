using DTO.Context;

namespace DataAccess.Mapper {

    internal static class ContextTypeMapper {
        internal static DTOContextType ToDto(this Model.ContextType contextType) {

            return contextType == null
                ? throw new ArgumentNullException(nameof(contextType))
                : new DTOContextType {
                    TypeName = contextType.TypeName,
                    Description = contextType.Description
                };
        }
    }
}