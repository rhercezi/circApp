using Core.Configs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Core.Repositories;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Events;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdLinkRepository _idLinkRepo;
        private MailConfig _config;
        private readonly EventProducer _eventProducer;

        public CreateUserCommandHandler(EventStore eventStore,
                                        PasswordHashService passwordHashService,
                                        IServiceProvider serviceProvider,
                                        IdLinkRepository idLinkRepo,
                                        IOptions<MailConfig> config,
                                        EventProducer eventProducer)
        {
            _eventStore = eventStore;
            _passwordHashService = passwordHashService;
            _serviceProvider = serviceProvider;
            _idLinkRepo = idLinkRepo;
            _config = config.Value;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(CreateUserCommand command)
        {

            CreateUserCommandValidator validator = new(_eventStore);

            await validator.ValidateCommand(command);

            var hash = _passwordHashService.HashPassword(command.Password, command.Id);
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.InvokAction<UserCreatedEvent>(
                new UserCreatedEvent
                (
                    command.Id,
                    0,
                    command.UserName,
                    hash,
                    command.FirstName,
                    command.FamilyName,
                    command.Email,
                    false,
                    command.Created
                )
            );

            await _eventProducer.ProduceAsync(
                new UserCreatedPublicEvent
                (
                    command.Id,
                    command.UserName,
                    command.FirstName,
                    command.FamilyName,
                    command.Email
                ),
                "user_public"
            );

            var idLink = IdLinkConverter.GenerateRandomString();

            await _idLinkRepo.SaveAsync(new IdLinkModel
            {
                LinkId = idLink,
                UserId = command.Id.ToString(),
                UserName = command.UserName,
                Email = command.Email
            });

            using (var scope = _serviceProvider.CreateScope())
            {
                var config = new MailConfig(_config);
                config.Body[0] = config.Body[0].Replace("[VerificationLink]", idLink);
                config.Body[0] = config.Body[0].Replace("[User]", command.FirstName + " " + command.FamilyName);

                var emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                emailSenderService.SendMail(idLink, command.Email, config, 0);
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((CreateUserCommand)command);
        }
    }
}