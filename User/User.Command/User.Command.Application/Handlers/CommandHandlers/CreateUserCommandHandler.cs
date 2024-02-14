using Core.Configs;
using Core.MessageHandling;
using Core.Repositories;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
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
        private readonly IMongoRepository<IdLinkModel> _idLinkRepo;
        private MailConfig _config;

        public CreateUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
            _passwordHashService = serviceProvider.GetRequiredService<PasswordHashService>();
            _serviceProvider = serviceProvider;
            _idLinkRepo = serviceProvider.GetRequiredService<IMongoRepository<IdLinkModel>>();
            _config = serviceProvider.GetRequiredService<IOptions<MailConfig>>().Value;
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

            var idLink = IdLinkConverter.GenerateRandomString();

            _config.Body[0] = _config.Body[0].Replace("[VerificationLink]", idLink);
            _config.Body[0] = _config.Body[0].Replace("[User]", command.FirstName + " " + command.FamilyName);

            using (var scope = _serviceProvider.CreateScope())
            {
                var _emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                _emailSenderService.SendMail(idLink, command.Email, _config, 0);
            }
            await _idLinkRepo.SaveAsync(new IdLinkModel
            {
                LinkId = idLink,
                UserId = command.Id.ToString(),
                UserName = command.UserName,
                Email = command.Email
            });
        }
    }
}