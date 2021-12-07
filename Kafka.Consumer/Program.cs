//dotnet run "manual" "localhost:9092" "COTACAO_ACOES"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using Serilog;

/// <summary>
///     Demonstração usando Consumer client.
/// </summary>
namespace Kafka.Consumer
{

    public class Program
    {

        /// <summary>
        ///     Nesse exemplo
        ///         - Os offsets são confirmados manualmente.
        ///         - Nenhum thread extra é criado para o loop Poll (Consume).
        /// </summary>
        public static void Run_Consume(string brokerList, List<string> topics, CancellationToken cancellationToken)
        {
            //Todo: Construa uma instância da 'ConsumerConfig' classe fortemente tipada, em seguida, passe-a para o 'ConsumerBuilder' construtor:
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "csharp-consumer",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                // A good introduction to the CooperativeSticky assignor and incremental rebalancing:
                // https://www.confluent.io/blog/cooperative-rebalancing-in-kafka-streams-consumer-ksqldb/
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
            };

            const int commitPeriod = 5;

            using IConsumer<Ignore, string> consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => Log.Information($"Error: {e.Reason}"))
                .SetStatisticsHandler((_, json) => Log.Information($"Statistics: {json}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Log.Information(
                            "Partitions incrementally assigned: [" +
                            string.Join(',', partitions.Select(p => p.Partition.Value)) +
                            "], all: [" +
                            string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)) +
                            "]");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                    Log.Information(
                        "Partitions incrementally revoked: [" +
                        string.Join(',', partitions.Select(p => p.Partition.Value)) +
                        "], remaining: [" +
                        string.Join(',', remaining.Select(p => p.Partition.Value)) +
                        "]");
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    Log.Information($"Partitions were lost: [{string.Join(", ", partitions)}]");
                })
                .Build();
            consumer.Subscribe(topics);

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult.IsPartitionEOF)
                        {
                            Log.Information($"Recebendo do topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                            continue;
                        }

                        Log.Information($"Recebendo a mensagem para {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");

                        if (consumeResult.Offset % commitPeriod == 0)
                        {
                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (KafkaException e)
                            {
                                Log.Information($"Commit error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Log.Information($"Consumo com erro: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("Fechando o Consumo.");
                consumer.Close();
            }
        }

        /// <summary>
        ///     Nesse exemplo
        ///         - Funcionalidade de grupo de consumidores (ou seja .Subscribe + offset commits) não são usadas.
        ///         - O consumidor é atribuído manualmente a uma partição e sempre inicia o consumo de um deslocamento específico (0).
        /// </summary>
        public static void Run_ManualAssign(string brokerList, List<string> topics, CancellationToken cancellationToken)
        {
            Log.Information("Iniciando o modo Manual...");
            var config = new ConsumerConfig
            {
                GroupId = new Guid().ToString(),
                BootstrapServers = brokerList,
                EnableAutoCommit = true
            };
            Log.Information($"GroupId:{config.GroupId} BootstrapServers:{config.BootstrapServers}");

            using var consumer =
                new ConsumerBuilder<Ignore, string>(config)
                    .SetErrorHandler((_, e) => Log.Error($"Error: {e.Reason}"))
                    .Build();

            consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 0, Offset.Beginning)).ToList());

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        Log.Information($"Recebendo a mensagem: {consumeResult.TopicPartitionOffset}: ${consumeResult.Message.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        Log.Error($"Consumo com erro: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("Fechando o consumo.");
                consumer.Close();
            }
        }

        private static void PrintUsage()
            => Console.WriteLine("Usage: .. <subscribe|manual> <broker,broker,..> <topic> [topic..]");

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Testando o Consumer com o Kafka");

            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }

            var mode = args[0];
            var brokerList = args[1];
            var topics = args.Skip(2).ToList();

            Log.Information($"Mode:{mode} brokerList:{brokerList} topics{topics}");
            Log.Information($"Iniciando o Consumo, Ctrl-C para parar o Consumo");

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                cts.Cancel();
            };

            switch (mode)
            {
                case "subscribe":
                    Run_Consume(brokerList, topics, cts.Token);
                    break;
                case "manual":
                    Run_ManualAssign(brokerList, topics, cts.Token);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }
    }
    
}
