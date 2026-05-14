using CorpComm.Domain.Common;

namespace CorpComm.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Порожній конструктор необхідний для Entity Framework Core при рефлексії
    private User() { } 

    public User(string email, string fullName)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}