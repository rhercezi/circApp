using Core.Messages;

namespace User.Command.Api.Commands
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