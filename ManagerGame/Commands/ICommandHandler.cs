namespace ManagerGame.Commands;

public interface ICommandHandler<in TCommand> 
    where TCommand : ICommand
{
    public Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
    public Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}