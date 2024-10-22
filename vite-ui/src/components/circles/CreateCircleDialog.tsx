import { Alert, Autocomplete, Box, Button, CircularProgress, Dialog, DialogContent, DialogContentText, DialogTitle, IconButton, TextField } from "@mui/material";
import { Field, FieldProps, Form, Formik } from "formik";
import { MuiColorInput } from "mui-color-input";
import { useStore } from "../../stores/store";
import uuid from "react-uuid";
import * as Yup from 'yup';
import { useEffect, useState } from "react";
import { UserDto } from "../../api/dtos/user_dtos/UserDto";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons/faTimes";
import Loader from "../common/Loader";

interface Props {
    open: boolean;
    setOpen: (open: boolean) => void;
}

const isUserDto = (option: string | UserDto): option is UserDto => {
    return typeof option !== 'string';
};

const validationSchema = Yup.object().shape({
    circleName: Yup.string().required('Required field')
        .min(3, 'Must be at least 3 characters')
        .max(20, 'Must be at most 20 characters'),
    circleColor: Yup.string().required('Required field')
        .matches(/^#([0-9A-F]{3}){1,2}$/i, 'Invalid color format')
})

export default function CreateCircleDialog({ open, setOpen }: Props) {
    const { circleStore, userStore } = useStore();
    const [color, setColor] = useState<string>('#000000');
    const [inputValue, setInputValue] = useState('');
    const [options, setOptions] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [loadingMain, setLoadingMain] = useState(false);
    const [isSuccess, setIsSuccess] = useState(false);
    const handleClose = () => {
        if (!circleStore.errorMap.has('createCircle')) {
            setOpen(false);
        }
    };

    const handleIsSuccess = () => {
        if (!circleStore.errorMap.has('createCircle')) {
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
        <>
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
                    <DialogTitle>Create Circle</DialogTitle>
                    <IconButton style={{ padding: '16px 24px' }} onClick={handleClose}>
                        <FontAwesomeIcon icon={faTimes} />
                    </IconButton>
                </div>
                <DialogContent>
                    <DialogContentText style={{ paddingBottom: '10px' }}>
                        To create a Circle, please add Circle name, color and select members. You will be added automatically.
                    </DialogContentText>
                    <Formik
                        initialValues={{
                            circleName: '',
                            circleColor: '',
                            users: [] as UserDto[]
                        }}
                        onSubmit={async (values) => {
                            setLoadingMain(true);
                            await circleStore.createCircle(
                                {
                                    id: uuid(),
                                    creatorId: userStore.user?.id!,
                                    userId: '',
                                    inviterId: userStore.user?.id!,
                                    name: values.circleName,
                                    color: values.circleColor,
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
                                    {isSuccess && <Alert severity="success">You have successfully created new circle</Alert>}
                                    {circleStore.errorMap.has('createCircle') && <Alert severity="error">{circleStore.errorMap.get('createCircle')}</Alert>}
                                    <Field name="circleName">
                                        {({ field }: FieldProps) => (
                                            <TextField
                                                {...field}
                                                label="Circle Name"
                                                type="text"
                                                error={!!errors.circleName && touched.circleName}
                                                helperText={errors.circleName && touched.circleName ? errors.circleName : ''}
                                                onKeyDown={() => {
                                                    setIsSuccess(false);
                                                    circleStore.errorMap.delete('createCircle');
                                                }}
                                            />
                                        )}
                                    </Field>
                                    <Field name="circleColor">
                                        {({ form }: FieldProps) => (
                                            <MuiColorInput
                                                value={color}
                                                onChange={(newColor) => {
                                                    setColor(newColor);
                                                    form.setFieldValue('circleColor', newColor);
                                                    setIsSuccess(false);
                                                    circleStore.errorMap.delete('createCircle');
                                                }}
                                                format="hex"
                                                label="Circle Color"
                                                error={!!errors.circleColor && touched.circleColor}
                                                helperText={errors.circleColor && touched.circleColor ? errors.circleColor : ''}
                                            />
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
                                                    option.userName! + ', ' + option.firstName! + ', ' + option.familyName!
                                                    : option}
                                                getOptionKey={(option) => isUserDto(option) ? option.id! : option}
                                                onInputChange={(_, newInputValue) => {
                                                    setInputValue(newInputValue);
                                                }}
                                                onChange={(_, newValue) => {
                                                    form.setFieldValue('users', newValue);
                                                    setIsSuccess(false);
                                                    circleStore.errorMap.delete('createCircle');
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
                                                    />
                                                )}
                                            />
                                        )}
                                    </Field>
                                    <Button variant="contained" type="submit" >Create Circle</Button>
                                </Box>
                            </Form>
                        )}
                    </Formik>
                </DialogContent>
            </Dialog>
        </>
    )
}