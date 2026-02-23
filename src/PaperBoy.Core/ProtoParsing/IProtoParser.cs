namespace PaperBoy.Core.ProtoParsing;

public interface IProtoParser
{
    public string ParseToJsonWithStub(byte[] protoDescription);
}