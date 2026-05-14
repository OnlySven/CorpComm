using MediatR;

namespace CorpComm.Application.Features.Meetings.Commands;
public record CreateMeetingCommand(Guid OrganizerId, string Title) : IRequest<Guid>;