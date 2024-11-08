using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Command.Application.EventProducer;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class CreateAppointmentCommandHandler : IMessageHandler<CreateAppointmentCommand>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly ILogger<CreateAppointmentCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        public CreateAppointmentCommandHandler(AppointmentRepository appointmentRepository,
                                               AppointmentDetailsRepository detailsRepository,
                                               ILogger<CreateAppointmentCommandHandler> logger,
                                               CAMapRepository mapRepository,
                                               AppointmentEventProducer eventProducer)
        {
            _appointmentRepository = appointmentRepository;
            _detailsRepository = detailsRepository;
            _logger = logger;
            _mapRepository = mapRepository;
            _eventProducer = eventProducer;
        }

        public async Task<BaseResponse> HandleAsync(CreateAppointmentCommand command)
        {
            using var session = await _appointmentRepository.GetSession();
            try
            {
                session.StartTransaction();

                var appointment = DtoConverter.Convert(command);
                if (command.Details != null)
                {
                    command.Details.AppointmentId = appointment.Id;
                    var details = DtoConverter.Convert(command.Details);
                    await _detailsRepository.SaveAsync(details);
                }
                await _appointmentRepository.SaveAsync(appointment);

                var mappings = command.Circles.Select(
                    c => new CircleAppointmentMap
                    {
                        AppointmentId = appointment.Id,
                        CircleId = c,
                        Date = appointment.StartDate
                    }
                ).ToList();

                await _mapRepository.SaveManyAsync(mappings);

                await _eventProducer.ProduceAsync(
                    new AppointmentChangePublicEvent(
                        appointment.Id,
                        appointment.Title,
                        EventType.Create,
                        command.CreatorId,
                        command.StartDate,
                        command.Circles
                    )
                );
                await session.CommitTransactionAsync();

                return new BaseResponse { ResponseCode = 200, Data = appointment };
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}\n{command}", e.Message, e.StackTrace, command);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((CreateAppointmentCommand)command);
        }
    }
}