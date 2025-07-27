namespace Game.Shared.Scripts.Network.Receiver;

public sealed class DisposableAction(Action onDispose) : IDisposable
{
    private Action _onDispose = onDispose;

    public void Dispose()
    {
        var action = _onDispose;
        _onDispose = null;
        action?.Invoke();
    }
}
