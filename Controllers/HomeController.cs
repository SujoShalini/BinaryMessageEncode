using MessageEncodeDecode.IServices;
using MessageEncodeDecode.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MessageEncodeDecode.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMessages _messages;

        public HomeController(ILogger<HomeController> logger, IMessages messages)
        {
            _logger = logger;
            _messages = messages;
        }

        public IActionResult Index()
        {

           //Encoding and decoding a binary message with headers and a payload
       Dictionary<string, string> headers = new Dictionary<string, string>()
        {
            { "Header1", "Value1" },
            { "Header2", "Value2" }
        };
            byte[] payload = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            // Encode the message
            Message message = new Message() { headers = headers, payload = payload };
            byte[] encodedMessage = _messages.encode(message);

            // Decode the message
            message = _messages.decode(encodedMessage);

            // Retrieve the headers and payload
            Dictionary<string, string> decodedHeaders = message.headers;
            byte[] decodedPayload = message.payload;

            // Print the original and decoded headers
            Console.WriteLine("Original Headers:");
            foreach (var header in headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            Console.WriteLine("Decoded Headers:");
            foreach (var header in decodedHeaders)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            // Print the original and decoded payload
            Console.WriteLine("Original Payload:");
            foreach (byte b in payload)
            {
                Console.Write($"{b} ");
            }
            Console.WriteLine();

            Console.WriteLine("Decoded Payload:");
            foreach (byte b in decodedPayload)
            {
                Console.Write($"{b} ");
            }
            Console.WriteLine();

            return View();
        }

    }
}