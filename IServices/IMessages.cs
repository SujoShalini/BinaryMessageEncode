using MessageEncodeDecode.Models;

namespace MessageEncodeDecode.IServices
{
    public interface IMessages
    {
        byte[] encode(Message message);
        Message decode(byte[] data);
    }
}
