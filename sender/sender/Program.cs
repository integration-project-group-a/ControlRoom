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

            String xmlStr = "<VISITOR> <UUID>0001</UUID> <SYSTEMID>1</SYSTEMID> <NAAM> <VOORNAAM>Anthe</VOORNAAM> <ACHTERNAAM>Boets</ACHTERNAAM> </NAAM> <GEBOORTEDATUM>1999-04-30T00:00:00+00:00 </GEBOORTEDATUM> <BTW-NUMMER>BE 0123.321.123</BTW-NUMMER> <GSM>0482455642</GSM> <EVENT> <SESSIE>SESSIE1</SESSIE> <SESSIE>SESSIE2</SESSIE> </EVENT> <CREATED> <TIMESTAMP>2019-03-13T18:52:30+00:00</TIMESTAMP> <VERSION>1</VERSION> </CREATED> <ISACTIVE>True</ISACTIVE> <SENDER>Front-end</SENDER> <TYPE-GEBRUIKER>Visitor</TYPE-GEBRUIKER> <GDPR>True</GDPR> <BANNED>False</BANNED> </VISITOR>";

            var messagetest = XDocument.Parse(xmlStr);

            var body = Encoding.UTF8.GetBytes(xmlStr);
            channel.BasicPublish(exchange: "logs",routingKey: "",basicProperties: null,body: body);
        }
    }
}