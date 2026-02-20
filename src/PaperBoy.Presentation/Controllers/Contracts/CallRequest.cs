using System.Collections.Generic;

namespace PaperBoy.Presentation.Controllers.Contracts;

public record CallRequest(
    string ServerAddress,
    string GrpcServiceName,
    string GrpcMethodName,
    Dictionary<string, string> Headers,
    byte[] GrpcRequestBody);