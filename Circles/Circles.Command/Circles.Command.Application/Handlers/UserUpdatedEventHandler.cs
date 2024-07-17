using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class UserUpdatedEventHandler : IMessageHandler<UserUpdatedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserUpdatedEventHandler> _logger;
        public UserUpdatedEventHandler(UserRepository userRepository,
                                       ILogger<UserUpdatedEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(UserUpdatedPublicEvent xEvent)
        {
            try
            {
                await _userRepository.UpdateUser(
                    new AppUserModel
                    {
                        UserId = xEvent.Id,
                        UserName = xEvent.UserName,
                        FirstName = xEvent.FirstName,
                        FamilyName = xEvent.FamilyName,
                        Email = xEvent.Email
                    }
                );

                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((UserUpdatedPublicEvent)xEvent);
        }
    }
}