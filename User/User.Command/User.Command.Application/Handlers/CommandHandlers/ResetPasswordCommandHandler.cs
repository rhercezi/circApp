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

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMongoRepository<IdLinkModel> _idLinkRepo;
        private readonly EventStore _eventStore;
        private MailConfig _config;

        public ResetPasswordCommandHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _idLinkRepo = serviceProvider.GetRequiredService<IMongoRepository<IdLinkModel>>();
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
            _config = serviceProvider.GetRequiredService<IOptions<MailConfig>>().Value;
        }

        public async Task HandleAsync(ResetPasswordCommand command)
        {
            var idLink = IdLinkConverter.GenerateRandomString();
            var userEvents = await _eventStore.GetByUsernameAsync(command.UserName);

            if (userEvents.IsNullOrEmpty()) throw new UserDomainException("No users found with the given Useranme.");
            UserCreatedEvent userEvent = null;
            userEvents.ForEach(e => {

                if(e is UserCreatedEvent event1)
                {
                    userEvent = event1;
                }
                else if (e is UserEditedEvent event2)
                {
                    if(!string.IsNullOrEmpty(event2.Email))
                    {
                        userEvent.Email = event2.Email;
                    }
                }

            });

            await _idLinkRepo.SaveAsync(new IdLinkModel
            {
                LinkId = idLink,
                UserId = userEvent.Id.ToString(),
                UserName = userEvent.UserName,
                Email = userEvent.Email
            });

             _config.Body[1] = _config.Body[1].Replace("[ResetLink]", idLink);
            _config.Body[1] = _config.Body[1].Replace("[User]", command.UserName);
            _config.Subject = "CircleApp - reset password";

            using var scope = _serviceProvider.CreateScope();
            var _emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
            _emailSenderService.SendMail(idLink, userEvent.Email, _config, 1);
        }
    }
}