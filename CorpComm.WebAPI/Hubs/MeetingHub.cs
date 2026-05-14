using Microsoft.AspNetCore.SignalR;

namespace CorpComm.WebAPI.Hubs;

public class MeetingHub : Hub
{
    // Метод, який клієнт викликатиме для підключення до кімнати зустрічі
    public async Task JoinMeeting(string meetingId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
        
        // Повідомляємо інших учасників кімнати, що хтось приєднався
        await Clients.Group(meetingId).SendAsync("UserJoined", userName, Context.ConnectionId);
    }

    // Метод для обміну WebRTC кандидатами (Signaling)
    public async Task SendSignal(string signal, string targetConnectionId)
    {
        await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", signal, Context.ConnectionId);
    }
}