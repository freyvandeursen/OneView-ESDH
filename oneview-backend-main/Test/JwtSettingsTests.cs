using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BusinessLogic.Helper;
using BusinessLogic.Service;
using DataAccess.Interface;
using DTO.Authentication;
using DTO.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Test;

public class JwtSettingsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                                ")]
    [InlineData("too-short")]
    public void MissingOrShortKeyIsRejected(string? key)
    {
        var configuration = Configure(key);

        Assert.Throws<InvalidOperationException>(() => JwtSettings.FromConfiguration(configuration));
    }

    [Fact]
    public void KeyLengthUsesUtf8BytesAndDefaultIssuerAndAudience()
    {
        var key = new string('\u00e6', 16);

        var settings = JwtSettings.FromConfiguration(Configure(key));

        Assert.Equal(32, Encoding.UTF8.GetByteCount(settings.Key));
        Assert.Equal("OneViewAPI", settings.Issuer);
        Assert.Equal("OneViewClient", settings.Audience);
    }

    [Fact]
    public void LoginTokenValidatesWithConfiguredKeyIssuerAndAudience()
    {
        var configuration = Configure("oneview-test-signing-key-at-least-32-bytes", "CustomIssuer", "CustomAudience");
        var settings = JwtSettings.FromConfiguration(configuration);
        var service = new LoginService(new UserRepositoryStub(), configuration);

        var response = service.Authenticate(new DTOLoginRequest { Username = "test", Password = "test" });
        var principal = new JwtSecurityTokenHandler().ValidateToken(response.Token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
        }, out _);

        Assert.Equal("test", principal.Identity?.Name);
        Assert.True(principal.IsInRole("ADMIN"));
    }

    private static IConfiguration Configure(string? key, string? issuer = null, string? audience = null)
        => new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = key,
            ["Jwt:Issuer"] = issuer,
            ["Jwt:Audience"] = audience,
        }).Build();

    private sealed class UserRepositoryStub : IUserRepository
    {
        public DTOUser? GetUser(string username, string password)
            => new DTOUser { UserName = username, Password = password, Role = "ADMIN" };

        public void AddUser(DTOUser dtoUser) => throw new NotSupportedException();
        public List<DTOUser>? GetUsers() => throw new NotSupportedException();
        public bool UpdateUser(DTOUser dtoUser) => throw new NotSupportedException();
    }
}
