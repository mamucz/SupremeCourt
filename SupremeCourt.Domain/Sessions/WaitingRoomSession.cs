using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Sessions
{
    public sealed class WaitingRoomSession : IDisposable
    {
        public Guid WaitingRoomId { get; }
        public DateTime CreatedAt { get; }
        public IPlayer CreatedBy { get; }

        // list hráčů chráněný lockem
        private readonly List<IPlayer> _players = new();
        private readonly object _playersLock = new();
        public IReadOnlyList<IPlayer> Players
        {
            get { lock (_playersLock) return _players.ToImmutableArray(); }
        }

        public bool IsFull { get { lock (_playersLock) return _players.Count >= 5; } }

        private readonly PeriodicTimer _timer;
        private readonly Func<Guid, Task> _expiredCallback;
        private readonly CancellationTokenSource _cts = new();
        private int _timeLeftSeconds;

        public event Func<Guid, int, Task>? OnCountdownTick;
        public event Func<Guid, Task>? OnRoomExpired;

        public WaitingRoomSession(IPlayer createdBy, Func<Guid, Task> expiredCallback, int timeLeftSeconds)
        {
            WaitingRoomId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;

            lock (_playersLock) _players.Add(createdBy);

            _expiredCallback = expiredCallback;
            _timeLeftSeconds = timeLeftSeconds > 0 ? timeLeftSeconds : 60;

            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            // spustíme asynchronní smyčku
            _ = RunTimerLoopAsync(_cts.Token);
        }

        private async Task RunTimerLoopAsync(CancellationToken ct)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(ct))
                {
                    var left = Interlocked.Decrement(ref _timeLeftSeconds);

                    // tick event (bezpečně a paralelně)
                    await RaiseCountdownTickAsync(left);

                    if (left <= 0)
                    {
                        await RaiseRoomExpiredAsync();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignorujeme při Dispose
            }
            catch (Exception)
            {
                // TODO: zalogovat
            }
        }

        private async Task RaiseCountdownTickAsync(int secondsLeft)
        {
            var handlers = OnCountdownTick?.GetInvocationList()
                .Cast<Func<Guid, int, Task>>()
                .ToArray();

            if (handlers is null || handlers.Length == 0) return;

            await Task.WhenAll(handlers.Select(h => SafeInvoke(() => h(WaitingRoomId, secondsLeft))));
        }

        private async Task RaiseRoomExpiredAsync()
        {
            var handlers = OnRoomExpired?.GetInvocationList()
                .Cast<Func<Guid, Task>>()
                .ToArray();

            if (handlers is not null && handlers.Length > 0)
            {
                await Task.WhenAll(handlers.Select(h => SafeInvoke(() => h(WaitingRoomId))));
            }

            // callback z konstruktoru (např. aby manager roomku odstranil)
            await SafeInvoke(() => _expiredCallback(WaitingRoomId));
        }

        private static async Task SafeInvoke(Func<Task> action)
        {
            try { await action(); }
            catch (Exception)
            {
                // TODO: zalogovat
            }
        }

        public int GetTimeLeft() => Volatile.Read(ref _timeLeftSeconds);

        public bool TryAddPlayer(IPlayer player)
        {
            lock (_playersLock)
            {
                if (_players.Any(p => (p.Id == player.Id && player.IsAi==false))) return false;
                if (_players.Count >= 5) return false;

                _players.Add(player);
                return true;
            }
        }

        public bool TryRemovePlayer(Guid playerId)
        {
            lock (_playersLock)
            {
                var idx = _players.FindIndex(p => p.Id == playerId);
                if (idx >= 0)
                {
                    _players.RemoveAt(idx);
                    return true;
                }
                return false;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _timer.Dispose();
            _cts.Dispose();
        }
    }
}
