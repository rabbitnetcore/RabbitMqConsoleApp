class Program
    {

       
        static void Main(string[] args)
        {
          

            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                AutomaticRecoveryEnabled = true,
                HostName = "localhost",
                Port = 5672,
                DispatchConsumersAsync = true
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var basicConsumer = new AsyncEventingBasicConsumer(channel);
            basicConsumer.Received += BasicConsumerOnReceived;
            channel.QueueDeclare("queename", true, false, false, null);
            channel.BasicConsume("queename", true, basicConsumer);
            System.Console.ReadLine();
        }


        private static Task BasicConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
           
            var elaasticClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")));
            var httpClient = new System.Net.Http.HttpClient();  
            httpClient.BaseAddress = new Uri("http://example.com/api");
            var _service = new SampleService(httpClient);
            var body = args.Body;
            var str = Encoding.UTF8.GetString(body.ToArray());
            var dto = JsonSerializer.Deserialize<SampleDto>(str);
            System.Console.WriteLine(str);                      
            _service.Methods(dto.Id);
            File.AppendAllLines("log.txt",new[] { str});
            elaasticClient.Index(str, d => d.Index("elasticlog"));
            return Task.CompletedTask;
        }
    }
