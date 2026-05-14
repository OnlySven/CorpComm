using CorpComm.Domain.Entities;
using CorpComm.Domain.Repositories;
using MediatR;

namespace CorpComm.Application.Features.Meetings.Commands;

public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, Guid>
{
    private readonly IMeetingRepository _repository;

    public CreateMeetingCommandHandler(IMeetingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        // Створюємо нову сутність зустрічі
        var meeting = new Meeting(request.Title, DateTime.UtcNow, request.OrganizerId);

        // Зберігаємо через репозиторій
        await _repository.AddAsync(meeting, cancellationToken);

        return meeting.Id;
    }
}