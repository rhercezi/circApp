using Circles.Command.Application.Commands;
using Circles.Command.Application.EventProducer;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class CreateCircleCommandHandler : ICommandHandler<CreateCircleCommand>
    {
        public CirclesRepository _circlesRepository { get; set; }
        public UserCircleRepository _userCircleRepository { get; set; }
        public JoinRequestRepository _requestRepository { get; set; }
        public JoinCircleEventProducer _eventProducer { get; set; }

        public CreateCircleCommandHandler(CirclesRepository circlesRepository,
                                          UserCircleRepository userCircleRepository,
                                          JoinRequestRepository requestRepository,
                                          JoinCircleEventProducer eventProducer)
        {
            _circlesRepository = circlesRepository;
            _userCircleRepository = userCircleRepository;
            _requestRepository = requestRepository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(CreateCircleCommand command)
        {
            await _circlesRepository.SaveAsync(
                new CircleModel
                {
                    CircleId = command.CircleId,
                    Name = command.Name,
                    Color = command.Color
                }
            );

            await _userCircleRepository.SaveAsync(
                new UserCircleModel
                {
                    CircleId = command.CircleId,
                    UserId = command.CreatorId
                }
            );

            var produceTask = command.Users.Select(
                uId => Task.Run(() => _eventProducer.ProduceAsync(
                        new JoinCircleRequestPublicEvent
                        (
                            command.CircleId,
                            uId,
                            command.CreatorId
                        )
                    )
                )
            );

            await Task.WhenAll(produceTask);
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((CreateCircleCommand)command);
        }
    }
}