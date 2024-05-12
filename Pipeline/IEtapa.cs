namespace Pipeline
{
    public interface IEtapa<T>
    {
        T Processar(T Entrada);
    }
}