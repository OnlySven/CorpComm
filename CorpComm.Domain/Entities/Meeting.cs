using CorpComm.Domain.Common;

namespace CorpComm.Domain.Entities;

public class Meeting : Entity
{
    public string Title { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public Guid OrganizerId { get; private set; }
    public MeetingStatus Status { get; private set; }

    private Meeting() { } 

    public Meeting(string title, DateTime scheduledStartTime, Guid organizerId)
    {
        Title = title;
        ScheduledStartTime = scheduledStartTime;
        OrganizerId = organizerId;
        Status = MeetingStatus.Scheduled;
    }

    public void Start()
    {
        if (Status != MeetingStatus.Scheduled)
            throw new InvalidOperationException("Можна розпочати лише заплановану зустріч.");
        
        Status = MeetingStatus.InProgress;
    }

    public void End()
    {
        if (Status != MeetingStatus.InProgress)
            throw new InvalidOperationException("Не можна завершити зустріч, яка не триває.");
            
        Status = MeetingStatus.Completed;
    }
}

public enum MeetingStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}