using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly UserCircleRepository _userCircleRepository;
        private readonly ILogger<UserDeletedEventHandler> _logger;

        public UserDeletedEventHandler(UserRepository userRepository,
                                       UserCircleRepository userCircleRepository,
                                       ILogger<UserDeletedEventHandler> logger)
        {
            _userRepository = userRepository;
            _userCircleRepository = userCircleRepository;
            _logger = logger;
        }

        public async Task HandleAsync(UserDeletedPublicEvent xEvent)
        {
            using var session = await _userRepository.GetSession();
            try
            {
                session.StartTransaction();
                await _userRepository.DeleteUser(xEvent.Id);
                await _userCircleRepository.DeleteByUser(xEvent.Id);
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((UserDeletedPublicEvent)xEvent);
        }
    }
}