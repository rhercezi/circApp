using Core.MessageHandling;
using Core.Messages;
using Core.Repositories;
using User.Command.Application.Commands;
using User.Command.Application.Exceptions;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {
        private readonly EventStore _eventStore;
        private readonly IdLinkRepository _idLinkRepo;

        public VerifyEmailCommandHandler(EventStore eventStore, IdLinkRepository idLinkRepo)
        {
            _eventStore = eventStore;
            _idLinkRepo = idLinkRepo;
        }

        public async Task HandleAsync(VerifyEmailCommand command)
        {

            var exception = new UserValidationException("Invalid email confirmation link");
            var idLinkModel = _idLinkRepo.GetByIdAsync(command.idLink).Result.First();
            
            if (idLinkModel == null)
            {
                throw exception;
            }

            command.Id = Guid.Parse(idLinkModel.UserId);
            var events = await _eventStore.GetEventsAsync(command.Id);

            if (events.Count == 0)
            {
                throw exception;
            }

            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            userAggregate.InvokAction<EmailVerifiedEvent>(
                new EmailVerifiedEvent(
                    command.Id,
                    version
                )
            );

            await _idLinkRepo.DeleteAsync(command.idLink);

        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((VerifyEmailCommand)command);
        }
    }
}