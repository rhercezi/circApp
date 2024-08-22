import { Field, FieldProps, Form, Formik } from "formik";
import { observer } from "mobx-react-lite";
import * as jsonpatch from "fast-json-patch";
import { useStore } from "../../stores/store";
import * as Yup from 'yup';
import { Alert, Box, Button, TextField } from "@mui/material";

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
        .email('Invalid email address')
})

export default observer(function UpdateUser() {
    const { userStore } = useStore();
    const initialValues = {
        userName: userStore.user?.userName,
        firstName: userStore.user?.firstName,
        familyName: userStore.user?.familyName,
        email: userStore.user?.email
    }

    return (
        <div className="profile-element">
            <Formik
                initialValues={initialValues}
                onSubmit={async (values: typeof initialValues) => {
                    const patchDocument = jsonpatch.compare(initialValues, values);
                    const jsonPatch = JSON.stringify(patchDocument);
                    userStore.updateUser(userStore.user!.id, jsonPatch);
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

                            <Button variant="contained" type="submit" >Update User</Button>
                        </Box>
                    </Form>
                )}
            </Formik>
        </div>
    );
});