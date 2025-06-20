namespace ListaTelefonicaIACOApp
{
    public class LdapSettings
    {
        public string LdapServer { get; set; } = "localhost";
        public int LdapPort { get; set; } = 10389;
        public string LdapBaseDn { get; set; } = "ou=system"; // raiz do seu LDAP

        public string UserSearchBase { get; set; } = "ou=users,ou=system"; // onde estão os usuários

        public string Username { get; set; } = "uid=admin,ou=system"; // bind administrativo
        public string Password { get; set; } = "secret";

        /// <summary>
        /// Formato do DN do usuário para bind, por exemplo: "cn={username},ou=users,ou=system"
        /// </summary>
        public string? UserDnFormat { get; set; } = "cn={username},ou=users,ou=system";
    }
}
