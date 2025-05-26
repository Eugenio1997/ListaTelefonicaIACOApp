using ListaTelefonicaIACOApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class ContatoIndexViewModel
    {

        [Display(Name = "Código do Contato")]
        public int Contato_Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Contato_Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
        [Display(Name = "Sobrenome")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Sobrenome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Contato_Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Fixo é obrigatório.")]
        [Display(Name = "Telefone Fixo")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Contato_Fixo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [Display(Name = "Celular")]
        [RegularExpression(@"^\(\d{2}\) \d{9}$", ErrorMessage = "Celular deve estar no formato (99) 999999999")]
        public string Contato_Celular { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Comercial é obrigatório.")]
        [Display(Name = "Telefone Comercial")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Contato_Comercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é Obrigatório.")]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Contato_Email { get; set; } = string.Empty;
        public int Contato_EnderecoId { get; set; }
        public List<ContatoIndexViewModel> Colaboradores { get; set; } = new();
        [Display(Name = "Endereço")]
        public EnderecoViewModel Endereco { get; set; } = new();

    }
}
