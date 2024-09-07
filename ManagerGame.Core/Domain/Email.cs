using System.ComponentModel.DataAnnotations;

namespace ManagerGame.Core.Domain;

public record Email
{
    public Email(string emailAddress)
    {
        ArgumentException.ThrowIfNullOrEmpty(emailAddress);
        if (!new EmailAddressAttribute().IsValid(emailAddress))
            throw new ArgumentException("Invalid email " + emailAddress, nameof(emailAddress));
        EmailAddress = emailAddress;
    }

    public string EmailAddress { get; private set; }
}