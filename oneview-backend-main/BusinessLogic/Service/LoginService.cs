using BusinessLogic.Interface;
using BusinessLogic.Helper;
using DataAccess.Interface;
using DTO.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogic.Service {
    public class LoginService : ILoginService {

        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public LoginService(IUserRepository userRepository, IConfiguration configuration) {
            _userRepository = userRepository;
            _jwtSettings = JwtSettings.FromConfiguration(configuration);
        }
        public DTOLoginResponse Authenticate(DTOLoginRequest request) {

            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password)) {
                return null!;
            }

            var user = _userRepository.GetUser(request.Username, request.Password);
            if (user == null) {
                return null!;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)

                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new DTOLoginResponse { Token = tokenHandler.WriteToken(token) };
        }
    }
}
