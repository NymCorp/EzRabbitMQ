using EzRabbitMQ;
using EzRabbitMQ.Tests.Messages;
using Microsoft.AspNetCore.Mvc;

namespace WebSample.Controllers;

[ApiController]
[Route("[controller]")]
public class RpcMessageSenderController : ControllerBase
{
    private readonly ILogger<RpcMessageSenderController> _logger;
    private readonly IMailboxService _mailboxService;

    public RpcMessageSenderController(ILogger<RpcMessageSenderController> logger, IMailboxService mailboxService)
    {
        _logger = logger;
        _mailboxService = mailboxService;
    }

    [HttpPost("increment/{value}")]
    public async Task<int?> SendRpcIncrementRequest(int value, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending rpc request...");
        using var c = _mailboxService.RpcClient("inc-rpc-server");
        var response = await c.CallAsync<RpcIncrementResponse>(new RpcIncrementRequest(value), cancellationToken);
        return response?.NewValue;
    }
}