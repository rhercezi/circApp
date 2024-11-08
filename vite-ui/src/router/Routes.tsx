import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../App";
import Login from "../components/users/Login";
import Signup from "../components/users/Signup";
import ResetPasswordForm from "../components/users/ResetPasswordForm";
import ResetPassword from "../components/users/ResetPassword";
import VerifyEmail from "../components/users/VerifyEmail";
import Dashboard from "../components/common/Dashboard";
import Profile from "../components/users/Profile";
import Settings from "../components/common/Settings";
import CreateAppointment from "../components/appointments/CreateAppointment";
import CreateTask from "../components/tasks/CreateTask";
import TaskOverview from "../components/tasks/TaskOverview";
import ErrorPage from "../components/common/ErrorPage";
import EditAppointment from "../components/appointments/EditAppointment";
import ProtectedRoute from "./ProtectedRute";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            { path: 'login', element: <Login /> },
            { path: 'signup', element: <Signup /> },
            { path: 'verify-email/:id', element: <VerifyEmail /> },
            { path: 'reset-password', element: <ResetPasswordForm /> },
            { path: 'reset-password/:id', element: <ResetPassword /> },
            {
                path: 'dashboard',
                element: (
                    <ProtectedRoute>
                        <Dashboard />
                    </ProtectedRoute>
                )
            },
            {
                path: 'dashboard/day',
                element: (
                    <ProtectedRoute>
                        <Dashboard />
                    </ProtectedRoute>
                )
            },
            {
                path: 'dashboard/appointment/:id',
                element: (
                    <ProtectedRoute>
                        <Dashboard />
                    </ProtectedRoute>
                )
            },
            {
                path: 'view/appointment/:id',
                element: (
                    <ProtectedRoute>
                        <EditAppointment />
                    </ProtectedRoute>
                )
            },
            {
                path: 'profile',
                element: (
                    <ProtectedRoute>
                        <Profile />
                    </ProtectedRoute>
                )
            },
            {
                path: 'settings',
                element: (
                    <ProtectedRoute>
                        <Settings />
                    </ProtectedRoute>
                )
            },
            {
                path: 'create',
                element: (
                    <ProtectedRoute>
                        <CreateAppointment />
                    </ProtectedRoute>
                )
            },
            {
                path: 'tasks',
                element: (
                    <ProtectedRoute>
                        <TaskOverview />
                    </ProtectedRoute>
                )
            },
            {
                path: 'task/:id',
                element: (
                    <ProtectedRoute>
                        <TaskOverview />
                    </ProtectedRoute>
                )
            },
            {
                path: 'tasks/new',
                element: (
                    <ProtectedRoute>
                        <CreateTask />
                    </ProtectedRoute>
                )
            },
            {
                path: 'tasks/edit/:id',
                element: (
                    <ProtectedRoute>
                        <CreateTask />
                    </ProtectedRoute>
                )
            },
            { path: '*', element: <ErrorPage /> }
        ]
    }
];

export const router = createBrowserRouter(routes);