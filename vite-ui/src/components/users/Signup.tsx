import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { Field, FieldProps, Form, Formik } from "formik";
import { UserDto } from "../../api/dtos/user_dtos/UserDto";
import uuid from "react-uuid";
import * as Yup from 'yup';
import { Alert, Box, Button, Checkbox, css, FormControlLabel, IconButton, Link, styled, TextField } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import { useState } from "react";

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
    const [tncAgreed, setTncAgreed] = useState(false);
    const [TnC, setTnC] = useState(false);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTncAgreed(event.target.checked);
    };
    const handleClickTnc = (event: React.MouseEvent<HTMLElement>) => {
        event.stopPropagation();
        setTnC(!TnC);
    };

    const handleCloseTnc = () => {
        setTnC(false);
    };

    if (userStore.isSuccess) {
        return (
            <Alert severity="info">Validation link was sent to your email.
                Please check your email and follow the validation link,
                after validating your email you will be able to log in.</Alert>
        )
    }

    return (
        <div className="flex-box">
            <div className="content-mid-div">
                <StyledPopperDiv TnC={TnC}>
                    <div>
                        <IconButton
                            aria-label="close"
                            onClick={handleCloseTnc}
                            sx={{
                                position: 'absolute',
                                right: 8,
                                top: 8,
                                color: (theme) => theme.palette.grey[500],
                            }}
                        >
                            <CloseIcon />
                        </IconButton>
                        <h2>Terms and Conditions</h2>
                        <h3>1. Introduction</h3>
                        <p>This application ("CircleApp") is developed as part of a Bachelor's thesis project. By accessing or using the App, you agree to be bound by these Terms and Conditions ("Terms"). If you do not agree with any part of the Terms, please do not use the App.</p>
                        <h3>2. Use of the App</h3>
                        <p>The App is provided for educational and informational purposes only. You may use the App freely, but you acknowledge and agree that its use is at your own risk.</p>
                        <h3>3. Disclaimer of Warranties</h3>
                        <p>The App is provided "as is" and "as available," without any representations or warranties of any kind, express or implied. To the fullest extent permitted by law, the developer disclaims all warranties, including but not limited to, implied warranties of merchantability, fitness for a particular purpose, and non-infringement.</p>
                        <h3>4. Limitation of Liability</h3>
                        <p>In no event shall the developer be liable for any damages arising out of or related to the use or inability to use the App, including but not limited to, direct, indirect, incidental, punitive, or consequential damages. This limitation applies even if the developer has been advised of the possibility of such damages.</p>
                        <h3>5. No Endorsement</h3>
                        <p>Any opinions, findings, or conclusions expressed in this App are those of the developer and do not necessarily reflect the views of any affiliated institution or organization.</p>
                        <h3>6. Changes to the App</h3>
                        <p>The developer reserves the right to modify, suspend, or discontinue the App at any time, without prior notice. The developer also reserves the right to update or modify these Terms at any time.</p>
                        <h3>7. Governing Law</h3>
                        <p>These Terms shall be governed by and construed in accordance with the laws of Austria. Any disputes arising out of or relating to these Terms or the use of the App shall be subject to the exclusive jurisdiction of the courts in Graz, Austria.</p>
                        <h3>8. Contact Information</h3>
                        <p>If you have any questions or concerns about these Terms, please contact the developer at circles.app@aol.com.</p>
                    </div>
                </StyledPopperDiv>
            </div>
            <div className="control-container">
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
                        await userStore.createUser(userDto);
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

                                <Button variant="contained" type="submit" disabled={!tncAgreed} >Create User</Button>
                                <FormControlLabel
                                    control={<Checkbox checked={tncAgreed} onChange={handleChange} />}
                                    label={
                                        <span>
                                            I agree to the{' '}
                                            <Link type="button" component="button" variant="body2" onClick={handleClickTnc} >
                                                Terms and Conditions
                                            </Link>
                                        </span>
                                    }
                                />
                            </Box>
                        </Form>
                    )}
                </Formik>
            </div>
        </div>
    )
});

const grey = {
    50: '#F3F6F9',
    100: '#E5EAF2',
    200: '#DAE2ED',
    300: '#C7D0DD',
    400: '#B0B8C4',
    500: '#9DA8B7',
    600: '#6B7A90',
    700: '#434D5B',
    800: '#303740',
    900: '#1C2025',
};


const StyledPopperDiv = styled('div')<{ TnC: boolean }>(
    ({ theme, TnC }) => css`
      border-radius: 8px;
      background-color: ${theme.palette.mode === 'dark' ? '#121212' : grey[50]};
      display: ${TnC ? 'block' : 'none'};
      position: relative;
      top: 1rem;
      right: 1rem;
      border: 1px solid ${theme.palette.mode === 'dark' ? grey[700] : grey[200]};
      box-shadow: ${theme.palette.mode === 'dark'
            ? `0px 4px 8px rgb(0 0 0 / 0.7)`
            : `0px 4px 8px rgb(0 0 0 / 0.1)`};
      padding: 0.75rem;
      color: ${theme.palette.mode === 'dark' ? grey[100] : grey[700]};
      font-size: 0.875rem;
      font-weight: 500;
      opacity: 1;
      margin: 0.75rem 0;
      width: 100%;
      max-height: 80%;
      overflow-y: scroll;
      z-index: 9;
    `,
);