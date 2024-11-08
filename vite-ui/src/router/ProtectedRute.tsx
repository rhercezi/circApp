import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useStore } from '../stores/store';

interface ProtectedRouteProps {
    children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
    const { userStore } = useStore();
    const location = useLocation();

    if (!userStore.user) {
        return <Navigate to="/login" state={{ from: location }} />;
    }

    return <>{children}</>;
};

export default ProtectedRoute;