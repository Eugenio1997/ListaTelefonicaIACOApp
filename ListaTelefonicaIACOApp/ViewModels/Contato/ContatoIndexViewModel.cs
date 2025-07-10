using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels.Contato
{
    public class ContatoIndexViewModel
    {

        [Display(Name = "Código do Contato")]
        public int Contato_Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Contato_Nome { get; set; } = string.Empty;
        [Display(Name = "Fixo")]
        //[RegularExpression(@"^\d{2} \d{4}\d{4}$", ErrorMessage = "Formato esperado: 99 99999999")]
        public string Contato_Fixo { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        //[RegularExpression(@"^\d{2} \d{5}\d{4}$", ErrorMessage = "Celular deve estar no formato 99 999999999")]
        public string Contato_Celular { get; set; } = string.Empty;

        [DisplayName("Ramal")]
        //[RegularExpression(@"^\d{4}$", ErrorMessage = "Formato esperado: 0000")]
        public string Contato_Comercial { get; set; } = string.Empty;

        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Contato_Email { get; set; } = string.Empty;
        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O campo endereco é obrigatório.")]
        [RegularExpression(@"^([A-Za-zÀ-ÿ\s]+)\s*-\s*(\d+)\s*-\s*([A-Za-zÀ-ÿ\s]+)\s*-\s*([A-Za-zÀ-ÿ\s]+)$", ErrorMessage = "Formato esperado: Nome da Rua - Número - Bairro - Cidade")]
        public string Contato_Endereco { get; set; } = string.Empty;
        public List<ContatoIndexViewModel> Contatos { get; set; } = new();
        [Display(Name = "Criado Em")]
        public string Contato_CriadoAs { get; set; } = string.Empty;

        [Display(Name = "Editado Em")]
        public string Contato_EditadoAs { get; set; } = string.Empty;


    }
}
