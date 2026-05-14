using CorpComm.Domain.Entities;

namespace CorpComm.Domain.Repositories;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation, CancellationToken ct);
}