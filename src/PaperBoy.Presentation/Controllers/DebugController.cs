using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaperBoy.Core.ProtoParsing;
using PaperBoy.Core.Unary;
using PaperBoy.Presentation.Controllers.Contracts.UnaryCall;

namespace PaperBoy.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController
{
    private readonly IGrpcCallProvider _grpcCallProvider;
    private readonly IProtoParser _protoParser;

    public DebugController(IGrpcCallProvider grpcCallProvider, IProtoParser protoParser)
    {
        _grpcCallProvider = grpcCallProvider;
        _protoParser = protoParser;
    }

    [HttpPost(nameof(MakeUnaryCall))]
    public async Task<RawGrpcCallResponse> MakeUnaryCall([FromBody] RawGrpcCallRequest request)
    {
        byte[] responseBytes = await _grpcCallProvider.MakeUnaryCall(
            request.ServerAddress,
            request.GrpcServiceName,
            request.GrpcMethodName,
            request.GrpcRequestBody,
            request.Headers);

        return new RawGrpcCallResponse(responseBytes);
    }

    [HttpPost(nameof(ConvertToJsonWithStubs))]
    public async Task<string> ConvertToJsonWithStubs(IFormFile file)
    {
        await using Stream fileStream = file.OpenReadStream();

        return await _protoParser.ParseToJsonWithStub(fileStream, file.FileName, CancellationToken.None);
    }

    [HttpPost(nameof(ConvertToJson))]
    public async Task<string> ConvertToJson([FromForm] IFormFile file)
    {
        await using Stream fileStream = file.OpenReadStream();

        return await _protoParser.ParseToJson(fileStream, file.FileName, CancellationToken.None);
    }
}