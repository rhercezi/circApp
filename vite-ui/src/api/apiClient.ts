import axios, { AxiosRequestConfig, AxiosResponse } from "axios"
import { UserDto } from "./dtos/user_dtos/UserDto";
import { CircleDto } from "./dtos/circle_dtos/CircleDto";
import { AppointmentDto, DetachedDetailsDto } from "./dtos/appointment_dtos/AppointmentDto";
import { TaskDto } from "./dtos/task_dtos/TaskDto";
import { CirclesByUserDto } from "./dtos/circle_dtos/CirclesByUserDto";
import { UsersByCircleDto } from "./dtos/circle_dtos/UsersByCircleDto";
import { PasswordUpdateDto } from "./dtos/user_dtos/PasswordUpdateDto";
import { UsersDto } from "./dtos/user_dtos/UsersDto";
import { AddUsersDto } from "./dtos/circle_dtos/AddUsersDto";
import { RemoveUsersDto } from "./dtos/circle_dtos/RemoveUsersDto";
import { RequestDto } from "./dtos/circle_dtos/RequestDto";
import { ConfirmJoinDto } from "./dtos/circle_dtos/ConfirmJoinDto";
import { CompleteTaskDto } from "./dtos/task_dtos/CompleteTaskDto";
import { LoginDto } from "./dtos/user_dtos/LoginDto";
import { RefreshDto } from "./dtos/user_dtos/RefreshDto";
import { NotificationDto } from "./dtos/notification_dtos/NotificationDto";

axios.defaults.baseURL = "http://localhost:5018";

const body = <T>(response: AxiosResponse<T>) => response.data;

const requests = {

    get: <T>(url: string, config?: AxiosRequestConfig) => axios.get<T>(url, config).then(body),
    post: <T>(url: string, body: any, config?: AxiosRequestConfig) => axios.post<T>(url, body, config).then(body),
    put: (url: string, body: any, config?: AxiosRequestConfig) => axios.put(url, body, config).then(body),
    patch: (url: string, body: any, config?: AxiosRequestConfig) => axios.patch(url, body, config).then(body),
    delete: (url: string, config?: AxiosRequestConfig) => axios.delete(url, config).then(body)

}

const Users = {

    get: (id: string) => requests.get<UserDto>(`/v1/user/by-id/${id}`),
    create: (body: UserDto) => requests.post('/v1/user/create', body),
    update: (id: string, jsonPatch: string) => requests.patch(`/v1/user/${id}`, jsonPatch, { headers: { 'Content-Type': 'application/json-patch+json' } }),
    delete: (id: string, password: string) => requests.delete('/v1/user/', { params: { id, password } }),
    verifyEmail: (idLink: string) => requests.post(`/v1/user/verify-email/${idLink}`, {}),
    updatePassword: (body: PasswordUpdateDto) => requests.put('/v1/user/password', body),
    resetPassword: (body: PasswordUpdateDto) => requests.put('/v1/user/password-id-link', body),
    login: (username: string, password: string) => requests.post<LoginDto>('/v1/auth', { username, password }),
    refresh: () => requests.post<RefreshDto>('/v1/auth/token', {}),
    Logout: () => requests.post('/v1/auth/logout', {}),
    resetPasswordRequest: (username: string) => requests.put('/v1/user/password/reset', { username }),

}

const Circles = {

    getCirclesByUser: (id: string) => requests.get<CirclesByUserDto>(`/v1/circles/${id}`),
    getUsersInCircle: (id: string) => requests.get<UsersByCircleDto>(`/v1/circles/users/${id}`),
    searchUsers: (search: string) => requests.get<UsersDto>(`/v1/circles/search/${search}`),
    getJoinRequests: (userId: string) => requests.get<RequestDto[]>(`/v1/circles/join-requests/${userId}`),
    create: (body: CircleDto) => requests.post('/v1/circles', body),
    update: (id: string, jsonPatch: string) => requests.patch(`/v1/circles/${id}`, jsonPatch, { headers: { 'Content-Type': 'application/json-patch+json' } }),
    delete: (id: string) => requests.delete('/v1/circles', { params: { id } }),
    confirmJoin: (body: ConfirmJoinDto) => requests.post('/v1/circles/confirm', body),
    addUsers: (body: AddUsersDto) => requests.post('/v1/circles/add-users', body),
    removeUsers: (body: RemoveUsersDto) => requests.post('/v1/circles/remove-users', body)

}

