namespace DapperStudy.Configuration;

public class JwtConfiguration
{
    public const string SectionName = "Jwt";
    public required bool ValidateIssuer { get; set; }
    public required bool ValidateAudience { get; set; }
    public required bool ValidateLifetime { get; set; }
    public required bool ValidateIssuerSigningKey { get; set; }
    public required int ExpireHours { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SigningKey { get; set; }
}