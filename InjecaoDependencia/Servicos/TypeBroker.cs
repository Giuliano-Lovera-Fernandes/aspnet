public static class TypeBroker
{
    public static IFormatadorEndereco instanciaCompartilhada = new EnderecoHtml();

    //Retorna o atributo instância compartilhada.
    public static IFormatadorEndereco FormatadorEndereco => instanciaCompartilhada;
}
