using System.Text;

namespace Pipeline
{
    public class EtapaPortas : IEtapa<StringBuilder>
    {
        public StringBuilder Processar(StringBuilder entrada)
        {
            entrada.Insert(0, "[Porta]", 2);
            entrada.Insert(entrada.Length, "[Porta]", 2);
            return entrada;
        }
    }
}