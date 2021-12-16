## Projeto de estudos sobre o Kafka.


1. Mensageria e Kafka
2. Instalando o Kafka localmente

    Link para Dowload do Kafka: [https://kafka.apache.org/downloads](https://kafka.apache.org/downloads)
    
    Lugar onde se armazena os dados é no zookeeper, download: [https://zookeeper.apache.org/releases.html](https://zookeeper.apache.org/releases.html)
    
    <aside>
    💡 O Kafka já vêm com o Zookeeper
    
    </aside>
    
    ```powershell
    bin/zookeeper-server-start.sh config/zookeeper.properties
    ```
    <img width="1182" alt="Captura de Tela 2021-12-07 às 04 47 39" src="https://user-images.githubusercontent.com/19518771/145032157-5b47133f-ae9c-454f-8b95-b61966757d52.png">
    
    ```powershell
    bin/kafka-server-start.sh config/server.properties
    ```
    <img width="1358" alt="Captura de Tela 2021-12-07 às 04 52 54" src="https://user-images.githubusercontent.com/19518771/145032231-fe757353-9187-440a-9348-0395231795f4.png">
    
    > Criação de um tópico:
    bin/kafka-topics → caminho do aquivo que será executado
    — create → Comando de criação
    —bootstrap-server [localhost:9092](http://localhost:9092) → onde o server está rodando
    —replication-factor 1 → quantidade de replicas de um tópico em brokers diferentes
    —partitions 1 → quatidade de partições em um tópico
    —topic COTACAO_ACOES → nome do tópico
    > 
    
    ```powershell
    bin/kafka-topics.sh --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic COTACAO_ACOES
    ```
    <img width="1358" alt="Captura de Tela 2021-12-07 às 05 11 07" src="https://user-images.githubusercontent.com/19518771/145032307-c72058cb-e208-4d05-961a-50de128700e9.png">
    
    > Para saber se o tópico foi criado:
    bin/kafka-topics → caminho do aquivo que será executado
    —list → comando de listar
    —bootstrap-server [localhost:9092](http://localhost:9092) → onde o server está rodando
    > 
    
    ```powershell
    bin/kafka-topics.sh --list --bootstrap-server localhost:9092
    ```
    <img width="1358" alt="Captura de Tela 2021-12-07 às 05 16 12" src="https://user-images.githubusercontent.com/19518771/145032357-dcbbf396-800d-4be6-b52a-3ce017cb5d6e.png">
    
    > Criando podutores:
    bin/kafka-console-producer.sh → caminho do arquivo que será executado
    —broker-list [localhost:9092](http://localhost:9092) → comando que será executado, apontando o broker que receberá as mensagens
    —topic COTACAO_ACOES → nome do tópico que será enviado as mensagens
    > 
    
    ```powershell
    bin/kafka-console-producer.sh --broker-list localhost:9092 --topic COTACAO_ACOES
    ```
    <img width="1358" alt="Captura de Tela 2021-12-07 às 05 27 02" src="https://user-images.githubusercontent.com/19518771/145032470-f3e8efc1-da70-4f47-bd56-aa8f9e3723f9.png">
    
    > Criando Consumidores:
    bin/kafka-console-consumer.sh → caminho do arquivo que será executado
    --bootstrap-server localhost:9092 →Configurações do server onde está as mensagens
    --topic COTACAO_ACOES → Tópico que escutaremos
    --from-beginning → Opicional, caso queira esctutar dês do início
    > 
    
    ```powershell
    bin/kafka-console-consumer.sh --bootstrap-server localhost:9092 --topic COTACAO_ACOES --from-beginning
    ```
    <img width="1358" alt="Captura de Tela 2021-12-07 às 05 34 13" src="https://user-images.githubusercontent.com/19518771/145032507-2e8a9fdd-e8c8-49ac-9e86-bcc0f2f5abb6.png">


    <!--https://github.com/marraia/Kafka-->
