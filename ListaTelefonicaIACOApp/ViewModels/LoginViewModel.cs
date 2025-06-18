using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O nome é Obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A senha é Obrigatório.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
