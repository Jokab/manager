namespace ManagerGame.Core;

public interface ICommandHandler<in TCommand, TResult>
{
    public Task<Result<TResult>> Handle(TCommand command,
        CancellationToken cancellationToken = default);
}
