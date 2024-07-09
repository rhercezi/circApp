import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { Field, FieldProps, Form, Formik } from "formik";
import { UserDto } from "../../api/dtos/UserDto";
import uuid from "react-uuid";
import * as Yup from 'yup';
import { Alert, Box, Button, TextField } from "@mui/material";
import { useNavigate } from "react-router-dom";

const validationSchema = Yup.object().shape({
    userName: Yup.string().required('Required field')
        .min(3, 'Must be at least 3 characters')
        .max(20, 'Must be at most 20 characters'),
    firstName: Yup.string().required('Required field')
        .min(3, 'Must be at least 3 characters')
        .max(20, 'Must be at most 20 characters'),
    familyName: Yup.string().required('Required field')
        .min(3, 'Must be at least 3 characters')
        .max(20, 'Must be at most 20 characters'),
    email: Yup.string().required('Required field')
        .email('Invalid email address'),
    password: Yup.string().required('Required field')
        .min(8, 'Must be at least 8 characters')
        .max(20, 'Must be at most 20 characters')
        .matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$/,
            'Must contain at least one uppercase letter, one lowercase letter, one number, ' +
            'and one special character and must be between 8-20 characters'),
    confirmPassword: Yup.string().required('Required field')
        .oneOf([Yup.ref('password')], 'Passwords must match')
})

export default observer(function Signup() {
    const { userStore } = useStore();
    const navigate = useNavigate();

    if (userStore.isSuccess) {
        return (
            <Alert severity="info">Validation link was sent to your email.
                Please chech you email and follow validation link,
                after validating your email you will be able to log in.</Alert>
        )
    }

    return (
        <>
            <Formik
                initialValues={{
                    userName: '',
                    firstName: '',
                    familyName: '',
                    email: '',
                    password: '',
                    confirmPassword: ''
                }}
                onSubmit={async (values) => {
                    const userDto: UserDto = {
                        id: uuid(),
                        oldPassword: '',
                        idLink: '',
                        ...values
                    };
                    await userStore.createUser(userDto).then(() => {
                        if (!userStore.errorMap.has('signup')) {
                            navigate('/login');
                        }
                    });
                }}
                validationSchema={validationSchema}>
                {({ errors, touched }) => (
                    <Form>
                        <Box sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'stretch',
                            gap: '1rem'
                        }}>
                            {userStore.errorMap.has('signup') && <Alert severity="error">{userStore.errorMap.get('signup')}</Alert>}
                            <Field name="userName">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Username"
                                        type="text"
                                        error={!!errors.userName && touched.userName}
                                        helperText={errors.userName && touched.userName ? errors.userName : ''}
                                    />
                                )}
                            </Field>

                            <Field name="firstName">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="First Name"
                                        type="text"
                                        error={!!errors.firstName && touched.firstName}
                                        helperText={errors.firstName && touched.firstName ? errors.firstName : ''}
                                    />
                                )}
                            </Field>

                            <Field name="familyName">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Family Name"
                                        type="text"
                                        error={!!errors.familyName && touched.familyName}
                                        helperText={errors.familyName && touched.familyName ? errors.familyName : ''}
                                    />
                                )}
                            </Field>

                            <Field name="email">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Email"
                                        type="email"
                                        error={!!errors.email && touched.email}
                                        helperText={errors.email && touched.email ? errors.email : ''}
                                    />
                                )}
                            </Field>

                            <Field name="password">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Password"
                                        type="password"
                                        error={!!errors.password && touched.password}
                                        helperText={errors.password && touched.password ? errors.password : ''}
                                    />
                                )}
                            </Field>

                            <Field name="confirmPassword">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Confirm Password"
                                        type="password"
                                        error={!!errors.confirmPassword && touched.confirmPassword}
                                        helperText={errors.confirmPassword && touched.confirmPassword ? errors.confirmPassword : ''}
                                    />
                                )}
                            </Field>

                            <Button variant="contained" type="submit" >Create User</Button>
                        </Box>
                    </Form>
                )}
            </Formik>
        </>
    )
});