using Circles.Command.Application.Commands;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class UpdateCircleCommandHandler : IMessageHandler<UpdateCircleCommand>
    {
        private readonly CirclesRepository _circlesRepository;
        private readonly ILogger<UpdateCircleCommandHandler> _logger;

        public UpdateCircleCommandHandler(CirclesRepository circlesRepository,
                                          ILogger<UpdateCircleCommandHandler> logger)
        {
            _circlesRepository = circlesRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(UpdateCircleCommand command)
        {
            var circle = await _circlesRepository.GetByIdAsync(command.CircleId);
            if (circle == null)
            {
                return new BaseResponse { ResponseCode = 404, Message = $"Circle with id {command.CircleId} not found." };
            }

            try
            {
                command.JsonPatchDocument.ApplyTo(circle);
                await _circlesRepository.UpdateCirce(circle);

                return new BaseResponse { ResponseCode = 200, Data = circle };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((UpdateCircleCommand)command);
        }
    }
}