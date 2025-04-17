namespace SupremeCourt.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a service for tracking user session activity in memory or other storage.
    /// </summary>
    public interface IUserSessionRepository
    {
        /// <summary>
        /// Checks whether the specified user is currently connected.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if connected, otherwise false.</returns>
        bool IsUserConnected(int userId);

        /// <summary>
        /// Registers the user as connected.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        void AddUser(int userId);

        /// <summary>
        /// Removes the user from the connected list.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        void RemoveUser(int userId);

        /// <summary>
        /// Gets a read-only list of all currently connected user IDs.
        /// </summary>
        /// <returns>A collection of user IDs.</returns>
        IReadOnlyCollection<int> GetAllConnectedUsers();
    }
}
