namespace CorpComm.Domain.Entities;

public class Invitation
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public string GuestEmail { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    
    public Meeting? Meeting { get; set; }

    public Invitation() { } // Для EF Core

    public Invitation(Guid meetingId, string guestEmail)
    {
        Id = Guid.NewGuid();
        MeetingId = meetingId;
        GuestEmail = guestEmail;
        SentAt = DateTime.UtcNow;
    }
}