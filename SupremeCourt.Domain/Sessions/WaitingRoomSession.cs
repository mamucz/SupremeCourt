using SupremeCourt.Domain.Interfaces;

public class WaitingRoomSession : IDisposable
{
    public Guid WaitingRoomId { get; }
    public DateTime CreatedAt { get; }
    public IPlayer CreatedBy { get; }
    public List<IPlayer> Players { get; } = new();

    public bool IsFull => Players.Count > 4;

    private readonly Timer _timer;
    private readonly Action<Guid> _onExpired;
    private int _timeLeftSeconds = 60; // např. 60 sekund default

    public WaitingRoomSession(IPlayer createdBy, Action<Guid> onExpired)
    {
        WaitingRoomId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Players.Add(createdBy);
        _onExpired = onExpired;

        _timer = new Timer(Tick, null, 1000, 1000);
    }

    private void Tick(object? state)
    {
        _timeLeftSeconds--;
        if (_timeLeftSeconds <= 0)
        {
            _timer.Dispose();
            _onExpired.Invoke(WaitingRoomId);
        }
    }

    public int GetTimeLeft() => _timeLeftSeconds;

    public bool TryAddPlayer(IPlayer player)
    {
        if (Players.Any(p => p.Id == player.Id))
            return false;
        Players.Add(player);
        return true;
    }

    public bool TryRemovePlayer(int playerId)
    {
        var player = Players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            Players.Remove(player);
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}