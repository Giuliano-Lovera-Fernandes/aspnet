using System.Text;

namespace Pipeline
{
    public class EtapaCarroceria : IEtapa<StringBuilder>
    {
        public StringBuilder Processar(StringBuilder entrada)
        {
            entrada.Insert(0, "[Carroceria]", 1);
            entrada.Insert(entrada.Length, "[Carroceria]", 1);
            return entrada;
        }
    }
}