using Microsoft.Extensions.DependencyInjection;
using Core.MessageHandling;
using Core.Utilities;
using User.Command.Application.Commands;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Common.Events;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Core.Configs;
using Core.Messages;
using User.Command.Domain.Repositories;
using Core.DTOs;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class ResetPasswordCommandHandler : IMessageHandler<ResetPasswordCommand>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IdLinkRepository _idLinkRepo;
        private readonly EventStore _eventStore;
        private MailConfig _config;

        public ResetPasswordCommandHandler(IServiceProvider serviceProvider,
                                           IdLinkRepository idLinkRepo,
                                           EventStore eventStore,
                                           IOptions<MailConfig> config)
        {
            _serviceProvider = serviceProvider;
            _idLinkRepo = idLinkRepo;
            _eventStore = eventStore;
            _config = config.Value;
        }

        public async Task<BaseResponse> HandleAsync(ResetPasswordCommand command)
        {
            var idLink = IdLinkConverter.GenerateRandomString();
            var userEvents = await _eventStore.GetByUsernameAsync(command.UserName);

            if (userEvents.IsNullOrEmpty()) return new BaseResponse { ResponseCode = 404, Message = "No users found with the given Useranme.", };
            string id = "";
            string userName = "";
            string email = "";
            userEvents.ForEach(e =>
            {

                if (e is UserCreatedEvent event1)
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

            var config = new MailConfig(_config);
            config.Body[1] = config.Body[1].Replace("[ResetLink]", idLink);
            config.Body[1] = config.Body[1].Replace("[User]", command.UserName);
            config.Body[1] = config.Body[1].Replace("[BaseUrl]", config.BaseUrl);
            config.Subject = "CircleApp - reset password";

            using var scope = _serviceProvider.CreateScope();
            var _emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
            _emailSenderService.SendMail(email, config, 1);

            return new BaseResponse { ResponseCode = 204 };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((ResetPasswordCommand)command);
        }
    }
}