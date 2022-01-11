using System;
using Confluent.Kafka;
using Serilog;

namespace Kafka.Consumer.Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Executando um Consumidor do Kafka com C#");

            var config = new ConsumerConfig
            {
                GroupId = "BUSQUEAKI",
                BootstrapServers = "localhost:9092"
            };
            Log.Information($"Configurações do consumidor - GroupID:'{config.GroupId}' e  Broker:'{config.BootstrapServers}'");

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("Nova_Consulta");
            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();
                        Log.Information($"Mensagem consumida é '{consumeResult.Message.Value}' do Topico:'{consumeResult.Topic}' na partition:'{consumeResult.Partition}' e offset:'{consumeResult.Offset}' ");
                    }
                    catch (ConsumeException e)
                    {
                        Log.Error($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("Garante que o consumidor deixou o grupo limpo e o commit foi feito nos offsets finais");
                consumer.Close();
            }
        }
    }

}
