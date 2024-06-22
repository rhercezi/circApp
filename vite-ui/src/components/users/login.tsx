import { Box, Button, TextField } from "@mui/material";
import { Field, Form, Formik, FieldProps } from "formik";
import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";


export default observer(function Login() {
    const {userStore} = useStore();

    return (
        <Formik
            initialValues={{ username: '', password: '' }}
            onSubmit={(values) => userStore.login(values.username, values.password)}>
            {({handleSubmit}) => (
                <Form onSubmit={handleSubmit}>
                    <Box sx={{display: 'flex', 
                            flexDirection: 'column',
                            justifyContent: 'stretch',
                            gap: '1rem'}}>
                        <Field name="username">
                        {
                            ({ field }: FieldProps) => (
                                <TextField
                                    {...field}
                                    label="Username"
                                    type="text"
                                />
                            )
                        }
                        </Field>

                        <Field name="password">
                        {
                            ({ field }: FieldProps) => (
                                <TextField
                                    {...field}
                                    label="Password"
                                    type="password"
                                />
                            )
                        }
                        </Field>

                        <Button variant="contained" type="submit" >Login</Button>
                    </Box>
                </Form>
            )}

        </Formik>
    );
})