import { PasswordUpdateDto } from "../../api/dtos/user_dtos/PasswordUpdateDto";
import { useStore } from "../../stores/store";
import Loader from "../common/Loader";
import { Field, FieldProps, Form, Formik } from "formik";
import { Alert, Box, Button, TextField, Tooltip } from "@mui/material";
import * as Yup from 'yup';

const validationSchema = Yup.object().shape({
    password: Yup.string().required('Required field')
        .min(8, 'Must be at least 8 characters')
        .max(20, 'Must be at most 20 characters')
        .matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$/,
            'Must contain at least one uppercase letter, one lowercase letter, one number, ' +
            'and one special character and must be between 8-20 characters'),
    confirmPassword: Yup.string().required('Required field')
        .oneOf([Yup.ref('password')], 'Passwords must match'),
    oldPassword: Yup.string().required('Required field')
})

export default function UpdatePassword() {

    const { userStore } = useStore();

    if (userStore.loading) {
        return (
            <Loader text={userStore.loaderText} className='loader' />
        )
    }

    return (
        <div className="profile-element">
            <Formik
                initialValues={{ password: '', confirmPassword: '', oldPassword: '' }}
                onSubmit={async (values) => {
                    const passwordUpdateDto: PasswordUpdateDto = {
                        id: userStore.user!.id,
                        oldPassword: values.oldPassword,
                        password: values.password,
                        idLink: ''
                    };
                    userStore.updatePassword(passwordUpdateDto);
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

                            {userStore.errorMap.has('Pwd') && <Alert severity="error">{userStore.errorMap.get('resetPwd')}</Alert>}


                            <Field name="password">
                                {({ field }: FieldProps) => (
                                    <Tooltip title="Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character and must be between 8-20 characters">
                                        <TextField
                                            {...field}
                                            label="Password"
                                            type="password"
                                            error={!!errors.password && touched.password}
                                            helperText={errors.password && touched.password ? errors.password : ''}
                                        />
                                    </Tooltip>
                                )}
                            </Field>


                            <Field name="confirmPassword">
                                {({ field }: FieldProps) => (
                                    <Tooltip title="Passwords must match">
                                        <TextField
                                            {...field}
                                            label="Confirm Password"
                                            type="password"
                                            error={!!errors.confirmPassword && touched.confirmPassword}
                                            helperText={errors.confirmPassword && touched.confirmPassword ? errors.confirmPassword : ''}
                                        />
                                    </Tooltip>
                                )}
                            </Field>

                            <Field name="oldPassword">
                                {({ field }: FieldProps) => (
                                    <Tooltip title="Please provide your password">
                                        <TextField
                                            {...field}
                                            label="Old Password"
                                            type="password"
                                            error={!!errors.confirmPassword && touched.confirmPassword}
                                            helperText={errors.confirmPassword && touched.confirmPassword ? errors.confirmPassword : ''}
                                        />
                                    </Tooltip>
                                )}
                            </Field>

                            <Button variant="contained" type="submit" >Update Password</Button>
                        </Box>
                    </Form>
                )}
            </Formik>
        </div>
    );
}