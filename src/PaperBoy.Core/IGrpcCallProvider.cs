using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperBoy.Core;

public interface IGrpcCallProvider
{
    public Task<byte[]> MakeUnaryCall(
        string serverUrl,
        string grpcServiceName,
        string grpcMethodName,
        byte[] requestBody,
        Dictionary<string, string> headers);
}