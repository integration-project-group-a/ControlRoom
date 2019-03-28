using System;
using RabbitMQ.Client;
using System.Text;
using System.Xml.Linq;
class EmitLog
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            String xmlStr = "<message> <header> <!-- type of message --> <MessageType>Purchase</MessageType> <!--What your Message does --> <description>Adds product purchase to user account</description> <!--Who sent it--> <sender/> <!-- kassa --> </header> <datastructure> <!-- required fields --> <UUID>001</UUID> <!-- id of the user --> <productname>pintje</productname> <price>1.00</price> <amount>2</amount> <total>2,00</total> <timestamp></timestamp> <version>1</version> <isActive>true</isActive> <!-- Not required fields --> <extraField/> </datastructure> </message> ";

            var messagetest = XDocument.Parse(xmlStr);

            var body = Encoding.UTF8.GetBytes(xmlStr);
            channel.BasicPublish(exchange: "logs",routingKey: "",basicProperties: null,body: body);
        }
    }
}