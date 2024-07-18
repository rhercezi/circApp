using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Utilities;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CreateTaskCommandHandler : IMessageHandler<CreateTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<CreateTaskCommandHandler> _logger;

        public CreateTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer,
                                        ILogger<CreateTaskCommandHandler> logger)
        {
            _repository = repository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(CreateTaskCommand command)
        {
            try
            {
                _logger.LogDebug("Creating task: {Task}", command);
                var model = CommandModelConverter.ConvertToModel<AppTaskModel>(command);
                await _repository.SaveAsync(model);
                var taskEvent = new TaskChangePublicEvent(command.Id, command.GetType().Name)
                {
                    CircleId = command.CircleId,
                    UserIds = command.UserModels?.Select(x => x.Id).ToList()
                };
                await _eventProducer.ProduceAsync(taskEvent);

                return new BaseResponse { ResponseCode = 201, Data = model };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((CreateTaskCommand)command);
        }
    }
}