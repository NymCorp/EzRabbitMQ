using EzRabbitMQ;

public class RpcServerHandler : BackgroundService
{
    private readonly IMailboxService _mailboxService;
    private readonly IServiceScope _serviceScope;
    private IncRpcServer? _rpcServer;
    
    public RpcServerHandler(IMailboxService mailboxService, IServiceProvider serviceProvider)
    {
        _mailboxService = mailboxService;
        _serviceScope = serviceProvider.CreateScope();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rpcServer = _mailboxService.RpcServer<IncRpcServer>("inc-rpc-server", customServiceProvider: _serviceScope.ServiceProvider);
        
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _rpcServer?.Dispose();
        _serviceScope.Dispose();
        
        return Task.CompletedTask;
    }
}