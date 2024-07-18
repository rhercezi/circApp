using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using User.Command.Application.Commands;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class VerifyEmailCommandHandler : IMessageHandler<VerifyEmailCommand>
    {
        private readonly EventStore _eventStore;
        private readonly IdLinkRepository _idLinkRepo;

        public VerifyEmailCommandHandler(EventStore eventStore, IdLinkRepository idLinkRepo)
        {
            _eventStore = eventStore;
            _idLinkRepo = idLinkRepo;
        }

        public async Task<BaseResponse> HandleAsync(VerifyEmailCommand command)
        {

            var idLinkModel = _idLinkRepo.GetByIdAsync(command.IdLink).Result.First();
            
            if (idLinkModel == null)
            {
                return new BaseResponse{Message="Invalid email verification link", ResponseCode=400};
            }

            command.Id = Guid.Parse(idLinkModel.UserId);
            var events = await _eventStore.GetEventsAsync(command.Id);

            if (events.Count == 0)
            {
                return new BaseResponse{Message="Invalid email verification link", ResponseCode=400};
            }

            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            userAggregate.InvokAction<EmailVerifiedEvent>(
                new EmailVerifiedEvent(
                    command.Id,
                    version
                )
            );

            await _idLinkRepo.DeleteAsync(command.IdLink);

            return new BaseResponse { ResponseCode = 204 };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((VerifyEmailCommand)command);
        }
    }
}