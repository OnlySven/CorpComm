using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CorpComm.Application.Features.Meetings.Queries;

public record GetMeetingTokenQuery(string RoomId, string UserName) : IRequest<string>;

public class GetMeetingTokenQueryHandler : IRequestHandler<GetMeetingTokenQuery, string>
{
    public Task<string> Handle(GetMeetingTokenQuery request, CancellationToken cancellationToken)
    {
        // Ключі для LiveKit (--dev mode)
        var apiKey = "devkey";
        var apiSecret = "secret_key_for_development_128_bits";

        // Формуємо об'єкт з правами доступу (Video Grants), які вимагає LiveKit
        var videoGrants = new 
        { 
            roomJoin = true, 
            room = request.RoomId 
        };

        // Серіалізуємо права в JSON
        var videoGrantsJson = JsonSerializer.Serialize(videoGrants);

        // Налаштовуємо ключ шифрування
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Створюємо Payload (тіло токена)
        var claims = new[]
        {
            // sub (Subject) - Унікальний ідентифікатор учасника (Identity)
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()), 
            
            // name - Відображуване ім'я
            new Claim("name", request.UserName),
            
            // video - Спеціальний claim для LiveKit (вказуємо, що це JSON-об'єкт)
            new Claim("video", videoGrantsJson, JsonClaimValueTypes.Json)
        };

        // Формуємо сам токен
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = apiKey, // iss (Issuer) - повинен бути API Key
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), // Токен дійсний 2 години
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        // Повертаємо готовий рядок
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}