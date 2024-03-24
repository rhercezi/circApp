using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using User.Command.Application.Commands;
using User.Command.Application.Exceptions;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Exceptions;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        private readonly IdLinkRepository _idLinkRepo;
        private readonly ILogger<UpdatePasswordCommandHandler> _logger;

        public UpdatePasswordCommandHandler(EventStore eventStore,
                                            PasswordHashService passwordHashService,
                                            IdLinkRepository idLinkRepo,
                                            ILogger<UpdatePasswordCommandHandler> logger)
        {
            _eventStore = eventStore;
            _passwordHashService = passwordHashService;
            _idLinkRepo = idLinkRepo;
            _logger = logger;
        }

        public async Task HandleAsync(UpdatePasswordCommand command)
        {
            if (!string.IsNullOrEmpty(command.IdLink))
            {
                try
                {
                    var idLinkModel = _idLinkRepo.GetByIdAsync(command.IdLink).Result.First();
                    command.Id = Guid.Parse(idLinkModel.UserId);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    throw new UserValidationException("Reset link has expired.");
                }
            }

            var events = await _eventStore.GetEventsAsync(command.Id);
            if (events.Count == 0) throw new UserDomainException("No users found with the given ID.");
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            UpdatePasswordCommandValidator validator = new(_eventStore, _passwordHashService, events);

            await validator.ValidateCommand(command);

            userAggregate.InvokAction<PasswordUpdatedEvent>(
                new PasswordUpdatedEvent
                (
                    command.Id,
                    version,
                    _passwordHashService.HashPassword(command.Password, command.Id),
                    command.Updated
                )
            );

            if (!string.IsNullOrEmpty(command.IdLink))
            {
                await _idLinkRepo.DeleteAsync(command.IdLink);
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((UpdatePasswordCommand)command);
        }
    }
}