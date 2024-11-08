
import dayjs from 'dayjs';
import { Field, FieldProps, Form, Formik } from 'formik';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from "@mui/x-date-pickers";
import { Box, Button, IconButton, TextField, Tooltip, Divider, FormControlLabel, Checkbox, Alert } from "@mui/material";
import { AppointmentDto, DetachedDetailsDto, DetailsDto, ReminderDto } from "../../api/dtos/appointment_dtos/AppointmentDto";
import * as yup from 'yup';
import SelectMultiple from "../common/selects/SelectMultiple";
import { useStore } from "../../stores/store";
import { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinusCircle, faPlusCircle } from "@fortawesome/free-solid-svg-icons";
import { observer } from 'mobx-react-lite';
import { useLocation, useParams } from 'react-router-dom';
import * as jsonpatch from "fast-json-patch";

const EditAppointment = () => {
    const { id } = useParams<{ id: string }>();
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
    const [appointment, setAppointment] = useState<AppointmentDto | undefined>(undefined);
    const [validationErrorReminder, setValidationErrorReminder] = useState<string | undefined>(undefined);
    const [validationErrorDate, setValidationErrorDate] = useState<string | undefined>(undefined);

    const location = useLocation();
    const isViewMode: boolean = location.pathname.startsWith("/view");

    useEffect(() => {
        const fetchAppointment = async () => {
            const fetchedAppointment = await appointmentStore.getAppointment(id!, userId);
            setAppointment(fetchedAppointment);
        };
        fetchAppointment();
    }, [id]);

    useEffect(() => {
        if (appointment) {
            setAppointmentInCircles(appointment.circles);
            setDetailsInCircles(appointment.detailsInCircles);
            setShowDetails(!!appointment.details);
            setShowAddress(!!appointment.details?.address);
            setShowReminder(!!appointment.details?.reminders?.length);
            setReminders(appointment.details?.reminders || []);
        }
    }, [appointment]);

    const toggleDetails = () => setShowDetails(!showDetails);
    const toggleAddress = () => setShowAddress(!showAddress);
    const toggleReminder = () => {
        setShowReminder(!showReminder);
        if (!showReminder) { setReminders([]); setReminderDate(new Date()); setReminderMessage(""); }
    }

    const handleEvaluate = () => setEvaluated(true);

    const handleIsSuccess = () => {
        if (!appointmentStore.errorMap.has('editAppointment') &&
            !appointmentStore.errorMap.has('createDetails') &&
            !appointmentStore.errorMap.has('updateDetails') &&
            !appointmentStore.errorMap.has('deleteDetails') &&
            !validationErrorDate && !validationErrorReminder) {
            setIsSuccess(true);
        } else {
            setIsSuccess(false);
        }
    };

    const validationSchema = yup.object().shape({
        title: yup.string().required('Title is required'),
        startDate: yup.date().required('Start date is required'),
        endDate: yup.date().required('End date is required'),
        note: showDetails ? yup.string().required('Note is required') : yup.string(),
        country: showAddress ? yup.string().required('Country is required') : yup.string(),
        city: showAddress ? yup.string().required('City is required') : yup.string(),
        street: showAddress ? yup.string().required('Street is required') : yup.string(),
        housenumber: showAddress ? yup.string().required('House number is required') : yup.string(),
        postCode: showAddress ? yup.string().required('Post code is required') : yup.string()
    });

    return (
        <Formik
            initialValues={{
                startDate: appointment?.startDate || new Date(),
                endDate: appointment?.endDate || new Date(),
                title: appointment?.title || "",
                detailsInCircles: detailsInCircles,
                circles: appointmentInCircles,
                appointmentId: id,
                note: appointment?.details?.note || "",
                country: appointment?.details?.address?.country || "",
                city: appointment?.details?.address?.city || "",
                street: appointment?.details?.address?.street || "",
                housenumber: appointment?.details?.address?.housenumber || "",
                postCode: appointment?.details?.address?.postCode || "",
                reminders: appointment?.details?.reminders || [] as ReminderDto[]
            }}
            enableReinitialize={true}
            validationSchema={validationSchema}
            onSubmit={async (values) => {

                if (appointmentInCircles.length < 1 || (showDetails && detailsInCircles.length < 1)) {
                    return;
                }

                if (values.startDate > values.endDate) {
                    setValidationErrorDate("Start date must be before end date.");
                    return;
                }

                let appointmentDto: AppointmentDto = {
                    id: appointment!.id,
                    title: values.title,
                    creatorId: appointment!.creatorId,
                    startDate: values.startDate,
                    endDate: values.endDate,
                    circles: appointmentInCircles,
                    detailsInCircles: detailsInCircles
                };

                let details: DetailsDto = {
                    appointmentId: appointment!.id,
                    note: values.note,
                    address: undefined
                };

                if (showAddress) {
                    let address = {
                        country: values.country,
                        city: values.city,
                        street: values.street,
                        housenumber: values.housenumber,
                        postCode: values.postCode,
                        longitude: 0,
                        latitude: 0
                    }
                    details.address = address;
                }

                if (showReminder) {
                    details.reminders = reminders;
                }

                if (appointment) {
                    if (showDetails && appointment.details) {
                        const patchDocumentD = jsonpatch.compare(appointment.details, details);
                        const jsonPatchD = JSON.stringify(patchDocumentD);
                        if (jsonPatchD !== "[]") {
                            await appointmentStore.updateDetails(appointment.id, jsonPatchD, userId);
                        }
                    } else if (showDetails && !appointment.details) {
                        let ddDto: DetachedDetailsDto = {
                            userId: userId,
                            details: details
                        };
                        await appointmentStore.createDetails(ddDto);
                    } else if (!showDetails && appointment.details) {
                        await appointmentStore.deleteDetails(appointment!.id, userId);
                    }

                    appointment.details = details;
                    const patchDocumentA = jsonpatch.compare(appointment, appointmentDto);
                    const jsonPatchA = JSON.stringify(patchDocumentA);
                    await appointmentStore.updateAppointment(appointment.id, jsonPatchA, userId);

                    handleIsSuccess();
                    return;
                }
                appointmentStore.errorMap.set('editAppointment', 'Something went wrong, please try again later.');

            }}>
            {({ errors, touched, setFieldValue }) => (
                <>
                    <Form>
                        <Box className="profile-container profile-container-ca" sx={{ padding: 2, gap: 2 }}>
                            <Box className="profile-container profile-container-ca" sx={{ gap: 2 }}>
                                <span>Edit Appointment</span>
                                <Field name="title">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Title"
                                            error={touched.title && Boolean(errors.title)}
                                            helperText={touched.title && errors.title}
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value);
                                                }}
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
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value);
                                                }}
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
                            {!isViewMode && <div className="expend-button-container">
                                <span>Add Details</span>
                                <Tooltip title={showDetails ? "Remove Details" : "Add Details"} >
                                    <IconButton onClick={toggleDetails}>
                                        <FontAwesomeIcon icon={showDetails ? faMinusCircle : faPlusCircle} size="xs" color={showDetails ? "red" : "green"} />
                                    </IconButton>
                                </Tooltip>
                            </div>}
                            <Box className="profile-container profile-container-ca" style={{ display: showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>

                                <Field name="note" >
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            label="Note"
                                            error={touched.note && Boolean(errors.note)}
                                            helperText={touched.note && errors.note}
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                            {!isViewMode && <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Address</span>
                                <Tooltip title={showAddress ? "Remove Address" : "Add Address"} >
                                    <IconButton onClick={toggleAddress} >
                                        <FontAwesomeIcon icon={showAddress ? faMinusCircle : faPlusCircle} size="xs" color={showAddress ? "red" : "green"} />
                                    </IconButton>
                                </Tooltip>
                            </div>}
                            <Box className="profile-container profile-container-ca" style={{ display: showAddress && showDetails ? 'flex' : 'none' }} sx={{ gap: 2 }}>
                                <Field name="country">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Country"
                                            error={touched.country && Boolean(errors.country)}
                                            helperText={touched.country && errors.country}
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
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
                                            value={field.value}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                            }}
                                        />
                                    )}
                                </Field>
                            </Box>
                            {!isViewMode && <div className="expend-button-container" style={{ display: showDetails ? 'flex' : 'none' }}>
                                <span>Add Reminder</span>
                                <Tooltip title={showReminder ? "Remove All reminders" : "Add Reminder"} >
                                    <IconButton onClick={toggleReminder}>
                                        <FontAwesomeIcon icon={showReminder ? faMinusCircle : faPlusCircle} size="xs" color={showReminder ? "red" : "green"} />
                                    </IconButton>
                                </Tooltip>
                            </div>}
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
                                                {!isViewMode && <div>
                                                    <IconButton onClick={() => {
                                                        setReminders(reminders.filter((_, i) => i !== index));
                                                    }}>
                                                        <FontAwesomeIcon icon={faMinusCircle} size="xs" color={"red"} />
                                                    </IconButton>
                                                </div>}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                                <Field name="reminderDate">
                                    {({ field }: FieldProps) => (
                                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                                            <DateTimePicker
                                                {...field}
                                                label="Start Date"
                                                value={dayjs(reminderDate)}
                                                onChange={(value) => {
                                                    setFieldValue(field.name, value);
                                                    setReminderDate(value ? value.toDate() : new Date());
                                                    setValidationErrorReminder(undefined);
                                                }}
                                                ampm={false}
                                                format="YYYY-MM-DD HH:mm"
                                            />
                                        </LocalizationProvider>
                                    )}
                                </Field>
                                <Field name="message">
                                    {({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            multiline={true}
                                            label="message"
                                            value={reminderMessage}
                                            onChange={(e) => {
                                                setFieldValue(field.name, e.target.value);
                                                setReminderMessage(e.target.value);
                                                setValidationErrorReminder(undefined);
                                            }}
                                            error={touched.title && Boolean(errors.title)}
                                            helperText={touched.title && errors.title}
                                        />
                                    )}
                                </Field>
                                {!isViewMode && (
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
                                )}
                                {!isViewMode && validationErrorReminder && <Alert severity="error">{validationErrorReminder}</Alert>}
                                {!isViewMode && (
                                    <Button variant="text" onClick={() => {
                                        if (!reminderMessage || !reminderDate || isNaN(reminderDate.getTime())) {
                                            setValidationErrorReminder("Reminder message and date are required.");
                                            return;
                                        }
                                        setReminders([...reminders, { time: reminderDate, message: reminderMessage, justForUser: justForUser }]);
                                        setReminderDate(new Date());
                                        setReminderMessage("");
                                        setJustForUser(true);
                                    }}>Add Reminder</Button>
                                )}
                            </Box>
                            {!isViewMode && <Divider />}
                            {isSuccess && <Alert severity="success">Appointment updated successfully!</Alert>}
                            {appointmentStore.errorMap.has('editAppointment') && <Alert severity="error">{appointmentStore.errorMap.get('editAppointment')}</Alert>}
                            {appointmentStore.errorMap.has('createDetails') && <Alert severity="error">{appointmentStore.errorMap.get('createDetails')}</Alert>}
                            {appointmentStore.errorMap.has('updateDetails') && <Alert severity="error">{appointmentStore.errorMap.get('updateDetails')}</Alert>}
                            {appointmentStore.errorMap.has('deleteDetails') && <Alert severity="error">{appointmentStore.errorMap.get('deleteDetails')}</Alert>}
                            {validationErrorDate && <Alert severity="error">{validationErrorDate}</Alert>}
                            {!isViewMode && <Button variant="contained" type="submit" onClick={handleEvaluate}>Update Appointment</Button>}
                        </Box>
                    </Form>
                </>
            )}
        </Formik >
    );
};

export default observer(EditAppointment);