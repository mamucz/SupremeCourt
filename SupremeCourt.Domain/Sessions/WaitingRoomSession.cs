using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain.Sessions
{


    public class WaitingRoomSession : IDisposable
    {
        public Guid WaitingRoomId { get; }
        public DateTime CreatedAt { get; }
        public IPlayer CreatedBy { get; }
        public List<IPlayer> Players { get; } = new();

        public bool IsFull => Players.Count >= 5;

        private readonly Timer _timer;
        private readonly Action<Guid> _onExpired;
        private int _timeLeftSeconds = 60;

        public event Func<Guid, int, Task>? OnCountdownTick;
        public event Func<Guid, Task>? OnRoomExpired;

        public WaitingRoomSession(IPlayer createdBy, Action<Guid> onExpired, int timeLeftSeconds)
        {
            WaitingRoomId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
            Players.Add(createdBy);
            _onExpired = onExpired;
            _timeLeftSeconds = timeLeftSeconds;
            _timer = new Timer(Tick, null, 1000, 1000);
        }

        private async void Tick(object? state)
        {
            _timeLeftSeconds--;

            // 🔁 Vyvolat událost o ticku
            if (OnCountdownTick != null)
                await OnCountdownTick.Invoke(WaitingRoomId, _timeLeftSeconds);

            if (_timeLeftSeconds <= 0)
            {
                _timer.Dispose();

                // 🧨 Zavolat událost o expiraci
                if (OnRoomExpired != null)
                    await OnRoomExpired.Invoke(WaitingRoomId);

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
}