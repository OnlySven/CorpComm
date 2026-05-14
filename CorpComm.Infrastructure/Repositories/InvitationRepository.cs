using CorpComm.Domain.Entities;
   using CorpComm.Domain.Repositories;
   using CorpComm.Infrastructure.Data;

   namespace CorpComm.Infrastructure.Repositories;

   public class InvitationRepository : IInvitationRepository
   {
       private readonly ApplicationDbContext _context;
       public InvitationRepository(ApplicationDbContext context) => _context = context;

       public async Task AddAsync(Invitation invitation, CancellationToken ct)
       {
           await _context.Invitations.AddAsync(invitation, ct);
           await _context.SaveChangesAsync(ct);
       }
   }