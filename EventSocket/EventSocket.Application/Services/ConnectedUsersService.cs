namespace EventSocket.Application.Services
{
    public class ConnectedUsersService : IConnectedUsersService
    {
        private List<Guid> _users = new();

        public event EventHandler<Guid>? UserAdded;

        public void AddConnectedUser(Guid userId)
        {
            _users.Add(userId);
            OnUserAdded(userId);
        }

        public List<Guid> GetConnectedUsers()
        {
            return _users;
        }

        public void RemoveConnectedUser(Guid userId)
        {
            _users.Remove(userId);
        }

        protected virtual void OnUserAdded(Guid userId)
        {
            UserAdded?.Invoke(this, userId);
        }
    }
}