using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaperBoy.Core;
using PaperBoy.Presentation.Controllers.Contracts;

namespace PaperBoy.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController
{
    private readonly IGrpcCallProvider _grpcCallProvider;

    public DebugController(IGrpcCallProvider grpcCallProvider)
    {
        _grpcCallProvider = grpcCallProvider;
    }

    [HttpPost(nameof(Make))]
    public async Task<RawGrpcCallResponse> Make([FromBody] RawGrpcCallRequest request)
    {
        byte[] responseBytes = await _grpcCallProvider.MakeUnaryCall(
            request.ServerAddress,
            request.GrpcServiceName,
            request.GrpcMethodName,
            request.GrpcRequestBody,
            request.Headers);

        return new RawGrpcCallResponse(responseBytes);
    }

    [HttpPost(nameof(ConvertToJson))]
    public async Task<string> ConvertToJson([FromBody] RawGrpcCallRequest request)
    {
        return string.Empty;
    }
}