using CorpComm.Domain.Entities;
using CorpComm.Domain.Repositories;
using CorpComm.Infrastructure.Data;

namespace CorpComm.Infrastructure.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly ApplicationDbContext _context;
    public async Task<Meeting?> GetByIdAsync(Guid id) 
    => await _context.Meetings.FindAsync(id);

    public MeetingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Meeting meeting, CancellationToken cancellationToken)
    {
        await _context.Meetings.AddAsync(meeting, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}