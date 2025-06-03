using System.Text.RegularExpressions;

namespace ListaTelefonicaIACOApp.Formatadores
{
    public class EnderecoFormatador
    {
        // Expressão regular: Nome da Rua - Número - Bairro - Cidade
        private static readonly Regex EnderecoRegex = new Regex(
            @"^([A-Za-zÀ-ÿ\s]+)\s*-\s*(\d+)\s*-\s*([A-Za-zÀ-ÿ\s]+)\s*-\s*([A-Za-zÀ-ÿ\s]+)$",
            RegexOptions.Compiled
        );

        public static bool ValidarEnderecoFormatado(string endereco)
        {
            return EnderecoRegex.IsMatch(endereco);
        }

        public static void ExibirPartes(string endereco)
        {
            var match = EnderecoRegex.Match(endereco);
            if (!match.Success)
            {
                Console.WriteLine("Formato inválido.");
                return;
            }

            Console.WriteLine("Rua: " + match.Groups[1].Value.Trim());
            Console.WriteLine("Número: " + match.Groups[2].Value.Trim());
            Console.WriteLine("Bairro: " + match.Groups[3].Value.Trim());
            Console.WriteLine("Cidade: " + match.Groups[4].Value.Trim());
        }
    }
}
