using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly ILogger<CreateAppointmentCommandHandler> _logger;
        public CreateAppointmentCommandHandler(AppointmentRepository appointmentRepository,
                                               AppointmentDetailsRepository detailsRepository,
                                               ILogger<CreateAppointmentCommandHandler> logger,
                                               CAMapRepository mapRepository)
        {
            _appointmentRepository = appointmentRepository;
            _detailsRepository = detailsRepository;
            _logger = logger;
            _mapRepository = mapRepository;
        }

        public async Task HandleAsync(CreateAppointmentCommand command)
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
                        CircleId = c
                    }
                ).ToList();

                await _mapRepository.SaveManyAsync(mappings);

                await session.CommitTransactionAsync();

            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError($"{e.Message}\n{command}\n{e.StackTrace}");
                throw;
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((CreateAppointmentCommand)command);
        }
    }
}