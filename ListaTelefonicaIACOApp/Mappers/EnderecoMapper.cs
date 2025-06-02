using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels.Endereco;

namespace ListaTelefonicaIACOApp.Mappers
{
    public static class EnderecoMapper
    {
        public static Endereco MapToEntity(EnderecoCreateViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            return new Endereco
            {
                Rua = viewModel.Endereco_Rua,
                Numero = viewModel.Endereco_Numero,
                Bairro = viewModel.Endereco_Bairro,
                Cidade = viewModel.Endereco_Cidade,
                CEP = viewModel.Endereco_CEP,
                Complemento = viewModel.Endereco_Complemento
            };
        }

    }
}
