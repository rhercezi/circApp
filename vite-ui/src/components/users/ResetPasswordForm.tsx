import { Alert, Box, Button, TextField } from "@mui/material";
import { Field, Form, Formik, FieldProps } from "formik";
import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import Loader from "../common/Loader";
import * as Yup from 'yup';


const validationSchema = Yup.object().shape({
    username: Yup.string().required('Required field')
        .min(3, 'Must be at least 3 characters')
        .max(20, 'Must be at most 20 characters')
})


export default observer(function ResetPasswordForm() {
    const { userStore } = useStore();

    if (userStore.loading) {
        return (
            <Loader text={userStore.loaderText} className='loader' />
        )
    }

    return (
        <>
            <Formik
                initialValues={{ username: '', email: '' }}
                onSubmit={async (values) => {
                    userStore.resetPasswordRequest(values.username);
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

                            {userStore.errorMap.has('resetRqst') && <Alert severity="error">{userStore.errorMap.get('resetRqst')}</Alert>}

                            {userStore.isSuccess && <Alert severity="success">Password reset link sent to your email</Alert>}

                            <Field name="username">
                                {
                                    ({ field }: FieldProps) => (
                                        <TextField
                                            {...field}
                                            label="Username"
                                            type="text"
                                            error={!!errors.username && touched.username}
                                            helperText={errors.username && touched.username ? errors.username : ''}
                                        />
                                    )
                                }
                            </Field>

                            <Button variant="contained" type="submit" >Send</Button>
                        </Box>
                    </Form>
                )}
            </Formik>
        </>
    );
})