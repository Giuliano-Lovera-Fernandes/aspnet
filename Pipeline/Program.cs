// See https://aka.ms/new-console-template for more information
using System.Text;
using Pipeline;

Console.WriteLine("Pipeline Simples para montagem de veículos");
var montagemVeiculo = new Pipeline<StringBuilder>();
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaChassi());
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaMotor());
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaBancos());
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaCarroceria());
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaPortas());
montagemVeiculo.AdicionarEtapa(new Pipeline.EtapaPintura());

for (int i = 0; i < 10; i++)
{
    var veiculo = montagemVeiculo.Processar(new StringBuilder());
    Console.WriteLine($"Veículo {i + 1:D2}: {veiculo.ToString()}");
}
