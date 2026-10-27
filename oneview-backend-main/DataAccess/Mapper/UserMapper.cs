using DTO.User;

namespace DataAccess.Mapper {
    internal static class UserMapper {
        internal static DTOUser ToDTO(this Model.User user) {
            return new DTOUser {
                UserName = user.Username,
                Password = user.Password,
                Role = user.Role

            };
        }

        public static List<DTOUser> ToDTO(this List<Model.User> users) {
            var dtoUsers = new List<DTOUser>();
            foreach (var user in users) {
                dtoUsers.Add(user.ToDTO());
            }
            return dtoUsers;
        }

        internal static Model.User NewToModel(this DTOUser dtoUser) {
            return new Model.User {
                Id = 0,
                Username = dtoUser.UserName,
                Password = dtoUser.Password,
                Role = dtoUser.Role

            };
        }

        internal static Model.User DTOToModel(this DTOUser dtoUser, Model.User user) {
            user.Username = dtoUser.UserName;
            user.Password = dtoUser.Password;
            user.Role = dtoUser.Role;

            return user;
        }
    }

}