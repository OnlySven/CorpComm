using CorpComm.Domain.Entities;

namespace CorpComm.Domain.Repositories;

public interface IMeetingRepository
{
    Task AddAsync(Meeting meeting, CancellationToken cancellationToken);
    Task<Meeting?> GetByIdAsync(Guid id);
}