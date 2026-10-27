using DTO.User;

namespace DataAccess.Interface {

    public interface IUserRepository {
        void AddUser(DTOUser dtoUser);
        List<DTOUser>? GetUsers();
        DTOUser? GetUser(string username, string password);
        bool UpdateUser(DTOUser dtoUser);
    }

}
