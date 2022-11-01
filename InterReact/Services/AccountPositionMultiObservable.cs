using Stringification;
using System.Reactive.Linq;
namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits PositionMulti objects for select accounts.
    /// All positions are sent initially, and then only updates as positions change. 
    /// PositionMultiEnd is emitted after the initial values for each account have been emitted.
    /// Multiple subscribers are supported. The latest values are cached for replay to new subscribers.
    /// </summary>
    public IObservable<object> AccountPositionMultiObservable { get; }

    private IObservable<object> CreateAccountPositionMultiObservable()
    {
        return Response
            .Where(x => x is AccountPositionMulti || x is AccountPositionMultiEnd)
            .ToObservableContinuousWithId(
                Request.GetNextId,
                (requestId) => { Request.RequestPositionsMulti(requestId, "", ""); },
                Request.CancelPositionsMulti)
            .CacheSource(GetAccountPositionMultiCacheKey);
    }

    private static string GetAccountPositionMultiCacheKey(object o)
    {
        return o switch
        {
            AccountPositionMulti p => $"{p.Account}+{p.Contract.Stringify()}",
            AccountPositionMultiEnd => "AccountPositionMultiEnd",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}
