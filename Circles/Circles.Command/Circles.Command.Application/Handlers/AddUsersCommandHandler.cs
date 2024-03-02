using Circles.Command.Application.Commands;
using Circles.Command.Application.EventProducer;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class AddUsersCommandHandler : ICommandHandler<AddUsersCommand>
    {
        private readonly JoinCircleEventProducer _eventProducer;
        private readonly JoinRequestRepository _requestRepository;

        public AddUsersCommandHandler(JoinCircleEventProducer eventProducer, JoinRequestRepository requestRepository)
        {
            _eventProducer = eventProducer;
            _requestRepository = requestRepository;
        }

        public async Task HandleAsync(AddUsersCommand command)
        {

            var insertTask = command.Users.Select(
                uId => Task.Run(() => _requestRepository.SaveAsync(
                        new JoinRequestModel
                        {
                            UserId = uId,
                            CircleId = command.CircleId,
                            InviterId = command.InviterId
                        }
                    )
                )
            );

            await Task.WhenAll(insertTask);

            command.Users.ForEach(
                uId => Task.Run(() => _eventProducer.ProduceAsync(
                        new JoinCircleRequestPublicEvent
                        (
                            command.CircleId,
                            uId,
                            command.InviterId
                        )
                    )
                )
            );
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((AddUsersCommand)command);
        }
    }
}