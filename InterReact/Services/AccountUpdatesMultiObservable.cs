using System.Reactive.Linq;
using Stringification;
namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which, upon subscription, continually emits account update objects for all accounts:
    /// AccountValue, PortfolioValue, AccountUpdateTime and AccountUpdateEnd.
    /// AccountUpdateEnd is emitted after the initial values for each account have been emitted.
    /// Multiple subscribers are supported. The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<object> AccountUpdatesMultiObservable { get; }

    private IObservable<object> CreateAccountUpdatesMultiObservable()
    {
        return Response
            .Where(x => x is AccountUpdateMulti|| x is AccountUpdateMultiEnd) 
            .ToObservableContinuousWithId(
                Request.GetNextId,
                (requestId) => { Request.RequestAccountUpdatesMulti(requestId, "", "", false); },
                Request.CancelAccountUpdatesMulti)
            .CacheSource(GetAccountUpdatesMultiCacheKey);
    }

    private static string GetAccountUpdatesMultiCacheKey(object o)
    {
        return o switch
        {
            AccountUpdateMulti av => $"{av.Account}+{av.Key}:{av.Currency}",
            AccountUpdateMultiEnd pv => "AccountUpdateMultiEnd",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}
