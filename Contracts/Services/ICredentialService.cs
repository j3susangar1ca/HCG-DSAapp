using Windows.Security.Credentials;

namespace DSAapp.Contracts.Services;

public interface ICredentialService
{
    PasswordCredential? GetCredential(string resource, string userName);
    void SaveCredential(string resource, string userName, string password);
    void RemoveCredential(string resource, string userName);
}
