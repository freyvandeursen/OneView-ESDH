using DTO.Context;

namespace DataAccess.Mapper
{
    internal static class ContextMapper
    {
        internal static DTOContext ToDto(this Model.Context context, string customerId)
        {
            return context == null
                ? throw new ArgumentException(nameof(context))
                : new DTOContext
                {
                    Name = context.Name,
                    ReferenceLink = context.ReferenceLink,
                    CreatedAt = context.CreatedAt,
                    LastUpdated = context.LastUpdated,
                    ContextType = context.ContextType.ToDto(),
                    ContextData = context.ContextData.ToDtoList(),
                    CustomerId = customerId,
                    ContextId = context.Id,
                };
        }

        internal static List<DTOContext> ToDtoList(
            this IEnumerable<Model.Context> contexts,
            string customerId
        )
        {
            return contexts.Select(c => c.ToDto(customerId)).ToList();
        }
    }
}
