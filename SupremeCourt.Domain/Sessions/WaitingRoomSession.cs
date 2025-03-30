using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Logic;

namespace SupremeCourt.Domain.Sessions
{
    public class WaitingRoomSession : IDisposable
    {
        public int WaitingRoomId { get; set; }
        public List<Player> Players { get; private set; } = new();
        public DateTime CreatedAt { get; set; }
        public int CreatedByPlayerId { get; set; }

        private Timer? _timer;
        private int _timeLeftSeconds;

        public event Action<int>? OnCountdownTick;
        public event Action<int>? OnRoomExpired;

        // ⚠️ parameterless constructor pro Mapperly
        public WaitingRoomSession()
        {
        }

        // ➕ volitelný inicializátor z entity
        public void InitializeFromEntity(WaitingRoom entity)
        {
            WaitingRoomId = entity.Id;
            CreatedAt = entity.CreatedAt;
            CreatedByPlayerId = entity.CreatedByPlayerId;
            Players = entity.Players;

            _timeLeftSeconds = 60;
            _timer = new Timer(Tick, null, 1000, 1000);
        }

        private void Tick(object? state)
        {
            _timeLeftSeconds--;
            OnCountdownTick?.Invoke(_timeLeftSeconds);
            if (_timeLeftSeconds <= 0)
            {
                _timer?.Dispose();
                OnRoomExpired?.Invoke(WaitingRoomId);
            }
        }

        public int GetTimeLeft() => _timeLeftSeconds;

        public void AddPlayer(Player player)
        {
            if (!Players.Any(p => p.Id == player.Id))
                Players.Add(player);
        }

        public bool IsFull => Players.Count >= GameRules.MaxPlayers;

        public void Dispose()
        {
           
        }
    }
}