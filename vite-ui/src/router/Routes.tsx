import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../App";
import Login from "../components/users/Login";
import Signup from "../components/users/Signup";
import ResetPasswordForm from "../components/users/ResetPasswordForm";
import ResetPassword from "../components/users/ResetPassword";
import VerifyEmail from "../components/users/VerifyEmail";
import Dashboard from "../components/common/Dashboard";
import Profile from "../components/users/Profile";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: 'login', element: <Login />},
            {path: 'signup', element: <Signup />},
            {path: 'verify-email/:id', element: <VerifyEmail />},
            {path: 'reset-password', element: <ResetPasswordForm />},
            {path: 'reset-password/:id', element: <ResetPassword />},
            {path: 'dashboard', element: <Dashboard />},
            {path: 'profile', element: <Profile />}
        ]
    }
]

export const router = createBrowserRouter(routes);    