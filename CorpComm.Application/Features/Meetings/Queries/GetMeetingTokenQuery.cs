using MediatR;
using Microsoft.IdentityModel.Tokens;
using CorpComm.Application.Common.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CorpComm.Application.Features.Meetings.Queries;

public record GetMeetingTokenQuery(string RoomId, string UserName) : IRequest<string>;

public class GetMeetingTokenQueryHandler : IRequestHandler<GetMeetingTokenQuery, string>
{
    private readonly LiveKitSettings _settings;

    // Конструктор для впровадження залежностей (Dependency Injection)
    public GetMeetingTokenQueryHandler(IOptions<LiveKitSettings> options)
    {
        _settings = options.Value;
    }

    public Task<string> Handle(GetMeetingTokenQuery request, CancellationToken cancellationToken)
    {
        // Тепер беремо ключі з конфігурації, а не хардкодимо їх
        var apiKey = _settings.ApiKey;
        var apiSecret = _settings.ApiSecret;

        var videoGrants = new 
        { 
            roomJoin = true, 
            room = request.RoomId 
        };

        var videoGrantsJson = JsonSerializer.Serialize(videoGrants);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()), 
            new Claim("name", request.UserName),
            new Claim("video", videoGrantsJson, JsonClaimValueTypes.Json)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = apiKey,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}