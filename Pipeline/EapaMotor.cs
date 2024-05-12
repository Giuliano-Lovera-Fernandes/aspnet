using System.Text;

namespace Pipeline
{
    public class EtapaMotor : IEtapa<StringBuilder>
    {
        public StringBuilder Processar(StringBuilder entrada)
        {
            entrada.Append("[Motor]");
            return entrada;
        }
    }
}