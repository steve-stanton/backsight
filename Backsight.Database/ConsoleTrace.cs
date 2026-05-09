namespace Backsight.Database;

using RepoDb;
using RepoDb.Interfaces;

public sealed class ConsoleTrace : ITrace
{
    public void BeforeExecution(CancellableTraceLog log)
    {
        Console.WriteLine(log.Statement);
    }

    public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
    }

    public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = new CancellationToken())
    {
        //return Task.CompletedTask;
        throw new NotImplementedException();
    }

    public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = new CancellationToken())
    {
        //return Task.CompletedTask;
        throw new NotImplementedException();
    }
}