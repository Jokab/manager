using System.ComponentModel.DataAnnotations;

namespace ManagerGame.Domain;

public record Email
{
    public Email(string emailAddress)
    {
        ArgumentException.ThrowIfNullOrEmpty(emailAddress);
        if (!new EmailAddressAttribute().IsValid(emailAddress))
            throw new ArgumentException("Invalid email " + emailAddress, nameof(emailAddress));
        EmailAddress = emailAddress;
    }

    public string EmailAddress { get; }

    public virtual bool Equals(Email? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(EmailAddress, other.EmailAddress, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => EmailAddress.GetHashCode();
}
