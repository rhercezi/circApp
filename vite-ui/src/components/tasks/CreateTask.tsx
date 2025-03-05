import { Alert, Autocomplete, Box, Button, CircularProgress, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";
import { Field, FieldProps, Form, Formik } from "formik";
import * as Yup from 'yup';
import { useStore } from "../../stores/store";
import { useEffect, useState } from "react";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import { UserDto } from "../../api/dtos/user_dtos/UserDto";
import { observer } from "mobx-react-lite";
import uuid from "react-uuid";
import { useParams } from "react-router-dom";
import Loader from "../common/Loader";

const isUserDto = (option: string | UserDto): option is UserDto => {
    return typeof option !== 'string';
};

const CreateTask = () => {
    const { id } = useParams<{ id: string }>();
    const { userStore, circleStore, taskStore } = useStore();
    const [inputValue, setInputValue] = useState('');
    const circles: CircleDto[] = Array.from(circleStore.circlesMap.values());
    const [options, setOptions] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState(false);
    const [isExecuting, setIsExecuting] = useState(false);
    const [executingText, setExecutingText] = useState('');
    const [initialValues, setInitialValues] = useState({
        title: '',
        description: '',
        endDate: new Date(),
        users: [] as UserDto[],
        circleId: ''
    });

    const getUser = async (id: string): Promise<UserDto> => {
        const user = await userStore.getUser(id);
        if (!user) {
            throw new Error(`User with id ${id} not found`);
        }
        return user;
    };

    useEffect(() => {
        const fetchTaskDetails = async () => {
            if (id) {
                const task = taskStore.tasks.find(task => task.id === id);
                if (task) {
                    const users = await Promise.all(
                        (task.userModels ?? []).map(async (u) => await getUser(u.id))
                    );
                    setInitialValues({
                        title: task.title || '',
                        description: task.description || '',
                        endDate: task.endDate || new Date(),
                        users,
                        circleId: task.circleId || ''
                    });
                }
            }
        };

        fetchTaskDetails();
    }, [id, taskStore, userStore]);

    const handleIsSuccess = () => {
        if (!taskStore.errorMap.has('createTask') && !taskStore.errorMap.has('updateTask')) {
            setIsSuccess(true);
        } else {
            setIsSuccess(false);
        }
    };

    useEffect(() => {
        let active = true;
        if (inputValue.length < 3) {
            setOptions([]);
            return undefined;
        }
        setLoading(true);
        (async () => {
            const users = await circleStore.searchUsers(inputValue);
            if (active) {
                setOptions(users || []);
                setLoading(false);
            }
        })();
        return () => {
            active = false;
        };
    }, [inputValue]);

    const validationSchema = Yup.object({
        title: Yup.string().required('Required'),
        description: Yup.string().required('Required'),
        endDate: Yup.string().required('Required'),
        users: Yup.array().test('at-least-one', 'Circle or users must be provided', function (value) {
            const { circleId } = this.parent;
            return value!.length > 0 || (circleId !== 'false' && circleId !== undefined);
        }).test('no-both', 'Both Circle and users cannot be provided', function (value) {
            const { circleId } = this.parent;
            return value!.length === 0 || circleId === 'false' || circleId === undefined;
        }),
        circleId: Yup.string().test('at-least-one', 'Circle or users must be provided', function (value) {
            const { users } = this.parent;
            return (value !== 'false' && value !== undefined) || users.length > 0;
        }).test('no-both', 'Both Circle and users cannot be provided', function (value) {
            const { users } = this.parent;
            return value === 'false' || users.length === 0 || value === undefined;
        })
    });

    if (taskStore.loading) {
        return <Loader text={taskStore.loaderText} className="loader" />;
    }

    return (
        <Formik
            initialValues={initialValues}
            enableReinitialize={true}
            validationSchema={validationSchema}
            onSubmit={
                async (values) => {
                    setIsExecuting(true);
                    setExecutingText(id ? 'Updating Task' : 'Creating Task');
                    try {
                        const task = {
                            id: uuid(),
                            ownerId: userStore.user?.id!,
                            title: values.title,
                            description: values.description,
                            isCompleted: false,
                            endDate: values.endDate,
                            createdAt: new Date(),
                            updatedAt: undefined,
                            parentTaskId: undefined,
                            userModels: values.users.map(user => ({
                                id: user.id,
                                userName: user.userName!,
                                isCompleted: false,
                                completedAt: undefined
                            })),
                            circleId: values.circleId === 'false' || values.circleId === '' ? undefined : values.circleId
                        };

                        if (id) {
                            task.id = id;
                            await taskStore.updateTask(task);
                        } else {
                            await taskStore.createTask(task);
                        }
                    }
                    catch (error) {
                        console.log(error);
                    }
                    finally {
                        handleIsSuccess();
                        setIsExecuting(false);
                        setExecutingText('');
                    }
                }
            }>

            {({ errors, touched, setFieldValue }) => (
                <Form>
                    <Box className="profile-container profile-container-ca" sx={{ padding: 2, gap: 2 }}>
                        <span>Create Task</span>
                        <Field name="title">
                            {({ field }: FieldProps) => (
                                <TextField
                                    {...field}
                                    label="Title"
                                    error={touched.title && Boolean(errors.title)}
                                    helperText={touched.title && errors.title} />
                            )}
                        </Field>

                        <Field name="description">
                            {({ field }: FieldProps) => (
                                <TextField
                                    {...field}
                                    multiline={true}
                                    minRows={3}
                                    label="Description"
                                    error={touched.description && Boolean(errors.description)}
                                    helperText={touched.description && errors.description} />
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

                        <Field name="users">
                            {({ form }: FieldProps) => (
                                <Autocomplete
                                    multiple
                                    freeSolo
                                    options={options}
                                    loading={loading}
                                    inputValue={inputValue}
                                    value={form.values.users}
                                    getOptionLabel={(option) => isUserDto(option) ?
                                        option.firstName + ' ' + option.familyName + ' (' + option.userName + ')'
                                        : option}
                                    getOptionKey={(option) => isUserDto(option) ? option.id! : option}
                                    onInputChange={(_, newInputValue) => {
                                        setInputValue(newInputValue);
                                    }}
                                    onChange={(_, newValue) => {
                                        form.setFieldValue('users', newValue);
                                    }}
                                    renderInput={(params) => (
                                        <TextField
                                            {...params}
                                            label="Search users"
                                            variant="outlined"
                                            InputProps={{
                                                ...params.InputProps,
                                                endAdornment: (
                                                    <>
                                                        {loading ? <CircularProgress color="inherit" size={20} /> : null}
                                                        {params.InputProps.endAdornment}
                                                    </>
                                                ),
                                            }}
                                            error={touched.users && Boolean(errors.users)}
                                            helperText={touched.users && typeof errors.users === 'string' ? errors.users : ''}
                                        />
                                    )}
                                />
                            )}
                        </Field>

                        <Field name="circleId">
                            {({ form }: FieldProps) => (
                                <FormControl sx={{ mminWidth: 120, width: "100%" }} size="small"
                                    error={touched.circleId && Boolean(errors.circleId)}>
                                    <InputLabel id="circle-select">Select Circle</InputLabel>
                                    <Select
                                        id="circle-select"
                                        labelId="circle-select"
                                        label="Select Circle"
                                        value={form.values.circleId}
                                        variant="outlined"
                                        onChange={(e) => {
                                            form.setFieldValue('circleId', e.target.value);
                                        }}
                                        displayEmpty
                                    >
                                        <MenuItem selected={true} key={uuid()} value={'false'}>All Circles</MenuItem>
                                        {
                                            circles.map(circle => (
                                                <MenuItem key={circle.id} value={circle.id}>{circle.name}</MenuItem>
                                            ))
                                        }
                                    </Select>
                                    {touched.circleId && errors.circleId ? (
                                        <FormHelperText>{errors.circleId}</FormHelperText>
                                    ) : null}
                                </FormControl>
                            )}
                        </Field>
                        {isSuccess && <Alert severity="success">{id ? 'Task successfully updated' : 'Task created successfully'}</Alert>}
                        {taskStore.errorMap.has('createTask') && <Alert severity="error">{taskStore.errorMap.get('createTask')}</Alert>}
                        {taskStore.errorMap.has('updateTask') && <Alert severity="error">{taskStore.errorMap.get('updateTask')}</Alert>}
                        <Button variant="contained" type="submit" >
                            {isExecuting ? (<><CircularProgress size={"1rem"} color="success" />
                                {executingText}</>) : id ? "Update Task" : "Create Task"} </Button>
                    </Box>
                </Form>
            )}
        </Formik>
    );
}

export default observer(CreateTask);