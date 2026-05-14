using MediatR;

namespace CorpComm.Application.Features.Meetings.Commands;

public record SendInvitationCommand(Guid MeetingId, string GuestEmail, string MeetingLink) : IRequest;