using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using User.Command.Application.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Events;
using User.Command.Domain.Exceptions;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class DeleteUserCommandHandler : IMessageHandler<DeleteUserCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        private readonly EventProducer _eventProducer;

        public DeleteUserCommandHandler(EventStore eventStore, PasswordHashService passwordHashService, EventProducer eventProducer)
        {
            _eventStore = eventStore;
            _passwordHashService = passwordHashService;
            _eventProducer = eventProducer;
        }

        public async Task<BaseResponse> HandleAsync(DeleteUserCommand message)
        {
            var events = await _eventStore.GetEventsAsync(message.Id);
            if (events.Count == 0) throw new UserDomainException("No users found with the given ID.");
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            DeleteUserCommandValidator validator = new(_eventStore, events, _passwordHashService);

            await validator.ValidateCommand(message);
            
            userAggregate.InvokAction<UserDeletedEvent>(
                new UserDeletedEvent
                (
                    message.Id,
                    version
                )
            );

            _ = _eventProducer.ProduceAsync(
                new UserDeletedPublicEvent
                (
                    message.Id
                ),
                "user_public"
            );
            return new BaseResponse { ResponseCode = 204 };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return await HandleAsync((DeleteUserCommand)message);
        }
    }
}