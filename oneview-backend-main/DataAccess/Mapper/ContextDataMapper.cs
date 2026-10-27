using DataAccess.Model;
using DTO.Context;

internal static class ContextDataMapper {
    internal static DTOContextData ToDto(this ContextData data) {

        return data == null
            ? throw new ArgumentNullException(nameof(data))
            : new DTOContextData {
                KeyName = data.KeyName,
                Value = data.Value,
                ContextDataType = data.ContextDataType.ToDto()!
            };
    }

    internal static List<DTOContextData> ToDtoList(this IEnumerable<ContextData> dataList) {
        return dataList?.Select(d => d.ToDto()).ToList() ?? new List<DTOContextData>();
    }
}
