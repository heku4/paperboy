using Google.Protobuf;

namespace PaperBoy.Core.ProtoParsing;

public class ProtoToJsonParser
{
    public static string ConvertProtoMessageToJson<T>(byte[] data)
        where T : IMessage<T>, new()
    {
        T message = new MessageParser<T>(() => new T()).ParseFrom(data);
        return JsonFormatter.Default.Format(message);
    }
}