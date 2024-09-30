import { observer } from "mobx-react-lite";
import dayjs from 'dayjs';
import { Field, FieldProps, Form, Formik } from 'formik';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from "@mui/x-date-pickers";
import { Box, Button, IconButton, TextField, Tooltip, Divider, FormControlLabel, Checkbox, Alert } from "@mui/material";
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
    const [appointmentInCircles, setAppointmentInCircles] = useState<string[]>([]);
    const [detailsInCircles, setDetailsInCircles] = useState<string[]>([]);
    const [showDetails, setShowDetails] = useState<boolean>(false);
    const [showAddress, setShowAddress] = useState<boolean>(false);
    const [showReminder, setShowReminder] = useState<boolean>(false);
    const [evaluated, setEvaluated] = useState<boolean>(false);
    const [reminders, setReminders] = useState<ReminderDto[]>([]);
    const [reminderDate, setReminderDate] = useState<Date>(new Date());
    const [reminderMessage, setReminderMessage] = useState<string>("");
    const [justForUser, setJustForUser] = useState<boolean>(true);
    const [isSuccess, setIsSuccess] = useState<boolean>(false);

    const toggleDetails = () => setShowDetails(!showDetails);
    const toggleAddress = () => setShowAddress(!showAddress);
    const toggleReminder = () => {
        setShowReminder(!showReminder);
        if (!showReminder) { setReminders([]); setReminderDate(new Date()); setReminderMessage(""); }
    }

    const handleEvaluate = () => setEvaluated(true);

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
        //circles: yup.array().min(1, 'At least one circle is required'),
        details: showDetails ?
            yup.object().shape({
                note: yup.string().required('Note is required'),
                address: showAddress
                    ? yup.object().shape({
                        country: yup.string().required('Country is required'),
                        city: yup.string().required('City is required'),
                        street: yup.string().required('Street is required'),
                        housenumber: yup.string().required('House number is required'),
                        postCode: yup.string().required('Post code is required'),
                    })
                    : yup.object(),
                // reminders: showReminder
                //     ? yup.array().of(
                //         yup.object().shape({
                //             date: yup.date().required('Reminder date is required'),
                //             message: yup.string().required('Reminder message is required'),
                //         })
                //     )
                //     : yup.array(),
            }) : yup.object(),
    });


    return (
        <Formik
            initialValues={{
                id: "",
                creatorId: "",
                startDate: new Date(localStorage.getItem('selectedDate')!),
                endDate: new Date(localStorage.getItem('selectedDate')!),
                title: "",
                detailsInCircles: [],
                circles: [],
                details: {
                    appointmentId: "",
                    note: "",
                    address: {
                        country: "",
                        city: "",
                        street: "",
                        housenumber: "",
                        postCode: "",
                        longitude: 0,
                        latitude: 0,
                    },
                    reminders: [],
                },
            } as AppointmentDto}
            validationSchema={validationSchema}
            onSubmit={async (values) => {
                if (appointmentInCircles.length < 1 || (showDetails && detailsInCircles.length < 1)) {
                    return;
                }

                values.id = uuid();
                values.creatorId = userId;
                values.circles = appointmentInCircles;
                values.detailsInCircles = detailsInCircles;

                if (!showDetails) {
                    values.details = undefined;
                } else {
                    if (values.details) {
                        values.details.appointmentId = values.id;
                    }
                }

                if (showReminder) {
                    values.details!.reminders = reminders;
                }

                await appointmentStore.createAppointment(values);
                handleIsSuccess();
            }}>
            {({ errors, touched, setFieldValue }) => (
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
                                                format="YYYY-MM-DD HH:mm"
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
                                                format="YYYY-MM-DD HH:mm"
                                            />
                                        </LocalizationProvider>
                                    )}
                                </Field>
                                <Field name="circles">
                                    {() => (
                                        <SelectMultiple
                                            dataSource={circles}
                                            value={appointmentInCircles}
                                            onChange={setAppointmentInCircles}
                                            className={touched.circles && Boolean(errors.circles) ? 'alert-border' : ''}
                                        />
                                    )}
                                </Field>
                                <span className="alert-span" style={{ display: appointmentInCircles.length < 1 && evaluated ? 'flex' : 'none' }}>At least one circle is required.</span>
                            </Box>
                            <div className="expend-button-container">
                                <span>Add Details</span>
                                <Tooltip title={showDetails ? "Remove Details" : "Add Details"} >
                                    <IconButton onClick={toggleDetails}>
                                        <FontAwesomeIcon icon={showDetails ? faMinusCircle : faPlusCircle} size="xs" color={showDetails ? "red" : "green"} />
                                    </IconButton>
                                </Tooltip>
                            </div>
                            <Box className="profile-container profile-container-ca" style={{ display: showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>

                                <Field name="details.note" >
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            label="Note"
                                            error={touched.details?.note && Boolean(errors.details?.note)}
                                            helperText={touched.details?.note && errors.details?.note}
                                        />
                                    )}
                                </Field>
                                <Field name="details.circles">
                                    {() => (
                                        <SelectMultiple
                                            dataSource={circles.filter(circle => appointmentInCircles.includes(circle.id))}
                                            value={detailsInCircles}
                                            onChange={setDetailsInCircles}
                                        />
                                    )}
                                </Field>
                                <span className="alert-span" style={{ display: detailsInCircles.length < 1 && evaluated && showDetails ? 'flex' : 'none' }}>At least one circle is required.</span>
                            </Box>
                            <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Address</span>
                                <Tooltip title={showAddress ? "Remove Address" : "Add Address"} >
                                    <IconButton onClick={toggleAddress} >
                                        <FontAwesomeIcon icon={showAddress ? faMinusCircle : faPlusCircle} size="xs" color={showAddress ? "red" : "green"} />
                                    </IconButton>
                                </Tooltip>
                            </div>
                            <Box className="profile-container profile-container-ca" style={{ display: showAddress && showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>
                                <Field name="details.address.country">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Country"
                                            error={touched.details?.address?.country && Boolean(errors.details?.address?.country)}
                                            helperText={touched.details?.address?.country && errors.details?.address?.country}
                                        />
                                    )}
                                </Field>
                                <Field name="details.address.city">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="City"
                                            error={touched.details?.address?.city && Boolean(errors.details?.address?.city)}
                                            helperText={touched.details?.address?.city && errors.details?.address?.city}
                                        />
                                    )}
                                </Field>
                                <Field name="details.address.street">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Street"
                                            error={touched.details?.address?.street && Boolean(errors.details?.address?.street)}
                                            helperText={touched.details?.address?.street && errors.details?.address?.street}
                                        />
                                    )}
                                </Field>
                                <Field name="details.address.housenumber">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="House Number"
                                            error={touched.details?.address?.housenumber && Boolean(errors.details?.address?.housenumber)}
                                            helperText={touched.details?.address?.housenumber && errors.details?.address?.housenumber}
                                        />
                                    )}
                                </Field>
                                <Field name="details.address.postCode">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Post Code"
                                            error={touched.details?.address?.postCode && Boolean(errors.details?.address?.postCode)}
                                            helperText={touched.details?.address?.postCode && errors.details?.address?.postCode}
                                        />
                                    )}
                                </Field>
                            </Box>
                            <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Reminder</span>
                                <Tooltip title={showReminder ? "Remove All reminders" : "Add Reminder"} >
                                    <IconButton onClick={toggleReminder}>
                                        <FontAwesomeIcon icon={showReminder ? faMinusCircle : faPlusCircle} size="xs" color={showReminder ? "red" : "green"} />
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
                                                    <span>{reminder.time.toLocaleString()}</span>
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
                                <Field name="details.reminders.date">
                                    {({ field }: FieldProps) => (
                                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                                            <DateTimePicker
                                                {...field}
                                                label="Start Date"
                                                value={dayjs(reminderDate)}
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value);
                                                    setReminderDate(value ? value.toDate() : new Date());
                                                }}
                                                ampm={false}
                                                format="YYYY-MM-DD HH:mm"
                                            />
                                        </LocalizationProvider>
                                    )}
                                </Field>
                                <Field name="details.reminders.message">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            label="message"
                                            value={reminderMessage}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                                setReminderMessage(e.target.value);
                                            }}
                                        />
                                    )}
                                </Field>
                                <Field name="details.reminders.justForUser">
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
                                <Button variant="text" onClick={() => {
                                    setReminders([...reminders, { time: reminderDate, message: reminderMessage, justForUser: justForUser }]);
                                    setReminderDate(new Date());
                                    setReminderMessage("");
                                    setJustForUser(true);
                                }}>Add Reminder</Button>
                            </Box>
                            <Divider />
                            {isSuccess && <Alert severity="success">Appointment created successfully!</Alert>}
                            {appointmentStore.errorMap.has('createAppointment') && <Alert severity="error">{appointmentStore.errorMap.get('createAppointment')}</Alert>}
                            <Button variant="contained" type="submit" onClick={handleEvaluate}>Create Appointment</Button>
                        </Box>
                    </Form>
                </>
            )}
        </Formik >
    );
};

export default observer(CreateAppointment);