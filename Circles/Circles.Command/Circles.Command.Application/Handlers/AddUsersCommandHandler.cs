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
        private readonly UserCircleRepository _userCircleRepository;

        public AddUsersCommandHandler(JoinCircleEventProducer eventProducer,
                                      JoinRequestRepository requestRepository,
                                      UserCircleRepository userCircleRepository)
        {
            _eventProducer = eventProducer;
            _requestRepository = requestRepository;
            _userCircleRepository = userCircleRepository;
        }

        public async Task HandleAsync(AddUsersCommand command)
        {
            if (!InviterIsInTheCircle(command)) throw new CirclesValidationException("Only member of the circle can invite new members");
            await ClearDuplicatesAndExisting(command);

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

        private async Task ClearDuplicatesAndExisting(AddUsersCommand command)
        {
            command.Users = command.Users.Distinct().ToList();
            var resultRequest = await _requestRepository.FindAsync(jr => jr.CircleId == command.CircleId && command.Users.Contains(jr.UserId));
            resultRequest.ForEach(jrm => command.Users.RemoveAll(gtr => gtr == jrm.UserId));
            
            var resultUC = await _userCircleRepository.FindAsync(uc => uc.CircleId == command.CircleId && command.Users.Contains(uc.UserId));
            resultUC.ForEach(uc => command.Users.RemoveAll(gtr => gtr == uc.UserId));
        }

        private bool InviterIsInTheCircle(AddUsersCommand command)
        {
            var result = _userCircleRepository.FindAsync(uc => uc.CircleId == command.CircleId && uc.UserId == command.InviterId).Result;

            if (result != null && result.Count > 0) return true;
            return false;
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((AddUsersCommand)command);
        }
    }
}