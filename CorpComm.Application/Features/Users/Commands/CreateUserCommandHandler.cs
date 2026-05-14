using CorpComm.Domain.Entities;
using CorpComm.Domain.Repositories;
using MediatR;

namespace CorpComm.Application.Features.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Перевірка чи існує такий email
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Користувач з таким email вже існує.");
        }

        // 2. Створення Domain-сутності (з використанням нашого конструктора)
        var user = new User(request.Email, request.FullName);

        // 3. Збереження в базу даних
        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}