import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import { AppointmentDto } from "../../api/dtos/appointment_dtos/AppointmentDto";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleDoubleDown, faAngleDoubleUp } from "@fortawesome/free-solid-svg-icons";
import { useNavigate } from "react-router-dom";
import uuid from "react-uuid";
import CalendarDay from "./CalendarDay";

const isNumeric = (value: any): boolean => {
    return !isNaN(parseFloat(value)) && isFinite(value);
};

const MonthOverview = () => {
    const { circleStore, userStore, appointmentStore } = useStore();

    const userId = userStore.user?.id;
    const circles: CircleDto[] = [...circleStore.circlesMap.values()];
    const [selectedCircle, setSelectedCircle] = useState<string | undefined>(undefined);
    const [appointments, setAppointments] = useState<AppointmentDto[]>([]);
    const [year, setYear] = useState<number>(
        localStorage.getItem("selectedYear") && isNumeric(localStorage.getItem("selectedYear")) 
            ? parseInt(localStorage.getItem("selectedYear") as string) 
            : new Date().getFullYear()
    );
    const [month, setMonth] = useState<number>(
        localStorage.getItem("selectedMonth") && isNumeric(localStorage.getItem("selectedMonth")) 
            ? parseInt(localStorage.getItem("selectedMonth") as string) 
            : new Date().getMonth()
    );
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
                                else { 
                                    setYear(e.target.value as number);
                                    localStorage.setItem("selectedYear", e.target.value as string);
                                }
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
                            onChange={(e) => { 
                                setMonth(e.target.value as number);
                                localStorage.setItem("selectedMonth", e.target.value as string);
                            }}
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

                                    return (
                                        <CalendarDay key={dayIndex*weekIndex} currentDate={currentDate} 
                                                 date={date} 
                                                 isCurrentMonth={isCurrentMonth} 
                                                 appointments={appointments} 
                                                 createAppointment={createAppointment} 
                                                 handleDayRedirect={handleDayRedirect} />
                                    )
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