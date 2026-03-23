using Windows.Security.Credentials;
using DSAapp.Contracts.Services;

namespace DSAapp.Services;

public class CredentialService : ICredentialService
{
    private readonly PasswordVault _vault = new();

    public PasswordCredential? GetCredential(string resource, string userName)
    {
        try
        {
            return _vault.Retrieve(resource, userName);
        }
        catch
        {
            return null;
        }
    }

    public void SaveCredential(string resource, string userName, string password)
    {
        var credential = new PasswordCredential(resource, userName, password);
        _vault.Add(credential);
    }

    public void RemoveCredential(string resource, string userName)
    {
        var credential = GetCredential(resource, userName);
        if (credential != null)
        {
            _vault.Remove(credential);
        }
    }
}
