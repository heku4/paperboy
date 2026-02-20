using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaperBoy.Core;
using PaperBoy.Presentation.Controllers.Contracts;

namespace PaperBoy.Presentation.Controllers;

[ApiController]
public class RawCallController
{
    private readonly IGrpcCallProvider _grpcCallProvider;

    public RawCallController(IGrpcCallProvider grpcCallProvider)
    {
        _grpcCallProvider = grpcCallProvider;
    }

    [HttpPost(nameof(Make))]
    public async Task<CallResponse> Make([FromBody] CallRequest request)
    {
        byte[] responseBytes = await _grpcCallProvider.MakeUnaryCall(
            request.ServerAddress,
            request.GrpcServiceName,
            request.GrpcMethodName,
            request.GrpcRequestBody,
            request.Headers);

        return new CallResponse(responseBytes);
    }
}