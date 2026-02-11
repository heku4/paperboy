using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace PaperBoy.Core;

public class GrpcCallProvider : IGrpcCallProvider
{
    /// <summary>
    ///     Prepare and make gRPC call to external server.
    /// </summary>
    /// <param name="serverUrl">External server address.</param>
    /// <param name="grpcServiceName">Full gRPC service name with package prefix from proto-file.</param>
    /// <param name="grpcMethodName">gRPC method name from proto-file.</param>
    /// <param name="requestBody">Request body serialized into bytes.</param>
    /// <param name="headers">Custom call headers.</param>
    /// <returns>Raw response in bytes.</returns>
    public async Task<byte[]> MakeUnaryCall(
        string serverUrl,
        string grpcServiceName,
        string grpcMethodName,
        byte[] requestBody,
        Dictionary<string, string> headers)
    {
        Method<byte[], byte[]> grpcMethod = new(
            MethodType.Unary,
            grpcServiceName,
            grpcMethodName,
            Marshallers.Create(bytes => bytes, bytes => bytes),
            Marshallers.Create(bytes => bytes, bytes => bytes));

        using GrpcChannel grpcChannel = GrpcChannel.ForAddress(serverUrl);
        CallInvoker invoker = grpcChannel.CreateCallInvoker();
        byte[] responseBytes = await invoker.AsyncUnaryCall(
            grpcMethod,
            serverUrl,
            new CallOptions(),
            requestBody);

        return responseBytes;
    }
}