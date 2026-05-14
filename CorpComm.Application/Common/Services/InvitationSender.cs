using CorpComm.Domain.Entities;

namespace CorpComm.Application.Common.Services;

public abstract class InvitationSender
{
    // Це і є наш Template Method. Він визначає загальний алгоритм (скелет).
    // Зверни увагу: метод не віртуальний, щоб спадкоємці не могли зламати саму послідовність кроків.
    public async Task SendInvitationAsync(Meeting meeting, string guestEmail, string meetingLink)
    {
        var subject = GenerateSubject(meeting);
        var content = GenerateContent(meeting, meetingLink);
        
        await DeliverAsync(guestEmail, subject, content);
        
        LogDelivery(guestEmail, meeting.Id);
    }

    // Крок 1: Генерація теми (можна перевизначити за потреби)
    protected virtual string GenerateSubject(Meeting meeting)
    {
        return $"Запрошення на зустріч: {meeting.Title}";
    }

    // Крок 2: Генерація тіла повідомлення
    protected virtual string GenerateContent(Meeting meeting, string meetingLink)
    {
        return $"Привіт!\nВас запрошено на корпоративну зустріч '{meeting.Title}'.\n" +
               $"Щоб приєднатися, перейдіть за посиланням:\n{meetingLink}\n\n" +
               $"Чекаємо на вас!";
    }

    // Крок 3: Абстрактний метод доставки. Його ОБОВ'ЯЗКОВО мають реалізувати спадкоємці (Email, SMS, тощо)
    protected abstract Task DeliverAsync(string email, string subject, string content);

    // Крок 4: Загальна логіка логування
    private void LogDelivery(string email, Guid meetingId)
    {
        // В реальному проекті тут буде запис у логер або в БД (наприклад, сутність Invitation)
        Console.WriteLine($"[AUDIT] Запрошення на зустріч {meetingId} успішно згенеровано для {email}.");
    }
}