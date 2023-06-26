using MessageEncodeDecode.Models;
using System.Text;
using MessageEncodeDecode.Services;
using MessageEncodeDecode.IServices;

namespace MessageEncodeDecode.Services
{
    public class Messages : IMessages
    {
        private const int MaxHeaderCount = 63;
        private const int MaxHeaderSize = 1023;
        private const int MaxPayloadSize = 256 * 1024;


        public byte[] encode(Message message)
        {
            Dictionary<string, string> headers = message.headers;
            byte[] payload = message.payload;

            if (headers.Count > MaxHeaderCount)
                throw new ArgumentException("Number of headers exceeds the limit.");

            byte[] headerBytes = EncodeHeaders(headers);

            if (payload.Length > MaxPayloadSize)
                throw new ArgumentException("Payload size exceeds the limit.");

            int headerLength = headerBytes.Length;
            int totalMessageLength = headerLength + payload.Length;
            byte[] encodedMessage = new byte[totalMessageLength];

            Array.Copy(headerBytes, encodedMessage, headerLength);

            Array.Copy(payload, 0, encodedMessage, headerLength, payload.Length);

            return encodedMessage;
        }

        public  Message decode(byte[] encodedMessage)
        {
            int headerLength = DecodeHeaderLength(encodedMessage);
            Dictionary<string, string> headers = DecodeHeaders(encodedMessage, headerLength);
            byte[] payload = new byte[encodedMessage.Length - headerLength];
            Array.Copy(encodedMessage, headerLength, payload, 0, payload.Length);
            return new Message() { headers = headers, payload = payload };
        }

        private  byte[] EncodeHeaders(Dictionary<string, string> headers)
        {
            List<byte> headerBytes = new List<byte>();

            foreach (var header in headers)
            {
                if (Encoding.ASCII.GetByteCount(header.Key) > MaxHeaderSize ||
                    Encoding.ASCII.GetByteCount(header.Value) > MaxHeaderSize)
                {
                    throw new ArgumentException("Header name or value size exceeds the limit.");
                }

                byte[] nameBytes = Encoding.ASCII.GetBytes(header.Key);
                headerBytes.AddRange(nameBytes);
                headerBytes.Add(0x00);

                byte[] valueBytes = Encoding.ASCII.GetBytes(header.Value);
                headerBytes.AddRange(valueBytes);
                headerBytes.Add(0x00);
            }

            return headerBytes.ToArray();
        }

        private  int DecodeHeaderLength(byte[] encodedMessage)
        {
            int length = 0;
            for (int i = 0; i < encodedMessage.Length; i++)
            {
                if (encodedMessage[i] == 0x00)
                {
                    length = i + 1;
                    break;
                }
            }

            if (length == 0)
                throw new ArgumentException("Invalid encoded message format.");

            return length;
        }

        private  Dictionary<string, string> DecodeHeaders(byte[] encodedMessage, int headerLength)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            int currentPosition = 0;
            while (currentPosition < headerLength - 1)
            {
                int nullTerminatorIndex = Array.IndexOf(encodedMessage, (byte)0x00, currentPosition);
                if (nullTerminatorIndex < 0 || nullTerminatorIndex >= headerLength)
                    throw new ArgumentException("Invalid encoded message format.");

                int nameLength = nullTerminatorIndex - currentPosition;
                string name = Encoding.ASCII.GetString(encodedMessage, currentPosition, nameLength);
                currentPosition = nullTerminatorIndex + 1;

                nullTerminatorIndex = Array.IndexOf(encodedMessage, (byte)0x00, currentPosition);
                if (nullTerminatorIndex < 0 || nullTerminatorIndex >= headerLength)
                    throw new ArgumentException("Invalid encoded message format.");

                int valueLength = nullTerminatorIndex - currentPosition;
                string value = Encoding.ASCII.GetString(encodedMessage, currentPosition, valueLength);
                currentPosition = nullTerminatorIndex + 1;

                headers.Add(name, value);
            }

            return headers;
        }
    }
}
