import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Alert, Box, Button, Chip, Dialog, DialogContent, DialogContentText, DialogTitle, FormControl, IconButton, InputLabel, MenuItem, OutlinedInput, Select, TextField } from "@mui/material";
import { Field, FieldProps, Form, Formik, setIn } from "formik";
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

const validationSchema = Yup.object().shape({
    circleId: Yup.string().required('Required field'),
    users: Yup.array().of(Yup.string()).min(1, "At least one user must be selected.").required('Required field.')
})

export default function RemoveUsersDialog({ open, setOpen }: Props) {
    const { userStore, circleStore } = useStore();
    const [circleValue, setCircleValue] = useState<string>('');
    const [inputValue, setInputValue] = useState<string[]>([]);
    const [options, setOptions] = useState<UserDto[]>([]);
    const [loadingMain, setLoadingMain] = useState(false);
    const [isSuccess, setIsSuccess] = useState(false);
    const circles: CircleDto[] = [...circleStore.circlesMap.values()].filter(circle => circle.creatorId === userStore.user?.id!)
    const handleClose = () => {
        setOpen(false);
    };

    const handleIsSuccess = () => {
        if (!circleStore.errorMap.has('removeUsers')) {
            setIsSuccess(true);
        } else {
            setIsSuccess(false);
        }
        setLoadingMain(false);
    };

    useEffect(() => {
        (async () => {
            if (circleValue) {
                const users = await circleStore.getUsersInCircle(circleValue);
                setOptions(users?.filter(x => x.id !== userStore.user!.id) || []);
            }
        })();
    }, [circleValue]);

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
                <DialogTitle>Add Users to circle</DialogTitle>
                <IconButton style={{ padding: '16px 24px' }} onClick={handleClose}>
                    <FontAwesomeIcon icon={faTimes} />
                </IconButton>
            </div>
            <DialogContent>
                <DialogContentText style={{ paddingBottom: '10px' }}>
                    Select Circle from the dropdovn and then select users you want to add to that circle.
                </DialogContentText>
                <Formik
                    initialValues={{ circleId: '', users: [] as string[] }}
                    onSubmit={async (values) => {
                        setLoadingMain(true);
                        await circleStore.removeUsers(
                            {
                                circleId: values.circleId,
                                users: values.users
                            }
                        );
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
                                {circleStore.errorMap.has('removeUsers') && <Alert severity="error">{circleStore.errorMap.get('removeUsers')}</Alert>}
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
                                                    setInputValue([]);
                                                    setIsSuccess(false);
                                                    circleStore.errorMap.delete('removeUsers');
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
                                        <FormControl sx={{ minWidth: 120, width: "100%" }}>
                                            <InputLabel id="demo-multiple-users-label">Select Users</InputLabel>
                                            <Select
                                                labelId="demo-multiple-users-label"
                                                id="demo-multiple-users"
                                                multiple
                                                value={inputValue}
                                                onChange={(e) => {
                                                    setInputValue(e.target.value as string[]);
                                                    form.setFieldValue('users', e.target.value);
                                                    setIsSuccess(false);
                                                    circleStore.errorMap.delete('removeUsers');
                                                }}
                                                input={<OutlinedInput id="select-multiple-users" label="Users" />}
                                                renderValue={(selected) => (
                                                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                                                        {selected.map((id) => {
                                                            let user = options.find(option => option.id === id);
                                                            if (user) {
                                                                return (
                                                                    <Chip key={user.id} label={user.firstName + ' ' + user.familyName + ' (' + user.userName + ')'} />
                                                                )
                                                            }
                                                        })}
                                                    </Box>
                                                )}
                                            >
                                                {options.map((user) => (
                                                    <MenuItem
                                                        key={user.id}
                                                        value={user.id}
                                                    >
                                                        {user.firstName + ' ' + user.familyName + ' (' + user.userName + ')'}
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    )}
                                </Field>
                                <Button variant="contained" type="submit" >Remove Users</Button>
                            </Box>
                        </Form>
                    )}
                </Formik>
            </DialogContent>
        </Dialog>
    )
}