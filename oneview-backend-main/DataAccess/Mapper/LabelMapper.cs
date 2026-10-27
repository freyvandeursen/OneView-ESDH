using DTO.Label;
using DataAccess.Model;

namespace DataAccess.Mapper {
    internal static class LabelMapper {

        internal static DTOLabel ToDto(this Label label) {
            return label == null
                ? throw new ArgumentException(nameof(label))
                : new DTOLabel {
                    Id = label.Id,
                    CustomerId = label.CustomerId!,
                    ContextId = label.ContextId,
                    Name = label.Name!,
                    Description = label.Description,
                    Value = label.Value
                };
        }

        internal static Label DtoToModel(this DTOLabel dtoLabel) {
            return dtoLabel == null
                ? throw new ArgumentException(nameof(dtoLabel))
                : new Label {
                    Id = dtoLabel.Id ?? 0,
                    CustomerId = dtoLabel.CustomerId,
                    ContextId = dtoLabel.ContextId,
                    Name = dtoLabel.Name,
                    Description = dtoLabel.Description,
                    Value = dtoLabel.Value
                };
        }

        internal static Label DtoToNewModel(this DTOLabel dtoLabel) {
            return dtoLabel == null
                ? throw new ArgumentException(nameof(dtoLabel))
                : new Label {
                    CustomerId = dtoLabel.CustomerId,
                    ContextId = dtoLabel.ContextId,
                    Name = dtoLabel.Name,
                    Description = dtoLabel.Description,
                    Value = dtoLabel.Value
                };
        }

        internal static List<DTOLabel> ToDtoList(this IEnumerable<Label> labels) {
            return labels.Select(label => label.ToDto()).ToList();
        }
    }
}
