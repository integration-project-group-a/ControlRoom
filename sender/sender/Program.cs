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
            String xmlStr;
            byte[] body;
            xmlStr = "<Message> <header> <!-- type of message --> <MessageType>Visitor</MessageType> <!--What your Message does --> <description>Creation of a visitor</description> <!--Who sent it--> <!--(fronted, crm, facturatie, kassa, monitor, planning, uuid) --> <sender>front-end</sender> <!-- kassa, crm, front-end --> </header> <datastructure> <!-- required fields = UUID name + email & hashing. --> <UUID>200</UUID> <!-- id of the user --> <name> <firstname>Anthe</firstname> <lastname>Boets</lastname> </name> <!-- kassa , front-end --> <email>anthe.boets@student.ehb.be</email> <GDPR>true</GDPR> <timestamp>1999-04-30T00:00:00+00:00</timestamp> <version>1</version> <isActive>true</isActive> <banned>false</banned> <!-- Not required fields --> <geboortedatum>1999-04-30T00:00:00+00:00</geboortedatum> <btw-nummer/> <gsm-nummer/> <extraField/> </datastructure> </Message>";

            body = Encoding.UTF8.GetBytes(xmlStr);
            channel.BasicPublish(exchange: "logs",routingKey: "",basicProperties: null,body: body);

            Console.ReadKey();

            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            xmlStr = "<Message> <header> <!-- type of message --> <MessageType>Visitor</MessageType> <!--What your Message does --> <description>Update of a visitor</description> <!--Who sent it--> <!--(fronted, crm, facturatie, kassa, monitor, planning, uuid) --> <sender>front-end</sender> <!-- kassa, crm, front-end --> </header> <datastructure> <!-- required fields = UUID name + email & hashing. --> <UUID>200</UUID> <!-- id of the user --> <name> <firstname>Anthe234</firstname> <lastname>Boets</lastname> </name> <!-- kassa , front-end --> <email>anthe.boets@student.ehb.be</email> <GDPR>true</GDPR> <timestamp>1999-04-30T00:00:00+00:00</timestamp> <version>1</version> <isActive>true</isActive> <banned>false</banned> <!-- Not required fields --> <geboortedatum>1999-04-30T00:00:00+00:00</geboortedatum> <btw-nummer/> <gsm-nummer/> <extraField/> </datastructure> </Message>";

            body = Encoding.UTF8.GetBytes(xmlStr);
            channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);


            Console.ReadKey();
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            xmlStr = "<Message> <header> <!-- type of message --> <MessageType>Visitor</MessageType> <!--What your Message does --> <description>Deletion of a visitor</description> <!--Who sent it--> <!--(fronted, crm, facturatie, kassa, monitor, planning, uuid) --> <sender>front-end</sender> <!-- kassa, crm, front-end --> </header> <datastructure> <!-- required fields = UUID name + email & hashing. --> <UUID>200</UUID> <!-- id of the user --> <name> <firstname>Anthe234</firstname> <lastname>Boets</lastname> </name> <!-- kassa , front-end --> <email>anthe.boets@student.ehb.be</email> <GDPR>true</GDPR> <timestamp>1999-04-30T00:00:00+00:00</timestamp> <version>1</version> <isActive>true</isActive> <banned>false</banned> <!-- Not required fields --> <geboortedatum>1999-04-30T00:00:00+00:00</geboortedatum> <btw-nummer/> <gsm-nummer/> <extraField/> </datastructure> </Message>";

            body = Encoding.UTF8.GetBytes(xmlStr);
            channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);

        }
    }
}