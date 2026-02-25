using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PaperBoy.Core.ProtoParsing;

public interface IProtoParser
{
    public Task<string> ParseToJsonWithStub(
        Stream originalProtoFile,
        string fileName,
        CancellationToken cancellationToken);
    public Task<string> ParseToJson(Stream originalProtoFile, string fileName, CancellationToken cancellationToken);
}