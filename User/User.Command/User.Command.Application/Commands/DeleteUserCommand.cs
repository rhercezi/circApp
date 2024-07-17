using Core.Messages;

namespace User.Command.Application.Commands
{
    public class DeleteUserCommand : BaseCommand
    {
        public string Password { get; set; }
        public DeleteUserCommand(Guid id, string password)
        {
            Id = id;
            Password = password;
        }
    }
}