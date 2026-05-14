using CorpComm.Application.Common.Services;
using CorpComm.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CorpComm.Infrastructure.Services;

public class SmtpEmailInvitationSender : InvitationSender
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpEmailInvitationSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    protected override async Task DeliverAsync(string email, string subject, string content)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;

        // Форматуємо лист як текст (можна замінити на TextFormat.Html для красивих листів)
        message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = content
        };

        using var client = new SmtpClient();
        try
        {
            // Підключаємося до сервера
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
            
            // Авторизуємося
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            
            // Відправляємо
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}