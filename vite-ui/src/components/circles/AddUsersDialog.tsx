import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Alert, Autocomplete, Box, Button, CircularProgress, Dialog, DialogContent, DialogContentText, DialogTitle, FormControl, IconButton, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import { Field, FieldProps, Form, Formik } from "formik";
import { UserDto } from "../../api/dtos/user_dtos/UserDto";
import { useStore } from "../../stores/store";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import { useEffect, useState } from "react";
import * as Yup from 'yup';
import Loader from "../common/Loader";

interface Props {
    open: boolean;
    setOpen: (open: boolean) => void;
}

const UserDtoSchema = Yup.object().shape({
    id: Yup.string().required('ID is required.'),
    userName: Yup.string().required('Username is required.'),
    firstName: Yup.string().required('First name is required.'),
    familyName: Yup.string().required('Family name is required.')
});

const validationSchema = Yup.object().shape({
    circleId: Yup.string().required('Required field'),
    users: Yup.array().of(UserDtoSchema).min(1, "At least one user must be selected.").required('Required field.')
})

const isUserDto = (option: string | UserDto): option is UserDto => {
    return typeof option !== 'string';
};

export default function AddUsersDialog({ open, setOpen }: Props) {
    const { userStore, circleStore } = useStore();
    const [circleValue, setCircleValue] = useState<string>('');
    const [inputValue, setInputValue] = useState('');
    const [options, setOptions] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [loadingMain, setLoadingMain] = useState(false);
    const [isSuccess, setIsSuccess] = useState(false);
    circleStore.getCirclesByUser();
    const circles: CircleDto[] = [...circleStore.circlesMap.values()].filter(circle => circle.creatorId === userStore.user?.id!)
    const handleClose = () => {
        setOpen(false);
    };

    const handleIsSuccess = () => {
        if (!circleStore.errorMap.has('addUsers')) {
            setIsSuccess(true);
        } else {
            setIsSuccess(false);
        }
        setLoadingMain(false);
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

    return (
        <Dialog
            open={open}
            onClose={handleClose}>
            {loadingMain && <div className="loader-div"><Loader text="Loading..." className="loader-dialog" /></div>}
            <div style={
                {
                    display: 'flex ',
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                }
            }>
                <DialogTitle >Add Users to circle</DialogTitle>
                <IconButton style={{ padding: '16px 24px' }} onClick={handleClose}>
                    <FontAwesomeIcon icon={faTimes} />
                </IconButton>
            </div>
            <DialogContent>
                <DialogContentText style={{ paddingBottom: '10px' }}>
                    Select Circle from the dropdovn and then select users you want to add to that circle.
                </DialogContentText>
                <Formik
                    initialValues={{ circleId: '', users: [] as UserDto[] }}
                    onSubmit={async (values) => {
                        setLoadingMain(true);
                        await circleStore.addUsers(
                            {
                                circleId: values.circleId,
                                inviterId: userStore.user?.id!,
                                users: values.users.map(user => user.id)
                            }
                        )
                        handleIsSuccess();
                    }}
                    validationSchema={validationSchema}>
                    {({ errors, touched }) => (
                        <Form>
                            <Box sx={{
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'stretch',
                                gap: '1rem',
                                margin: '1rem'
                            }}>
                                {isSuccess && <Alert severity="success">Users added successfully</Alert>}
                                {circleStore.errorMap.has('addUsers') && <Alert severity="error">{circleStore.errorMap.get('addUsers')}</Alert>}
                                <Field name="circleId">
                                    {({ form }: FieldProps) => (
                                        <FormControl sx={{ mminWidth: 120, width: "100%" }} size="small">
                                            <InputLabel id="circle-select">Select Circle</InputLabel>
                                            <Select
                                                id="circle-select"
                                                labelId="circle-select"
                                                label="Select Circle"
                                                value={circleValue}
                                                variant="outlined"
                                                onChange={(e) => {
                                                    setCircleValue(e.target.value);
                                                    form.setFieldValue('circleId', e.target.value);
                                                }}
                                                error={!!errors.circleId && touched.circleId}
                                                displayEmpty
                                            >
                                                {
                                                    circles.map(circle => (
                                                        <MenuItem key={circle.id} value={circle.id}>{circle.name}</MenuItem>
                                                    ))
                                                }
                                            </Select>
                                        </FormControl>
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
                                                    error={!!(errors.users && touched.users)}
                                                    helperText={touched.users && typeof errors.users === 'string' ? errors.users : ''}
                                                    InputProps={{
                                                        ...params.InputProps,
                                                        endAdornment: (
                                                            <>
                                                                {loading ? <CircularProgress color="inherit" size={20} /> : null}
                                                                {params.InputProps.endAdornment}
                                                            </>
                                                        ),
                                                    }}
                                                />
                                            )}
                                        />
                                    )}
                                </Field>
                                <Button variant="contained" type="submit" >Add Users</Button>
                            </Box>
                        </Form>
                    )}
                </Formik>
            </DialogContent>
        </Dialog>
    )
}