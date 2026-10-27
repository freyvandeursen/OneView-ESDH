using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Mapper;
using DataAccess.Model;
using DTO.User;
using System.Data.Common;

namespace DataAccess.Repository {
    internal class UserRepository : IUserRepository {

        private readonly Database _db;

        public UserRepository(Database db) {
            _db = db;
        }

        public void AddUser(DTOUser dtoUser) {
            try {
                var user = dtoUser.NewToModel();
                _db.Add(user);
                _db.SaveChanges();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw new Exception("Failed to add user", ex);
            }
        }
        public List<DTOUser>? GetUsers() {
            try {
                var users = (
                    from u in _db.Users
                    select u).ToList();

                return users == null || users.Count == 0 ? null : UserMapper.ToDTO(users);
            }
            catch (DbException ex) {
                Console.WriteLine($"Database error retrieving user: {ex.Message}");
                throw new Exception("Failed to retrieve user due to a database error.", ex);
            }

        }


        public DTOUser? GetUser(string username, string password) {
            try {
                var user = (from u in _db.Users
                            where u.Username == username && u.Password == password
                            select u).FirstOrDefault();

                return user == null ? null : UserMapper.ToDTO(user);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error retrieving user by username and password: {ex.Message}");
                throw new Exception("Failed to retrieve user, because username or password was incorrect", ex);
            }
        }

        public bool UpdateUser(DTOUser dtoUser) {
            try {
                var user = (from u in _db.Set<User>()
                            where u.Username == dtoUser.UserName && u.Password == dtoUser.Password
                            select u).FirstOrDefault();

                if (user == null) {
                    return false;
                }

                user = UserMapper.DTOToModel(dtoUser, user);
                _db.Update(user);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw new Exception("Failed to update user", ex);
            }
        }
    }
}
