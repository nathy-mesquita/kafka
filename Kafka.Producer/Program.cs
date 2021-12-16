//dotnet run "localhost:9092" "COTACAO_ACOES"
//"Mensagem de teste 1" "Mensagem de teste 2"
using System;
using Serilog;
using System.IO;
using Confluent.Kafka;
using System.Threading.Tasks;

/// <summary>
///     Demonstração usando Producer.
/// </summary>
namespace Kafka.Producer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Testando o Producer com o Kafka");

            if (args.Length != 2)
            {
                Log.Error("Envie o BrokerList - ip/porta para testes no kafka, " +
                        "envie o topicName - Tópico que receberá a mensagem");
                return;
            }

            string brokerList = args[0];
            string topicName = args[1];

            Log.Information($"BrokerList = {brokerList}");
            Log.Information($"TopicName = {topicName}");

            //Todo: Primeiro construa uma instância da 'ProducerConfig' classe fortemente tipada e, em seguida, passe-a para o 'ProducerBuilder' construtor:
            var config = new ProducerConfig { BootstrapServers = brokerList};

            using var producer = new ProducerBuilder<string, string>(config).Build();
            Log.Information("\n----------------------------------------------------------------------");
            Log.Information($"Produtor {producer.Name} Produzindo no tópico {topicName}.");
            Log.Information("-----------------------------------------------------------------------");
            Log.Information("Para criar uma mensagem kafka com chave e valor codificado UTF-8: ");
            Log.Information("> Chave <Enter>");
            Log.Information("Para criar uma mensagem kafka com uma chave nula e valor codificado em UTF-8: ");
            Log.Information("> Valor <Enter>");
            Log.Information("Ctrl-C para sair.\n");

            var cancelled = false;
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cancelled = true;
            };

            while (!cancelled)
            {
                Console.Write("> ");

                string text;
                try
                {
                    text = Console.ReadLine();
                }
                catch (IOException)
                {
                    break;
                }
                if (text == null)
                {
                    break;
                }

                string key = "1";
                string val = text;

                int index = text.IndexOf(" ");
                if (index != -1)
                {
                    key = text.Substring(0, index);
                    val = text.Substring(index + 1);
                }

                try
                {
                    var deliveryReport = await producer.ProduceAsync(topicName, new Message<string, string> { Key = key, Value = val });
                    Log.Information($"Entregue para: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    Log.Information($"Falhou ao entregar a mensagem: {e.Message} [{e.Error.Code}]");
                    Console.WriteLine();
                }
            }
        }
    }
}
