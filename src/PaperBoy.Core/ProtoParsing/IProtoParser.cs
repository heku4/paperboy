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
    
    /// <summary>
    /// This method use Google.Protobuf.JsonFormatter to return JSON-structured data from proto descriptor.
    /// </summary>
    /// <param name="originalProtoFile">Original .proto contract passed as Stream.</param>
    /// <param name="fileName">Name of descriptor file.</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Data form proto contract in JSON-structured description.</returns>
    public Task<string> ParseToJson(Stream originalProtoFile, string fileName, CancellationToken cancellationToken);
}