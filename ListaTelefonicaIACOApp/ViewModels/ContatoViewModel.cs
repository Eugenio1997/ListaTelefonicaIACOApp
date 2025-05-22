using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class ContatoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Fixo é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Fixo { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{9}$", ErrorMessage = "Celular deve estar no formato (99) 999999999")]
        public string Celular { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Comercial é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Comercial { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Endereco é obrigatório.")]
        [RegularExpression(@"^[\p{L}\d\s\-']+,\s*[\p{L}\d\s\-']+,\s*[\p{L}\d\s\-']+\s*-\s*[A-Z]{2}$", ErrorMessage = "Formato esperado: Rua, Bairro, Cidade - UF")]

        public string Endereco { get; set; } = string.Empty;
        [Required(ErrorMessage = "O e-mail é Obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;
        public List<ContatoViewModel> Colaboradores { get; set; } = new();
    }
}
