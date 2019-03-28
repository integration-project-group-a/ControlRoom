using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;

class ReceiveLogs
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: "");

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(message);
                
                string jsonText = JsonConvert.SerializeXmlNode(doc,Newtonsoft.Json.Formatting.None,true);
                Console.WriteLine(jsonText);
                //request naar elasticsearch
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.3.56.26:9200/sales/_doc"); //url
                httpWebRequest.ContentType = "application/json"; //ContentType
                httpWebRequest.Method = "POST"; //Methode

                //body
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse(); //sending request
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String result = streamReader.ReadToEnd(); //get result Json string From respons
                    Console.WriteLine("result");
                    Console.WriteLine(result);
                }

            };
            channel.BasicConsume(queue: queueName,autoAck: true,consumer: consumer);
            Console.ReadLine();
        }
       
    }
}