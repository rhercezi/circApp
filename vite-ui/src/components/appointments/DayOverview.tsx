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

const DayOverview = () => {
    const { appointmentStore, circleStore, userStore } = useStore();
    const [appointments, setAppointments] = useState<AppointmentDto[] | undefined>([]);
    const [selectedCircle, setSelectedCircle] = useState<string | undefined>(undefined);
    const navigate = useNavigate();
    const circles: CircleDto[] = [...circleStore.circlesMap.values()];
    const userId = userStore.user?.id;
    let startOfDay = new Date(localStorage.getItem('selectedDate')!)
    let endOfDay = new Date(localStorage.getItem('selectedDate')!)
    startOfDay.setHours(0, 0, 0, 0);
    endOfDay.setHours(23, 59, 59, 999);

    const getDuration = (startDate: Date, endDate: Date): string => {
        const diff = endDate.getTime() - startDate.getTime();
        const diffInMinutes = Math.floor(diff / (1000 * 60));
        const diffInHours = Math.floor(diff / (1000 * 60 * 60));
        const diffInDays = Math.floor(diff / (1000 * 60 * 60 * 24));
        const diffInMonths = Math.floor(diff / (1000 * 60 * 60 * 24 * 30));
        const diffInYears = Math.floor(diff / (1000 * 60 * 60 * 24 * 365));

        if (diffInYears > 0) return `${diffInYears} year(s)`;
        if (diffInMonths > 0) return `${diffInMonths} month(s)`;
        if (diffInDays > 0) return `${diffInDays} day(s)`;
        if (diffInHours > 0) return `${diffInHours} hour(s)`;
        return `${diffInMinutes} minute(s)`;
    };

    useEffect(() => {
        if (selectedCircle) {
            appointmentStore.getAppointments(selectedCircle, undefined, startOfDay, endOfDay);
        } else {
            appointmentStore.getAppointments(undefined, userId, startOfDay, endOfDay);
        }
    }, [selectedCircle]);

    useEffect(() => {
        setAppointments(appointmentStore.appointments);
        if (itemsContainerRef.current) {
            const items = itemsContainerRef.current.querySelectorAll('.day-overview-item');
            let maxWidth = 0;

            items.forEach(item => {
                const itemWidth = item.getBoundingClientRect().width;
                if (itemWidth > maxWidth) {
                    maxWidth = itemWidth;
                }
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
                        <div className="line-div"></div>
                        <div className="day-overview-item-inner">
                            <div className="time-position">
                                <div className="time-circle">
                                    {new Date(appointment.startDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                </div>
                            </div>
                            <div className="day-overview-item-data">
                                <div><strong>{appointment.title}</strong></div>
                                <div><strong>Duration: </strong> {getDuration(new Date(appointment.startDate), new Date(appointment.endDate))}</div>
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
                ))}

            </div>
        </>
    );
}

export default observer(DayOverview);