using ListaTelefonicaIACOApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ListaTelefonicaIACOApp.Services
{
    public class HashService : IHashService
    {

        private readonly PasswordHasher<Usuario> _hasher = new();

        public string HashPassword(Usuario usuario, string senha)
        {
            return _hasher.HashPassword(usuario, senha);
        }

        public bool VerifyPassword(Usuario usuario, string senhaHash, string senhaDigitada)
        {
            var result = _hasher.VerifyHashedPassword(usuario, senhaHash, senhaDigitada);
            return result == PasswordVerificationResult.Success;
        }
    }
}
