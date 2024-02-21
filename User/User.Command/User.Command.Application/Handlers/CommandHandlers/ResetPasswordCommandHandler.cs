using Microsoft.Extensions.DependencyInjection;
using Core.MessageHandling;
using Core.Utilities;
using User.Command.Application.Commands;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Command.Domain.Exceptions;
using User.Common.Events;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Core.Configs;
using Core.Repositories;
using Core.Messages;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMongoRepository<IdLinkModel> _idLinkRepo;
        private readonly EventStore _eventStore;
        private MailConfig _config;

        public ResetPasswordCommandHandler(IServiceProvider serviceProvider,
                                           IMongoRepository<IdLinkModel> idLinkRepo,
                                           EventStore eventStore,
                                           IOptions<MailConfig> config)
        {
            _serviceProvider = serviceProvider;
            _idLinkRepo = idLinkRepo;
            _eventStore = eventStore;
            _config = config.Value;
        }

        public async Task HandleAsync(ResetPasswordCommand command)
        {
            var idLink = IdLinkConverter.GenerateRandomString();
            var userEvents = await _eventStore.GetByUsernameAsync(command.UserName);

            if (userEvents.IsNullOrEmpty()) throw new UserDomainException("No users found with the given Useranme.");
            string id = "";
            string userName = "";
            string email = "";
            userEvents.ForEach(e => {

                if(e is UserCreatedEvent event1)
                {
                    id = event1.Id.ToString();
                    userName = event1.UserName;
                    email = event1.Email;
                }
                else if (e is UserEditedEvent event2)
                {
                    id = event2.Id.ToString();
                    userName = event2.UserName;
                    email = event2.Email;
                    
                }

            });

            await _idLinkRepo.SaveAsync(new IdLinkModel
            {
                LinkId = idLink,
                UserId = id,
                UserName = userName,
                Email = email
            });

             _config.Body[1] = _config.Body[1].Replace("[ResetLink]", idLink);
            _config.Body[1] = _config.Body[1].Replace("[User]", command.UserName);
            _config.Subject = "CircleApp - reset password";

            using var scope = _serviceProvider.CreateScope();
            var _emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
            _emailSenderService.SendMail(idLink, email, _config, 1);
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((ResetPasswordCommand)command);
        }
    }
}