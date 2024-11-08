namespace EventSocket.Application.Services
{
    public interface IConnectedUsersService
    {
        event EventHandler<Guid>? UserAdded;
        void AddConnectedUser(Guid userId);
        void RemoveConnectedUser(Guid userId);
        List<Guid> GetConnectedUsers();
    }
}