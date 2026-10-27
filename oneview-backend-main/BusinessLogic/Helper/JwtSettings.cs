using System.Text;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Helper;

public sealed record JwtSettings(string Key, string Issuer, string Audience)
{
    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
        {
            throw new InvalidOperationException(
                "Set Jwt:Key to a secret containing at least 32 UTF-8 bytes using user secrets or the Jwt__Key environment variable."
            );
        }

        return new JwtSettings(
            key,
            configuration["Jwt:Issuer"] ?? "OneViewAPI",
            configuration["Jwt:Audience"] ?? "OneViewClient"
        );
    }
}