const Appointments = {

    create: (body: AppointmentDto) => requests.post('/v1/appointments', body),
    update: (id: string, jsonPatch: string, userId: string) => requests.patch(`/v1/appointments/${id}`, jsonPatch, { headers: { 'Content-Type': 'application/json-patch+json', 'userId': `${userId}` } }),
    delete: (id: string, userId: string) => requests.delete(`/v1/appointments/${id}`, { headers : { 'userId': `${userId}` } }),
    createDetails: (body: DetachedDetailsDto) => requests.post('/v1/appointments/details', body),
    updateDetails: (id: string, jsonPatch: string, userId: string) => requests.patch(`/v1/appointments/details/${id}`, jsonPatch, { headers: { 'Content-Type': 'application/json-patch+json', 'userId': `${userId}` } }),
    deleteDetails: (id: string, userId: string) => requests.delete(`/v1/appointments/details/${id}`, { headers : { 'userId': `${userId}` } }),
    getByCircle: (id: string, dateFrom: Date, dateTo: Date) => requests.get<AppointmentDto[]>(`/v1/appointments/circle/${id}`, {
        params: { dateFrom, dateTo }
    }),
    getByUser: (id: string, dateFrom: Date, dateTo: Date) => requests.get<AppointmentDto[]>(`/v1/appointments/user/${id}`, {
        params: { dateFrom, dateTo }
    }),
    getAppointment: (id: string, userId: string) => requests.get<AppointmentDto>(`/v1/appointments/${id}`, { headers : { 'userId': `${userId}` } })
}

const Tasks = {

    create: (body: TaskDto) => requests.post('/v1/tasks', body),
    update: (body: TaskDto) => requests.put('/v1/tasks', body),
    complete: (body: CompleteTaskDto) => requests.patch('/v1/tasks', body),
    delete: (id: string) => requests.delete(`/v1/tasks/${id}`),
    getByCircle: (id: string, includeCompleted: boolean) => requests.get<TaskDto[]>(`/v1/tasks/circle/${id}`, {
        params: { includeCompleted }
    }),
    getByUser: (id: string, includeCompleted: boolean, searchByCircles: boolean) => requests.get<TaskDto[]>(`/v1/tasks/user/${id}`, {
        params: { searchByCircles, includeCompleted }
    }),
    getById: (id: string) => requests.get<TaskDto>(`/v1/tasks/${id}`)

}

const Socket = {
    connect: (onMessage: (message: NotificationDto) => void) => {
        const socket = new WebSocket('ws://localhost:5018/v1/event-socket');

        socket.onopen = () => {
            //console.log('WebSocket connection established');
        };

        socket.onmessage = (event) => {
            try {
                const data = JSON.parse(event.data);
                if (isNotificationDto(data)) {
                    onMessage(data);
                } else {
                    console.error('Received data does not match NotificationDto:', data);
                }
            } catch (error) {
                console.error('Error parsing WebSocket message:', error);
            }
        };

        socket.onclose = (event) => {
            if (event.wasClean) {
                console.log(`WebSocket connection closed cleanly, code=${event.code}, reason=${event.reason}`);
            } else {
                console.error('WebSocket connection closed unexpectedly');
            }
        };

        socket.onerror = (error) => {
            console.error('WebSocket error:', error);
        };

        const sendMessage = (message: string) => {
            if (socket.readyState === WebSocket.OPEN) {
                socket.send(message);
            } else {
                console.error('WebSocket is not open. Ready state:', socket.readyState);
            }
        };

        return { socket, sendMessage };
    }
};

const isNotificationDto = (data: any): data is NotificationDto => {
    return typeof data === 'object' &&
        typeof data.Id === 'string' &&
        typeof data.UserId === 'string' &&
        typeof data.IsRead === 'boolean' &&
        typeof data.Body === 'object' &&
        typeof data.Body.TargetId === 'string' &&
        typeof data.Body.Message === 'string' &&
        typeof data.Body.Type === 'number';
};

axios.defaults.withCredentials = true;

const apiClient = {
    Users,
    Circles,
    Appointments,
    Tasks,
    Socket
}

export default apiClient;