using System;
using Serilog;
using System.IO;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace Kafka.Producer.Demo
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Executando um Produtor de Kafka com C#");

            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            Log.Information($"Configurações do produtor - Broker:'{config.BootstrapServers}'");
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var deliveryResult = await producer.ProduceAsync("Nova_Consulta", new Message<Null, string> { Value = "Computador gamer" });
                    Log.Information($"Entrega de '{deliveryResult.Value}' para o Topico:'{deliveryResult.Topic}' partition:'{deliveryResult.Partition}' e offset:'{deliveryResult.Offset}' ");
                }
                catch (ProduceException<Null, string> e)
                {
                    Log.Information($"Entrega Falhou: {e.Error.Reason}");
                }
            }
            Console.ReadLine();
        }
    }
}
