using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        public string Role { get; set; }
    }
}
