using System.Collections.Generic;

namespace PaperBoy.Presentation.Controllers.Contracts.UnaryCall;

public record RawGrpcCallRequest(
    string ServerAddress,
    string GrpcServiceName,
    string GrpcMethodName,
    Dictionary<string, string> Headers,
    byte[] GrpcRequestBody);