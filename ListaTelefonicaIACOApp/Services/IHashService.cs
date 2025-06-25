using ListaTelefonicaIACOApp.Models;

namespace ListaTelefonicaIACOApp.Services
{
    public interface IHashService
    {
        string HashPassword(Usuario usuario, string senha);
        bool VerifyPassword(Usuario usuario, string senhaHash, string senhaDigitada);
    }
}
