using Core.MessageHandling;
using Core.Messages;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Application.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Exceptions;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        private readonly IMongoRepository<IdLinkModel> _idLinkRepo;

        public UpdatePasswordCommandHandler(EventStore eventStore, PasswordHashService passwordHashService, IMongoRepository<IdLinkModel> idLinkRepo)
        {
            _eventStore = eventStore;
            _passwordHashService = passwordHashService;
            _idLinkRepo = idLinkRepo;
        }

        public async Task HandleAsync(UpdatePasswordCommand command)
        {
            if (!string.IsNullOrEmpty(command.IdLink))
            {
                var idLinkModel = _idLinkRepo.GetByIdAsync(command.IdLink).Result.First();
                command.Id = Guid.Parse(idLinkModel.UserId);
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

        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((UpdatePasswordCommand)command);
        }
    }
}