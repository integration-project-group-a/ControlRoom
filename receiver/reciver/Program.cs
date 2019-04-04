using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

                JObject jObject = JObject.Parse(jsonText);

                HttpWebRequest httpWebRequest;
                HttpWebResponse httpWebResponse;
                string elasticId;
                switch ((string)jObject["header"]["description"])
                {
                    case "Creation of a visitor":
                        //request naar elasticsearch
                        httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.3.56.26:9200/test/anthe"); //url
                        httpWebRequest.ContentType = "application/json"; //ContentType
                        httpWebRequest.Method = "POST"; //Methode

                        //body
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(jsonText);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse(); //sending request
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            String result = streamReader.ReadToEnd(); //get result Json string From respons
                            Console.WriteLine("result");
                            Console.WriteLine(result);
                        }
                        break;
                    case "Update of a visitor":
                        elasticId = GetId();
                        
                        //request naar elasticsearch
                        httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.3.56.26:9200/test/anthe/" + elasticId); //url
                        httpWebRequest.ContentType = "application/json"; //ContentType
                        httpWebRequest.Method = "PUT"; //Methode

                        //body
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(jsonText);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse(); //sending request
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            String result = streamReader.ReadToEnd(); //get result Json string From respons
                            Console.WriteLine("result");
                            Console.WriteLine(result);
                        }
                        break;
                    case "Deletion of a visitor":

                        elasticId = GetId();
                        //request naar elasticsearch
                        httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.3.56.26:9200/test/anthe/" + elasticId); //url
                        httpWebRequest.ContentType = "application/json"; //ContentType
                        httpWebRequest.Method = "DELETE"; //Methode

                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse(); //sending request
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            String result = streamReader.ReadToEnd(); //get result Json string From respons
                            Console.WriteLine("result");
                            Console.WriteLine(result);
                        }
                        break;
                }

                

            };
            channel.BasicConsume(queue: queueName,autoAck: true,consumer: consumer);
            Console.ReadLine();
        }
       
    }
    public static string GetId()
    {
        HttpWebRequest httpWebRequest;
        HttpWebResponse httpWebResponse;
        string elasticId;
        //request naar elasticsearch
        httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.3.56.26:9200/test/anthe/_search"); //url
        httpWebRequest.ContentType = "application/json"; //ContentType
        httpWebRequest.Method = "POST"; //Methode

        //body
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            streamWriter.Write("{\"query\": {\"terms\": {\"datastructure.UUID\": [\"" + (string)jObject["datastructure"]["UUID"] + "\"]}}}");
            streamWriter.Flush();
            streamWriter.Close();
        }

        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse(); //sending request
        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
        {
            String result = streamReader.ReadToEnd(); //get result Json string From respons
            Console.WriteLine("result");
            JObject jObjectResponse = JObject.Parse(result);
            Console.WriteLine(result);
            Console.WriteLine("elasticId: " + (string)jObjectResponse["hits"]["hits"][0]["_id"]);
            elasticId = (string)jObjectResponse["hits"]["hits"][0]["_id"];
        }
        return "elasticId";
    }
}