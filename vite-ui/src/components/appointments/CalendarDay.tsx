import React, { useState } from 'react';
import { IconButton } from "@mui/material";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlusCircle } from "@fortawesome/free-solid-svg-icons";
import { AppointmentDto } from "../../api/dtos/appointment_dtos/AppointmentDto";

interface Props {
    currentDate: Date;
    date: Date;
    isCurrentMonth: boolean;
    appointments: AppointmentDto[];
    createAppointment: (event: React.MouseEvent<HTMLButtonElement>, date: Date) => void;
    handleDayRedirect: (date: Date) => void;
}

const isToday = (startDate: Date | string, endDate: Date | string, currentDate: Date): boolean => {

    const start = typeof startDate === 'string' ? new Date(startDate) : startDate;
    const end = typeof endDate === 'string' ? new Date(endDate) : endDate;

    if (isNaN(start.getTime()) || isNaN(end.getTime())) {
        return false;
    }

    const isStartBeforeOrOnCurrent =
        start.getFullYear() < currentDate.getFullYear() ||
        (start.getFullYear() === currentDate.getFullYear() && start.getMonth() < currentDate.getMonth()) ||
        (start.getFullYear() === currentDate.getFullYear() && start.getMonth() === currentDate.getMonth() && start.getDate() <= currentDate.getDate());

    const isEndAfterOrOnCurrent =
        end.getFullYear() > currentDate.getFullYear() ||
        (end.getFullYear() === currentDate.getFullYear() && end.getMonth() > currentDate.getMonth()) ||
        (end.getFullYear() === currentDate.getFullYear() && end.getMonth() === currentDate.getMonth() && end.getDate() >= currentDate.getDate());

    return isStartBeforeOrOnCurrent && isEndAfterOrOnCurrent;
};

const isPastDate = (date: Date): boolean => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date < today;
};

const CalendarDay = ({ currentDate, date, isCurrentMonth, appointments, createAppointment, handleDayRedirect }: Props) => {

    const [screenWidth, setScreenWidth] = useState(window.innerWidth >= 1024);

    window.addEventListener('resize', () => {
        setScreenWidth(window.innerWidth >= 1024);
    });

    const hasAppointments = appointments && appointments
        .filter((appointment) =>
            isToday(
                appointment.startDate,
                appointment.endDate,
                currentDate
            )
        ).length > 0;

    const pastDate = isPastDate(currentDate);

    return (
        <div key={currentDate.toISOString()} className={`${isCurrentMonth ?
            (currentDate.getDate() === date.getDate())
                && (currentDate.getFullYear() === date.getFullYear())
                && (currentDate.getMonth() === date.getMonth())
                ? 'month-overview-day month-overview-day-today'
                : 'month-overview-day'
            : 'other-month-day'} ${pastDate ? 'past-date' : ''}`}
            onClick={() => { handleDayRedirect(currentDate) }}>
            <div className="month-overview-day-date-row">
                {screenWidth ? `${currentDate.getDate()}.${currentDate.getMonth() + 1}.${currentDate.getFullYear()}`
                    : `${currentDate.getDate()}`}

                <IconButton onClick={(e) => { createAppointment(e, currentDate) }} >
                    <FontAwesomeIcon icon={faPlusCircle} size="xs" color="#1976D2" />
                </IconButton>
            </div>
            <div className={`month-overview-day-appointments${hasAppointments ? ' has-appointments' : ''}`}>
                {appointments ? appointments
                    .filter((appointment) =>
                        isToday(
                            appointment.startDate,
                            appointment.endDate,
                            currentDate
                        )
                    ).length : '0'}{screenWidth ? ' appointments' : ''}{screenWidth && currentDate.getMonth() === date.getMonth() &&
                        currentDate.getDate() === date.getDate()
                        ? ' today'
                        : ''}
            </div>
        </div>
    );
};

export default CalendarDay;