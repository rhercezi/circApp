using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Common.Events;
using User.Common.Utility;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        public UserCreatedEventHandler(IServiceProvider serviceProvider, UserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserCreatedEvent xEvent)
        {
            await _userRepository.CreateUser(xEvent);

            IdLinkConverter converter = new();
            var idLink = converter.GuidToIdLink(xEvent.Id);

            using (var scope = _serviceProvider.CreateScope())
            {
                var emailSender = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                emailSender.SendMail(idLink, xEvent);
        }
            }
    }
}