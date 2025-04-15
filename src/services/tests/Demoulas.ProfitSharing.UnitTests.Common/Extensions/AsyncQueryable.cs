using System.Collections;
using System.Linq.Expressions;

namespace Demoulas.ProfitSharing.UnitTests.Common.Helpers;
public static class ICollectionExtensions
{
    public static IQueryable<T> AsAsyncQueryable<T>(this ICollection<T> source) =>
        new AsyncQueryable<T>(source.AsQueryable());
}

internal sealed class AsyncQueryable<T> : IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryable<T> Source;

    public AsyncQueryable(IQueryable<T> source)
    {
        Source = source;
    }

    public Type ElementType => typeof(T);

    public Expression Expression => Source.Expression;

    public IQueryProvider Provider => new AsyncQueryProvider<T>(Source.Provider);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new AsyncEnumeratorWrapper<T>(Source.GetEnumerator());
    }

    public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class AsyncQueryProvider<T> : IQueryProvider
{
    private readonly IQueryProvider Source;

    public AsyncQueryProvider(IQueryProvider source)
    {
        Source = source;
    }

    public IQueryable CreateQuery(Expression expression) =>
        Source.CreateQuery(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new AsyncQueryable<TElement>(Source.CreateQuery<TElement>(expression));

#pragma warning disable CS8603 // Possible null reference return.
    public object Execute(Expression expression) => Execute<T>(expression);
#pragma warning restore CS8603 // Possible null reference return.

    public TResult Execute<TResult>(Expression expression) =>
        Source.Execute<TResult>(expression);
}



internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> Source;

    public AsyncEnumeratorWrapper(IEnumerator<T> source)
    {
        Source = source;
    }

    public T Current => Source.Current;

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        try
        {
            return await new ValueTask<bool>(Source.MoveNext());
        }
        catch (NullReferenceException)
        {
            return await new ValueTask<bool>(false);
        }
    }
}
