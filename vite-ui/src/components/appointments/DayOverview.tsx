import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { useEffect, useRef, useState } from "react";
import { AppointmentDto } from "../../api/dtos/appointment_dtos/AppointmentDto";
import { FormControl, IconButton, InputLabel, MenuItem, Select } from "@mui/material";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPencilSquare } from "@fortawesome/free-solid-svg-icons/faPencilSquare";
import { faTrash } from "@fortawesome/free-solid-svg-icons/faTrash";
import { useNavigate } from "react-router-dom";
import { getTimeSpan } from "../../helpers/helpers";

const DayOverview = () => {
    const { appointmentStore, circleStore, userStore } = useStore();
    const [appointments, setAppointments] = useState<AppointmentDto[] | undefined>([]);
    const [selectedCircle, setSelectedCircle] = useState<string | undefined>(undefined);
    const navigate = useNavigate();
    const [circles, setCircles] = useState<CircleDto[]>([...circleStore.circlesMap.values()]);
    const userId = userStore.user?.id;
    let startOfDay = new Date(localStorage.getItem('selectedDate')!)
    let endOfDay = new Date(localStorage.getItem('selectedDate')!)
    startOfDay.setHours(0, 0, 0, 0);
    endOfDay.setHours(23, 59, 59, 999);

    useEffect(() => {
        if (selectedCircle) {
            appointmentStore.getAppointments(selectedCircle, undefined, startOfDay, endOfDay);
        } else {
            appointmentStore.getAppointments(undefined, userId, startOfDay, endOfDay);
        }
    }, [selectedCircle]);

    useEffect(() => {
        setCircles([...circleStore.circlesMap.values()]);
    }, [circleStore.circlesMap]);

    useEffect(() => {
        setAppointments(appointmentStore.appointments);
        if (itemsContainerRef.current) {
            const items = itemsContainerRef.current.querySelectorAll('.day-overview-item');
            let maxWidth = 0;

            items.forEach(item => {
                const itemWidth = item.getBoundingClientRect().width;
                const itmHeight = item.getBoundingClientRect().height;
                if (itemWidth > maxWidth) {
                    maxWidth = itemWidth;
                }
                const lineDiv = item.querySelector('.line-div') as HTMLElement;
                lineDiv.style.height = `${itmHeight}px`;
            });

            items.forEach(item => {
                (item as HTMLElement).style.width = `${maxWidth}px`;
            });
        }
    }, [appointmentStore.appointments]);

    const itemsContainerRef = useRef<HTMLDivElement>(null);

    const editAppointment = (appointmentId: string) => {
        navigate(`/dashboard/appointment/${appointmentId}`);
    };

    const deleteAppointment = (appointmentId: string) => {
        appointmentStore.deleteAppointment(appointmentId, userId!);
    };

    return (
        <>
            <div className="day-overview-header">
                <FormControl variant="outlined" sx={{ minWidth: 200 }} >
                    <InputLabel id="circle-select">Select Circle</InputLabel>
                    <Select
                        id="circle-select"
                        labelId="circle-select"
                        label="Select Circle"
                        value={selectedCircle}
                        onChange={(e) => {
                            setSelectedCircle(e.target.value);
                        }}
                    >
                        <MenuItem selected={true} key={userId} value={undefined}>All Circles</MenuItem>
                        {
                            circles.map(circle => (
                                <MenuItem key={circle.id} value={circle.id}>{circle.name}</MenuItem>
                            ))
                        }
                    </Select>
                </FormControl>
            </div>
            <div className="day-overview-items-container" ref={itemsContainerRef}>
                {appointments && appointments.map(appointment => (
                    <div key={appointment.id} className="day-overview-item">
                        <div className="vertical-center">
                            <div className="line-div"></div>
                            <div className="day-overview-item-inner">

                                <div className="time-position">
                                    <div className="time-circle">
                                        {new Date(appointment.startDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                    </div>
                                </div>

                                <div className="day-overview-item-data">
                                    <div><strong>{appointment.title}</strong></div>
                                    <div><strong>Duration: </strong> {getTimeSpan(new Date(appointment.startDate), new Date(appointment.endDate))}</div>
                                    {appointment.details && appointment.detailsInCircles &&
                                        (selectedCircle === undefined
                                            ? appointment.detailsInCircles.some(circleId => circles.map(circle => circle.id).includes(circleId))
                                            : appointment.detailsInCircles.includes(selectedCircle)) && (
                                            <>
                                                <div><strong>Note: </strong> {appointment.details.note}</div>
                                                {appointment.details && appointment.details.address && (
                                                    <div><strong>Address: </strong>
                                                        {appointment.details.address.street ? appointment.details.address.street : ''}
                                                        {appointment.details.address.housenumber ? ' ' + appointment.details.address.housenumber + ',' : ''}
                                                        {appointment.details.address.postCode ? ' ' + appointment.details.address.postCode + ',' : ''}
                                                        {appointment.details.address.city ? ' ' + appointment.details.address.city + ',' : ''}
                                                        {appointment.details.address.country ? ' ' + appointment.details.address.country : ''}
                                                    </div>
                                                )}
                                                {appointment.details && appointment.details.reminders && (
                                                    <div className="flex-column-left"><strong>Reminders: </strong>
                                                        {appointment.details.reminders.map(reminder => (
                                                            <div className="flex-column-left" 
                                                                key={reminder && reminder.time.toLocaleString()}>
                                                                <div>{reminder && reminder.message}</div>
                                                                <div>{reminder && reminder.time.toLocaleString()}</div>
                                                            </div>
                                                        ))}
                                                    </div>
                                                )}
                                            </>
                                        )}
                                </div>
                            </div>
                            {appointment.creatorId === userId && (
                                <div className="day-overview-item-edit-div">
                                    <IconButton onClick={() => { editAppointment(appointment.id) }}>
                                        <FontAwesomeIcon icon={faPencilSquare} size="1x" className="day-overview-item-edit-button" />
                                    </IconButton>
                                    <IconButton onClick={() => { deleteAppointment(appointment.id) }}>
                                        <FontAwesomeIcon icon={faTrash} size="1x" className="day-overview-item-edit-button" />
                                    </IconButton>
                                </div>
                            )}
                        </div>
                    </div>
                ))}

            </div>
        </>
    );
}

export default observer(DayOverview);