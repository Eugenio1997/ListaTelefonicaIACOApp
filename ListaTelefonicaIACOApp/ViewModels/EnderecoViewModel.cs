using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class EnderecoViewModel
    {
        [Display(Name = "Código do Endereço")]
        public int Endereco_Id { get; set; }

        [Required(ErrorMessage = "A rua é obrigatória.")]
        [Display(Name = "Rua")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A rua deve ter entre 3 e 100 caracteres.")]
        public string Endereco_Rua { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        [Display(Name = "Número")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O número deve conter apenas dígitos.")]
        public string Endereco_Numero { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [Display(Name = "Bairro")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres.")]
        public string Endereco_Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [Display(Name = "Cidade")]
        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
        public string Endereco_Cidade { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [Display(Name = "CEP")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
        public string Endereco_CEP { get; set; }

        [Display(Name = "Complemento")]
        public string? Endereco_Complemento { get; set; }

    }
}
