using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System.Threading.Tasks;

namespace ListaTelefonicaIACOApp.Services.Ldap;

/// <summary>
/// Serviço de autenticação LDAP (compatível com ApacheDS e Active Directory).
/// </summary>
public class LdapService
{
    private readonly ILogger<LdapService> _logger;
    private readonly LdapSettings _ldapSettings;

    public LdapService(ILogger<LdapService> logger, IOptions<LdapSettings> ldapSettings)
    {
        _logger = logger;
        _ldapSettings = ldapSettings.Value;
    }

    public async Task<bool> AuthenticateAgainstLdap(string username, string password)
    {
        try
        {
            using var connection = new LdapConnection();
            await connection.ConnectAsync(_ldapSettings.LdapServer, _ldapSettings.LdapPort);

            // First, bind with admin credentials to search for the user
            await connection.BindAsync(_ldapSettings.Username, _ldapSettings.Password);

            // Search for the user's DN
            string userDn = await GetUserDnAsync(connection, username);

            if (string.IsNullOrEmpty(userDn))
            {
                _logger.LogWarning("Usuário {Username} não encontrado no LDAP.", username);
                return false;
            }

            // Bind with user credentials
            await connection.BindAsync(userDn, password);

            _logger.LogInformation("Usuário {Username} autenticado com sucesso.", username);
            return true;
        }
        catch (LdapException ex)
        {
            _logger.LogWarning("Falha na autenticação LDAP para usuário {Username}: {Message}", username, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado na autenticação LDAP para usuário {Username}", username);
            return false;
        }
    }

    private async Task<string> GetUserDnAsync(LdapConnection connection, string username)
    {
        /*
        string filter = $"(&(objectClass=person)(cn={username}))";
        await using var search = await connection.SearchAsync(
            _ldapSettings.UserSearchBase,
            LdapConnection.ScopeSub,
            filter,
            null, // attributes to return (null for all)
            false // typesOnly
        );

        await foreach (var result in search)
        {
            if (result != null)
            {
                return result.Dn;
            }
        }

        _logger?.LogWarning("Usuário {Username} não encontrado no LDAP.", username);
        return null;
        */
    }


}