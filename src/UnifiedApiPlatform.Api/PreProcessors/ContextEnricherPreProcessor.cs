using FastEndpoints;
using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Api.PreProcessors;

public class ContextEnricherPreProcessor<TRequest> : IPreProcessor<TRequest>
{

    public Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
    {
        if (ctx.Request is CommandBase command)
        {
            command.IpAddress = ctx.HttpContext.Connection.RemoteIpAddress?.ToString();
            command.UserAgent = ctx.HttpContext.Request.Headers.UserAgent.FirstOrDefault();
            command.TraceId = ctx.HttpContext.TraceIdentifier;
        }

        return Task.CompletedTask;
    }
}
