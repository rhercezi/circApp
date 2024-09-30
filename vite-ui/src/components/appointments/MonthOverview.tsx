import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import { AppointmentDto } from "../../api/dtos/appointment_dtos/AppointmentDto";
import { IconButton } from "@mui/material";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleDoubleDown, faAngleDoubleUp, faPlusCircle } from "@fortawesome/free-solid-svg-icons";
import { useNavigate } from "react-router-dom";
import uuid from "react-uuid";

const MonthOverview = () => {
    const { circleStore, userStore, appointmentStore } = useStore();

    const userId = userStore.user?.id;
    const circles: CircleDto[] = [...circleStore.circlesMap.values()];
    const [selectedCircle, setSelectedCircle] = useState<string | undefined>(undefined);
    const [appointments, setAppointments] = useState<AppointmentDto[]>([]);
    const [year, setYear] = useState<number>(new Date().getFullYear());
    const [month, setMonth] = useState<number>(new Date().getMonth());
    const [date] = useState<Date>(new Date());
    const [baseSearchYear, setBaseSearchYear] = useState<number>(new Date().getFullYear());
    const navigate = useNavigate();

    const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
    ];

    const years = Array.from({ length: 10 }, (_, i) => (baseSearchYear + 5) - i);

    // Get the first and last day of the current month
    let firstDayOfMonth = new Date(year, month, 1);
    let lastDayOfMonth = new Date(year, month + 1, 0);

    // Calculate the number of days in the month
    let numberOfDaysInMonth = lastDayOfMonth.getDate();

    // Adjust the first day of the month to start from Monday
    let adjustedFirstDay = (firstDayOfMonth.getDay() + 6) % 7;

    // Calculate the first date in the table
    let firstDateInTable = new Date(firstDayOfMonth);
    firstDateInTable.setDate(firstDateInTable.getDate() - adjustedFirstDay);

    // Calculate the last date in the table
    let lastDateInTable = new Date(lastDayOfMonth);
    lastDateInTable.setDate(lastDateInTable.getDate() + (6 - (lastDayOfMonth.getDay() + 6) % 7));

    // Calculate the number of weeks
    let numberOfWeeks = Math.ceil((adjustedFirstDay + numberOfDaysInMonth) / 7);
    let weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    // Get appointments for the selected circle or all circles
    useEffect(() => {
        if (selectedCircle) {
            appointmentStore.getAppointments(selectedCircle, undefined, firstDateInTable, lastDateInTable);
        } else {
            appointmentStore.getAppointments(undefined, userId, firstDateInTable, lastDateInTable);
        }
    }, [selectedCircle, year, month]);

    // Update the local appointments collection when the store changes
    useEffect(() => {
        setAppointments(appointmentStore.appointments);
    }, [appointmentStore.appointments]);

    const createAppointment = (event: React.MouseEvent<HTMLButtonElement>, date: Date) => {
        event.stopPropagation();
        localStorage.setItem('selectedDate', date.toISOString());
        navigate('/create');
    };

    const handleDayRedirect = (date: Date) => {
        localStorage.setItem('selectedDate', date.toISOString());
        navigate('/dashboard/day');
    };

    //Set the base year for the year select
    function setBaseYear(arg0: number): void {
        setBaseSearchYear(baseSearchYear + arg0);
    }

    const handleMenuClose = (event: React.MouseEvent) => {
        if ((event.target as HTMLElement).getAttribute('value') === '++' ||
            (event.target as HTMLElement).getAttribute('value') === '--') {
            event.stopPropagation();
        }
    };

    return (
        <>
            <div className="month-overview">
                <div className="month-overview-header">
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

                    <FormControl variant="outlined" style={{ minWidth: 200 }}>
                        <InputLabel id="year-select-label">Year</InputLabel>
                        <Select
                            labelId="year-select-label"
                            id="year-select"
                            value={year}
                            onChange={(e) => {
                                if (e.target.value === '++') { setBaseYear(10) }
                                else if (e.target.value === '--') { setBaseYear(-10) }
                                else { setYear(e.target.value as number) }
                            }}
                            label="Year"
                            MenuProps={{
                                onClose: handleMenuClose, // Custom onClose handler
                            }}
                        >
                            <MenuItem key={uuid()} value={'++'}>
                                <FontAwesomeIcon icon={faAngleDoubleUp} size="xs" />
                            </MenuItem>
                            {years.map((_year) => (
                                <MenuItem key={_year} value={_year}>
                                    {_year}
                                </MenuItem>
                            ))}
                            <MenuItem key={uuid()} value={'--'}>
                                <FontAwesomeIcon icon={faAngleDoubleDown} size="xs" />
                            </MenuItem>
                        </Select>
                    </FormControl>

                    <FormControl variant="outlined" style={{ marginRight: 10, minWidth: 200 }}>
                        <InputLabel id="month-select-label">Month</InputLabel>
                        <Select
                            labelId="month-select-label"
                            id="month-select"
                            value={month}
                            onChange={(e) => { setMonth(e.target.value as number) }}
                            label="Month"
                        >
                            {months.map((month, index) => (
                                <MenuItem key={index} value={(index)}>
                                    {month}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div className="month-overview-body">
                    <div className="month-overview-weekdays">
                        {weekDays.map((day, index) => (
                            <div key={index} className="month-overview-weekday">{day}</div>
                        ))}
                    </div>
                    <div className="month-overview-weeks">
                        {[...Array(numberOfWeeks).keys()].map((week, weekIndex) => (
                            <div key={weekIndex} className="month-overview-week" style={{ height: `${100 / numberOfWeeks}%` }}>
                                {[...Array(7).keys()].map((day, dayIndex) => {
                                    let currentDate = new Date(firstDateInTable);
                                    currentDate.setDate(firstDateInTable.getDate() + week * 7 + day);
                                    let isCurrentMonth = currentDate.getMonth() === month;

                                    function isToday(startDate: Date | string, endDate: Date | string, currentDate: Date): boolean {
                                        // Convert strings to Date objects if necessary
                                        const start = typeof startDate === 'string' ? new Date(startDate) : startDate;
                                        const end = typeof endDate === 'string' ? new Date(endDate) : endDate;

                                        // Check if both start and end are valid Date objects
                                        if (isNaN(start.getTime()) || isNaN(end.getTime())) {
                                            return false;
                                        }

                                        // Compare full date: day, month, and year
                                        const isStartBeforeOrOnCurrent =
                                            start.getFullYear() < currentDate.getFullYear() ||
                                            (start.getFullYear() === currentDate.getFullYear() && start.getMonth() < currentDate.getMonth()) ||
                                            (start.getFullYear() === currentDate.getFullYear() && start.getMonth() === currentDate.getMonth() && start.getDate() <= currentDate.getDate());

                                        const isEndAfterOrOnCurrent =
                                            end.getFullYear() > currentDate.getFullYear() ||
                                            (end.getFullYear() === currentDate.getFullYear() && end.getMonth() > currentDate.getMonth()) ||
                                            (end.getFullYear() === currentDate.getFullYear() && end.getMonth() === currentDate.getMonth() && end.getDate() >= currentDate.getDate());

                                        return isStartBeforeOrOnCurrent && isEndAfterOrOnCurrent;
                                    }

                                    return (
                                        <div key={dayIndex} className={isCurrentMonth ?
                                            (currentDate.getDate() === date.getDate())
                                                && (currentDate.getFullYear() === date.getFullYear())
                                                && (currentDate.getMonth() === date.getMonth())
                                                ? 'month-overview-day  month-overview-day-today'
                                                : 'month-overview-day'
                                            : 'other-month-day'}
                                            onClick={() => {handleDayRedirect(currentDate)}}>
                                            <div className="month-overview-day-date-row">
                                                {currentDate.getDate()}.{currentDate.getMonth() + 1}.{currentDate.getFullYear()}
                                                <IconButton onClick={(e) => { createAppointment(e, currentDate) }} >
                                                    <FontAwesomeIcon icon={faPlusCircle} size="xs" color="green" />
                                                </IconButton>
                                            </div>
                                            <div className="month-overview-day-appointments">
                                                {appointments ? appointments
                                                    .filter((appointment) =>
                                                        isToday(
                                                            appointment.startDate,
                                                            appointment.endDate,
                                                            currentDate
                                                        )
                                                    ).length + ' appointments' : '0 appointments'}{isCurrentMonth ? currentDate.getDate() === date.getDate()
                                                        ? ' today'
                                                        : ''
                                                        : ''}
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        ))}
                    </div>
                </div>
            </div>

        </>
    );
};

export default observer(MonthOverview);