using System.Text.Json;
using System.Text.Json.Serialization;
using ManagerGame.Core;

namespace ManagerGame;

public class LoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class
    where TResult : class
{
    private readonly ICommandHandler<TCommand, TResult> _innerHandler;
    private readonly ILogger<LoggingDecorator<TCommand, TResult>> _logger;

    public LoggingDecorator(ICommandHandler<TCommand, TResult> innerHandler, ILogger<LoggingDecorator<TCommand, TResult>> logger)
    {
        _innerHandler = innerHandler;
        _logger = logger;
    }

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReferenceHandler = ReferenceHandler.Preserve };

    public async Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Executing {method} with command {result}",
            typeof(ICommandHandler<TCommand, TResult>),
            JsonSerializer.Serialize(command, _jsonSerializerOptions));

        try
        {
            var result = await _innerHandler.Handle(command, cancellationToken);
            _logger.LogInformation(
                "Successfully executed {method} with result {result}",
                typeof(ICommandHandler<TCommand, TResult>),
                JsonSerializer.Serialize(result.Value, _jsonSerializerOptions));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in {method}.", typeof(ICommandHandler<TCommand, TResult>));
            throw;
        }
    }
}
