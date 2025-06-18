using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels.Contato;

namespace ListaTelefonicaIACOApp.Mappers
{
    public static class ContatoMapper
    {

        public static ContatoIndexViewModel MapToViewModel(Contato entidade)
        {
            if (entidade == null)
                return null;

            return new ContatoIndexViewModel
            {
                Contato_Nome = entidade.Nome,
                Contato_Fixo = entidade.Fixo,
                Contato_Celular = entidade.Celular,
                Contato_Comercial = entidade.Comercial,
                Contato_Endereco = entidade.Endereco,
                Contato_Email = entidade.Email
            };
        }


    }
}
