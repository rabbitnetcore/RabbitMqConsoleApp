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
        //services.AddSingleton(connection);

        //services.AddScoped(serviceProvider =>
        //{
        //    var conn = serviceProvider.GetService<IConnection>();
        //    return conn.CreateModel();
        //});

        var channel = connection.CreateModel();

        var basicConsumer = new AsyncEventingBasicConsumer(channel);
        basicConsumer.Received += BasicConsumerOnReceived;
        channel.QueueDeclare("queename", true, false, false, null);
		    //publish test message
        //channel.BasicPublish("", "queename", false, null, Encoding.UTF8.GetBytes(" test message"));
        channel.BasicConsume("queename", true, basicConsumer);
        System.Console.ReadLine();
    }


    private static Task BasicConsumerOnReceived(object sender, BasicDeliverEventArgs args)
    {
        var body = args.Body;
        var str = Encoding.UTF8.GetString(body.ToArray());
        System.Console.WriteLine(str);		
        return Task.CompletedTask;
    }
}
