using CorpComm.Domain.Entities;
using CorpComm.Domain.Repositories;
using CorpComm.Application.Common.Services;
using MediatR;

namespace CorpComm.Application.Features.Meetings.Commands;

public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IMeetingRepository _meetingRepository;
    private readonly InvitationSender _emailSender;

    public SendInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IMeetingRepository meetingRepository,
        InvitationSender emailSender)
    {
        _invitationRepository = invitationRepository;
        _meetingRepository = meetingRepository;
        _emailSender = emailSender;
    }

    public async Task Handle(SendInvitationCommand request, CancellationToken ct)
    {
        // 1. Отримуємо зустріч із бази (нам потрібен заголовок для листа)
        // Тобі потрібно додати IMeetingRepository у конструктор хендлера
        var meeting = await _meetingRepository.GetByIdAsync(request.MeetingId);
        
        if (meeting == null)
            throw new Exception("Зустріч не знайдено");

        // 2. Створюємо запис про запрошення для історії
        var invitation = new Invitation(request.MeetingId, request.GuestEmail);
        await _invitationRepository.AddAsync(invitation, ct);

        // 3. Викликаємо метод ПРАВИЛЬНО (однина + передаємо об'єкт meeting)
        await _emailSender.SendInvitationAsync(
            meeting, 
            request.GuestEmail, 
            request.MeetingLink
        );
    }
}