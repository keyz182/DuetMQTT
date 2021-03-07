using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Client.Options;
using CommandLine;
using DuetAPI.Connection;
using DuetAPIClient;
using DuetAPI.ObjectModel;


namespace DuetMQTT
{
    class Program
    {
        public class Options
        {
            [Option('q', "quiet", Required = false, HelpText = "Suppress output", Default = false )]
            public bool Quiet { get; set; }
            
            [Option('s', "socket", Required = false, HelpText = "UNIX socket to connect to.", Default = Defaults.FullSocketPath)]
            public string Socket {get; set;}
            
            [Option('t', "topic", Required = false, HelpText = "The topic to send to.", Default = "duet/printer")]
            public string Topic {get; set;}
            
            [Option('h', "host", Required = false, HelpText = "The MQTT Host.", Default = "localhost")]
            public string Host {get; set;}
            
            [Option('p', "port", Required = false, HelpText = "The MQTT Port.", Default = 1883)]
            public int Port {get; set;}
            
            [Option("username", Required = false, HelpText = "The MQTT User.", Default = null)]
            public string Username {get; set;}
            
            [Option("password", Required = false, HelpText = "The MQTT Password.", Default = null)]
            public string Password {get; set;}
            
            [Option("tls", Required = false, HelpText = "Use TLS.", Default = false)]
            public bool TLS {get; set;}
            
            [Option('i', "interval", Required = false, HelpText = "The Interval between calls to duet.", Default = 1000)]
            public int Interval {get; set;}
            
            [Option("drop-thumbs", Required = false, HelpText = "Drop Thumbnails from the payload to reduce size.", Default = false)]
            public bool DropThumbs {get; set;}
        }

        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync<Options>(async o =>
                {
                    if(!o.Quiet) Console.WriteLine("Starting");
                    // Create a new MQTT client.
                    var factory = new MqttFactory();
                    using (SubscribeConnection connection = new SubscribeConnection())
                    using (var mqttClient = factory.CreateMqttClient()){
                        // Create TCP based options using the builder.
                        var _options = new MqttClientOptionsBuilder()
                            .WithClientId("DuetMQTT")
                            .WithTcpServer(o.Host, o.Port)
                            .WithCredentials(o.Username, o.Password)
                            .WithCleanSession();
                        
                        if(o.TLS){
                            _options = _options.WithTls();
                        }
                        var options = _options.Build();

                        if(!o.Quiet) Console.WriteLine("Connecting to MQTT");
                        await mqttClient.ConnectAsync(options, CancellationToken.None);
                        List<string> filter = null;
                        if(!o.Quiet) Console.WriteLine("Connecting to Duet");
                        await connection.Connect(SubscriptionMode.Full, filter, o.Socket, CancellationToken.None);

                        if(!o.Quiet) Console.WriteLine("Fetching Object Model");
                        ObjectModel model = await connection.GetObjectModel(CancellationToken.None);

                        do {
                            if(o.DropThumbs && model.Job != null && model.Job.File != null){
                                if(!o.Quiet) Console.WriteLine("Dropping Thumbnails");
                                model.Job.File.Thumbnails.Clear();
                            }
                            var message = new MqttApplicationMessageBuilder()
                                .WithTopic(o.Topic)
                                .WithPayload(model.ToString())
                                .WithExactlyOnceQoS()
                                .WithRetainFlag()
                                .WithContentType("application/json")
                                .Build();

                            if(!o.Quiet) Console.WriteLine("Publishing");
                            //if(!o.Quiet) Console.WriteLine(model.ToString());
                            await mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken

                            
                            if(!o.Quiet) Console.WriteLine("Waiting");
                            await Task.Delay(TimeSpan.FromMilliseconds(o.Interval));

                            if(!o.Quiet) Console.WriteLine("Updating Model");
                            var json = await connection.GetObjectModelPatch(CancellationToken.None);
                            model.UpdateFromJson(json.RootElement);
                        }while(true);


                    }
                });
        }
    }
}
