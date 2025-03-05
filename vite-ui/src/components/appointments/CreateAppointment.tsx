import { observer } from "mobx-react-lite";
import dayjs from 'dayjs';
import { Field, FieldProps, Form, Formik } from 'formik';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from "@mui/x-date-pickers";
import { Box, Button, IconButton, TextField, Tooltip, Divider, FormControlLabel, Checkbox, Alert, FormHelperText } from "@mui/material";
import { AppointmentDto, ReminderDto } from "../../api/dtos/appointment_dtos/AppointmentDto";
import * as yup from 'yup';
import SelectMultiple from "../common/selects/SelectMultiple";
import { useStore } from "../../stores/store";
import { useState } from "react";
import uuid from "react-uuid";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinusCircle, faPlusCircle } from "@fortawesome/free-solid-svg-icons";

const CreateAppointment = () => {
    const { circleStore, userStore, appointmentStore } = useStore();
    const userId = userStore.user?.id!;
    const circles = [...circleStore.circlesMap.values()];
    const [showDetails, setShowDetails] = useState<boolean>(false);
    const [showAddress, setShowAddress] = useState<boolean>(false);
    const [showReminder, setShowReminder] = useState<boolean>(false);
    const [reminders, setReminders] = useState<ReminderDto[]>([]);
    const [reminderDate, setReminderDate] = useState<Date>(new Date());
    const [reminderMessage, setReminderMessage] = useState<string>("");
    const [justForUser, setJustForUser] = useState<boolean>(true);
    const [isSuccess, setIsSuccess] = useState<boolean>(false);
    const [reminderDateError, setReminderDateError] = useState<string>("");
    const [reminderMessageError, setReminderMessageError] = useState<string>("");

    const toggleDetails = () => setShowDetails(!showDetails);
    const toggleAddress = () => setShowAddress(!showAddress);
    const toggleReminder = () => {
        setShowReminder(!showReminder);
        if (!showReminder) { setReminders([]); setReminderDate(new Date()); setReminderMessage(""); }
    };

    const handleIsSuccess = () => {
        if (!appointmentStore.errorMap.has('createAppointment')) {
            setIsSuccess(true);
        } else {
            setIsSuccess(false);
        }
    };

    const validationSchema = yup.object().shape({
        startDate: yup.date().required('Start date is required'),
        endDate: yup.date().required('End date is required'),
        title: yup.string().required('Title is required'),
        circles: yup.array()
            .min(1, 'At least one circle must be selected')
            .required('Circles are required'),
        note: showDetails ? yup.string().required('Note is required') : yup.string(),
        detailsInCircles: showDetails ? yup.array()
            .min(1, 'At least one circle must be selected')
            .required('Details in circles are required')
            : yup.array(),
        country: showAddress ? yup.string().required('Country is required') : yup.string(),
        city: showAddress ? yup.string().required('City is required') : yup.string(),
        street: showAddress ? yup.string().required('Street is required') : yup.string(),
        housenumber: showAddress ? yup.string().required('House number is required') : yup.string(),
        postCode: showAddress ? yup.string().required('Post code is required') : yup.string()
    });


    return (
        <Formik
            initialValues={{
                id: "",
                creatorId: "",
                startDate: new Date(localStorage.getItem('selectedDate')!),
                endDate: new Date(localStorage.getItem('selectedDate')!),
                title: "",
                detailsInCircles: [] as string[],
                circles: [] as string[],
                appointmentId: "",
                note: "",
                country: "",
                city: "",
                street: "",
                housenumber: "",
                postCode: "",
                longitude: 0,
                latitude: 0,
                reminders: [] as ReminderDto[],
            }}
            validationSchema={validationSchema}
            onSubmit={async (values) => {
                const id = uuid();
                const appointment = {
                    id: id,
                    creatorId: userId,
                    title: values.title,
                    startDate: values.startDate,
                    endDate: values.endDate,
                    circles: values.circles,
                    detailsInCircles: values.detailsInCircles,
                    details: showDetails ? {
                        appointmentId: id,
                        note: values.note,
                        address: showAddress ? {
                            country: values.country,
                            city: values.city,
                            street: values.street,
                            housenumber: values.housenumber,
                            postCode: values.postCode,
                            longitude: 0,
                            latitude: 0,
                        } : undefined,
                        reminders: showReminder ? reminders : undefined,
                    } : undefined,
                } as AppointmentDto;

                await appointmentStore.createAppointment(appointment);
                handleIsSuccess();
            }}>
            {({ values, errors, touched, setFieldValue }) => (
                <>
                    <Form>
                        <Box className="profile-container profile-container-ca" sx={{ padding: 2, gap: 2 }}>
                            <Box className="profile-container profile-container-ca" sx={{ gap: 2 }}>
                                <span>Create Appointment</span>
                                <Field name="title">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Title"
                                            multiline={true}
                                            error={touched.title && Boolean(errors.title)}
                                            helperText={touched.title && errors.title}
                                        />
                                    )}
                                </Field>
                                <Field name="startDate">
                                    {({ field }: FieldProps) => (
                                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                                            <DateTimePicker
                                                {...field}
                                                label="Start Date"
                                                value={dayjs(field.value)}
                                                onChange={(value) => { setFieldValue(field.name, value); }}
                                                ampm={false}
                                                format="DD.MM.YYYY HH:mm:ss"
                                            />
                                        </LocalizationProvider>
                                    )}
                                </Field>
                                <Field name="endDate">
                                    {({ field }: FieldProps) => (
                                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                                            <DateTimePicker
                                                {...field}
                                                label="End Date"
                                                value={dayjs(field.value)}
                                                onChange={(value) => { setFieldValue(field.name, value); }}
                                                ampm={false}
                                                format="DD.MM.YYYY HH:mm:ss"
                                            />
                                        </LocalizationProvider>
                                    )}
                                </Field>
                                <Field name="circles">
                                    {({ field }: FieldProps) => (
                                        <>
                                            <SelectMultiple
                                                dataSource={circles}
                                                value={field.value}
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value);
                                                }}
                                            />
                                            {touched.circles && errors.circles && (
                                                <FormHelperText error>{errors.circles}</FormHelperText>
                                            )}
                                        </>
                                    )}
                                </Field>
                            </Box>
                            <div className="expend-button-container">
                                <span>Add Details</span>
                                <Tooltip title={showDetails ? "Remove Details" : "Add Details"} >
                                    <IconButton onClick={toggleDetails}>
                                        <FontAwesomeIcon icon={showDetails ? faMinusCircle : faPlusCircle} size="xs" color={showDetails ? '#d12121' : '#1976D2'} />
                                    </IconButton>
                                </Tooltip>
                            </div>
                            <Box className="profile-container profile-container-ca" style={{ display: showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>

                                <Field name="note" >
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            minRows={3}
                                            label="Note"
                                            error={touched.note && Boolean(errors.note)}
                                            helperText={touched.note && errors.note}
                                        />
                                    )}
                                </Field>
                                <Field name="detailsInCircles">
                                    {({ field }: FieldProps) => (
                                        <>
                                            <SelectMultiple
                                                dataSource={circles.filter(circle => values.circles.includes(circle.id))}
                                                value={field.value}
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value)
                                                }}
                                            />
                                            {touched.detailsInCircles && errors.detailsInCircles && (
                                                <FormHelperText error>{errors.detailsInCircles}</FormHelperText>
                                            )}
                                        </>
                                    )}
                                </Field>
                            </Box>
                            <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Address</span>
                                <Tooltip title={showAddress ? "Remove Address" : "Add Address"} >
                                    <IconButton onClick={toggleAddress} >
                                        <FontAwesomeIcon icon={showAddress ? faMinusCircle : faPlusCircle} size="xs" color={showAddress ? '#d12121' : '#1976D2'} />
                                    </IconButton>
                                </Tooltip>
                            </div>
                            <Box className="profile-container profile-container-ca" style={{ display: showAddress && showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>
                                <Field name="country">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Country"
                                            error={touched.country && Boolean(errors.country)}
                                            helperText={touched.country && errors.country}
                                        />
                                    )}
                                </Field>
                                <Field name="city">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="City"
                                            error={touched.city && Boolean(errors.city)}
                                            helperText={touched.city && errors.city}
                                        />
                                    )}
                                </Field>
                                <Field name="street">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Street"
                                            error={touched.street && Boolean(errors.street)}
                                            helperText={touched.street && errors.street}
                                        />
                                    )}
                                </Field>
                                <Field name="housenumber">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="House Number"
                                            error={touched.housenumber && Boolean(errors.housenumber)}
                                            helperText={touched.housenumber && errors.housenumber}
                                        />
                                    )}
                                </Field>
                                <Field name="postCode">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Post Code"
                                            error={touched.postCode && Boolean(errors.postCode)}
                                            helperText={touched.postCode && errors.postCode}
                                        />
                                    )}
                                </Field>
                            </Box>
                            <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Reminder</span>
                                <Tooltip title={showReminder ? "Remove All reminders" : "Add Reminder"} >
                                    <IconButton onClick={toggleReminder}>
                                        <FontAwesomeIcon icon={showReminder ? faMinusCircle : faPlusCircle} size="xs" color={showReminder ? '#d12121' : '#1976D2'} />
                                    </IconButton>
                                </Tooltip>
                            </div>
                            <Box className="profile-container profile-container-ca" style={{ display: showReminder && showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>
                                <div>
                                    {reminders.map((reminder, index) => (
                                        <div key={index}>
                                            <div style={{
                                                display: "flex",
                                                flexDirection: "row",
                                                justifyContent: "space-between",
                                                gap: 2,
                                                borderTop: "1px solid #7a7a7a",
                                                borderBottom: "1px solid #7a7a7a",
                                                padding: "0.5rem 0"
                                            }}>
                                                <div style={{ display: "flex", flexDirection: "column" }}>
                                                    <span>{new Intl.DateTimeFormat('de-DE', {
                                                        day: '2-digit',
                                                        month: '2-digit',
                                                        year: 'numeric',
                                                        hour: '2-digit',
                                                        minute: '2-digit',
                                                        second: '2-digit',
                                                        hour12: false
                                                    }).format(new Date(reminder.time))}</span>
                                                    <span>{reminder.message}</span>
                                                </div>
                                                <div>
                                                    <IconButton onClick={() => {
                                                        setReminders(reminders.filter((_, i) => i !== index));
                                                    }}>
                                                        <FontAwesomeIcon icon={faMinusCircle} size="xs" color={"red"} />
                                                    </IconButton>
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                                <Field name="reminderDate">
                                    {({ field }: FieldProps) => (
                                        <>
                                            <LocalizationProvider dateAdapter={AdapterDayjs}>
                                                <DateTimePicker
                                                    {...field}
                                                    label="Start Date"
                                                    value={dayjs(reminderDate)}
                                                    onChange={(value) => {
                                                        setFieldValue(field.name, value);
                                                        setReminderDate(value ? value.toDate() : new Date());
                                                        setReminderDateError("");
                                                    }}
                                                    ampm={false}
                                                    format="DD.MM.YYYY HH:mm:ss"
                                                />
                                            </LocalizationProvider>
                                            {reminderDateError && <FormHelperText error>{reminderDateError}</FormHelperText>}
                                        </>
                                    )}
                                </Field>
                                <Field name="message">
                                    {({ field }: FieldProps) => (
                                        <>
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            label="message"
                                            value={reminderMessage}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                                setReminderMessage(e.target.value);
                                                setReminderMessageError("");
                                            }}
                                        />
                                        {reminderMessageError && <FormHelperText error>{reminderMessageError}</FormHelperText>}
                                        </>
                                    )}
                                </Field>
                                <Field name="justForUser">
                                    {({ field }: FieldProps) => (
                                        <FormControlLabel
                                            control={<Checkbox defaultChecked
                                                onChange={(e) => {
                                                    setFieldValue(field.name, e.target.checked);
                                                    setJustForUser(e.target.checked);
                                                }} />}
                                            label="Reminder Just For User" />
                                    )}
                                </Field>
                                <Button variant="outlined" onClick={async () => {
                                    if (!reminderDate || reminderDate < new Date()) {
                                        setReminderDateError("Reminder date must be in the future");
                                    } else {
                                        setReminderDateError("");
                                    }
                                    if (!reminderMessage) {
                                        setReminderMessageError("Reminder message is required");
                                    } else {
                                        setReminderMessageError("");
                                    }
                                    if (reminderDateError || reminderMessageError) {
                                        return;
                                    }
                                    setReminders([...reminders, { time: reminderDate, message: reminderMessage, justForUser: justForUser }]);
                                    setReminderDate(new Date());
                                    setReminderMessage("");
                                    setJustForUser(true);
                                }}>Add Reminder</Button>
                            </Box>
                            <Divider />
                            {isSuccess && <Alert severity="success">Appointment created successfully!</Alert>}
                            {appointmentStore.errorMap.has('createAppointment') && <Alert severity="error">{appointmentStore.errorMap.get('createAppointment')}</Alert>}
                            <Button variant="contained" type="submit" >Create Appointment</Button>
                        </Box>
                    </Form>
                </>
            )}
        </Formik >
    );
};

export default observer(CreateAppointment);