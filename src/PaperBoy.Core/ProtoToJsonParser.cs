using Google.Protobuf;

namespace PaperBoy.Core;

public class ProtoToJsonParser
{
    public static string ConvertProtoToJson<T>(byte[] data)
        where T : IMessage<T>, new()
    {
        T message = new MessageParser<T>(() => new T()).ParseFrom(data);
        return JsonFormatter.Default.Format(message);
    }
}