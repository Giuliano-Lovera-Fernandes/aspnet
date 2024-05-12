using System.Text;

namespace Pipeline
{
    public class EtapaPintura : IEtapa<StringBuilder>
    {
        public StringBuilder Processar(StringBuilder entrada)
        {
            string[] cores = { "Preto", "Branco", "Vermelho", "Grafite", };
            var random = new Random();
            var corAleatoria = cores[random.Next(0, cores.Length)];
            entrada.Insert(0, $"[{corAleatoria}]", 1);
            entrada.Insert(entrada.Length, $"[{corAleatoria}]", 1);
            return entrada;
        }
    }
}