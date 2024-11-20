import { Alert, Box, Button, TextField, InputAdornment, IconButton } from "@mui/material";
import { Field, Form, Formik, FieldProps } from "formik";
import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import Loader from "../common/Loader";
import { NavLink, useNavigate } from "react-router-dom";
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import { useState } from "react";

export default observer(function Login() {
    const { userStore, circleStore } = useStore();
    const navigate = useNavigate();
    const [showPassword, setShowPassword] = useState(false);

    const handleClickShowPassword = () => {
        setShowPassword(!showPassword);
    };

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    if (userStore.loading) {
        return <Loader text={userStore.loaderText} className='loader' />;
    }

    return (
        <div className="control-container">
            <Formik
                initialValues={{ username: '', password: '' }}
                onSubmit={async (values) => {
                    await userStore.login(values.username, values.password);
                    if (userStore.isLoggedIn) {
                        await circleStore.setUserId(userStore.user?.id!);
                        navigate('/dashboard');
                    }
                }}>
                {({ handleSubmit, isSubmitting }) => (
                    <Form onSubmit={handleSubmit}>
                        <Box sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'stretch',
                            gap: '1rem'
                        }}>
                            {userStore.errorMap.has('login') && <Alert severity="error">{userStore.errorMap.get('login')}</Alert>}

                            {userStore.isLoggedIn && <Alert severity="info">You are already logged in</Alert>}

                            <Field name="username">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Username"
                                        type="text"
                                        fullWidth
                                    />
                                )}
                            </Field>

                            <Field name="password">
                                {({ field }: FieldProps) => (
                                    <TextField
                                        {...field}
                                        label="Password"
                                        type={showPassword ? 'text' : 'password'}
                                        fullWidth
                                        InputProps={{
                                            endAdornment: (
                                                <InputAdornment position="end">
                                                    <IconButton
                                                        aria-label="toggle password visibility"
                                                        onClick={handleClickShowPassword}
                                                        onMouseDown={handleMouseDownPassword}
                                                        edge="end"
                                                    >
                                                        {showPassword ? <VisibilityOff /> : <Visibility />}
                                                    </IconButton>
                                                </InputAdornment>
                                            )
                                        }}
                                    />
                                )}
                            </Field>

                            <Button
                                variant="contained"
                                type="submit"
                                disabled={isSubmitting}
                            >
                                Login
                            </Button>

                            <NavLink to='/reset-password'>
                                Forgot your password?
                            </NavLink>
                        </Box>
                    </Form>
                )}
            </Formik>
        </div>
    );
});