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
    
    ![Captura de Tela 2021-12-07 às 04.47.39.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/a459e778-a0c4-4736-8a09-c6f982d96750/Captura_de_Tela_2021-12-07_as_04.47.39.png)
    
    ```powershell
    bin/kafka-server-start.sh config/server.properties
    ```
    
    ![Captura de Tela 2021-12-07 às 04.52.54.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/8203f80c-c6a3-4858-a1ad-7d6b259c1811/Captura_de_Tela_2021-12-07_as_04.52.54.png)
    
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
    
    ![Captura de Tela 2021-12-07 às 05.11.07.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/d374f868-aedb-4aa3-a23c-3711610422a8/Captura_de_Tela_2021-12-07_as_05.11.07.png)
    
    > Para saber se o tópico foi criado:
    bin/kafka-topics → caminho do aquivo que será executado
    —list → comando de listar
    —bootstrap-server [localhost:9092](http://localhost:9092) → onde o server está rodando
    > 
    
    ```powershell
    bin/kafka-topics.sh --list --bootstrap-server localhost:9092
    ```
    
    ![Captura de Tela 2021-12-07 às 05.16.12.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/73459bfa-4c85-470d-a6e9-da462275c1cc/Captura_de_Tela_2021-12-07_as_05.16.12.png)
    
    > Criando podutores:
    bin/kafka-console-producer.sh → caminho do arquivo que será executado
    —broker-list [localhost:9092](http://localhost:9092) → comando que será executado, apontando o broker que receberá as mensagens
    —topic COTACAO_ACOES → nome do tópico que será enviado as mensagens
    > 
    
    ```powershell
    bin/kafka-console-producer.sh --broker-list localhost:9092 --topic COTACAO_ACOES
    ```
    
    ![Captura de Tela 2021-12-07 às 05.27.02.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/f2c6e2a4-ad01-49eb-833f-4b5db2d37ac2/Captura_de_Tela_2021-12-07_as_05.27.02.png)
    
    > Criando Consumidores:
    bin/kafka-console-consumer.sh → caminho do arquivo que será executado
    --bootstrap-server localhost:9092 →Configurações do server onde está as mensagens
    --topic COTACAO_ACOES → Tópico que escutaremos
    --from-beginning → Opicional, caso queira esctutar dês do início
    > 
    
    ```powershell
    bin/kafka-console-consumer.sh --bootstrap-server localhost:9092 --topic COTACAO_ACOES --from-beginning
    ```
    
    ![Captura de Tela 2021-12-07 às 05.34.13.png](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/90a33313-2057-4559-938a-3587de62d2ef/Captura_de_Tela_2021-12-07_as_05.34.13.png)