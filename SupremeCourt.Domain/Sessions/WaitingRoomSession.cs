using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;

namespace SupremeCourt.Domain.Sessions
{
    public class WaitingRoomSession : IDisposable
    {
        public int WaitingRoomId { get; set; }
        public List<IPlayer> Players { get; private set; } = new();
        public DateTime CreatedAt { get; set; }
        public IPlayer CreatedBy{ get; set; }

        private Timer? _timer;
        private int _timeLeftSeconds;

        public event Action<int, int>? OnCountdownTick;
        public event Action<int>? OnRoomExpired;

        public WaitingRoomSession()
        {
        }

        public WaitingRoomSession(int roomId, IPlayer createdBy, IWaitingRoomEventHandler eventHandler)
        {
            WaitingRoomId = roomId;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
            Players.Add(createdBy);

            _timeLeftSeconds = 3 * 60; // Např. 3 minuty
            _timer = new Timer(Tick, null, 1000, 1000);

            OnCountdownTick += async (id, seconds) => await eventHandler.HandleCountdownTickAsync(id, seconds);
            OnRoomExpired += async id => await eventHandler.HandleRoomExpiredAsync(id);
        }

        public bool TryAddPlayer(IPlayer player)
        {
            if (Players.Any(p => p.Id == player.Id))
                return false;

            Players.Add(player);
            return true;
        }

        private void Tick(object? state)
        {
            _timeLeftSeconds--;

            OnCountdownTick?.Invoke(WaitingRoomId, _timeLeftSeconds);

            if (_timeLeftSeconds <= 0)
            {
                _timer?.Dispose();
                OnRoomExpired?.Invoke(WaitingRoomId);
            }
        }

        public int GetTimeLeft() => _timeLeftSeconds;

        public bool IsFull => Players.Count >= GameRules.MaxPlayers;
        public bool TryRemovePlayer(int playerId)
        {
            var player = Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                return false;

            Players.Remove(player);
            return true;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}