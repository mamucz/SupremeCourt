﻿namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomNotifier
    {
        Task NotifyPlayerJoinedAsync(int gameId, string playerName);
        Task NotifyWaitingRoomCreatedAsync(object roomDto);
    }
}