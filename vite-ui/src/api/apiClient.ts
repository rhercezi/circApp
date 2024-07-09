import axios, { AxiosResponse } from "axios"
import { UserDto } from "./dtos/UserDto";
import { CircleDto } from "./dtos/CircleDto";
import { AppointmentDto } from "./dtos/AppointmentDto";
import { TaskDto } from "./dtos/TaskDto";
import { CompleteTaskDto } from "./dtos/CompleteTaskDto";
import { CirclesByUserDto } from "./dtos/CirclesByUserDto";
import { UsersByCircleDto } from "./dtos/UsersByCircleDto";
import { PasswordUpdateDto } from "./dtos/PasswordUpdateDto";

axios.defaults.baseURL = "http://localhost:5018";

const body = <T>(response: AxiosResponse<T>) => response.data;

const requests = {

    get: <T>(url: string, config?: any) => axios.get<T>(url, config).then(body),
    post: <T>(url: string, body: any) => axios.post<T>(url, body).then(body),
    put: (url: string, body: any) => axios.put(url, body).then(body),
    patch: (url: string, body: any) => axios.patch(url, body).then(body),
    delete: (url: string, config?: any) => axios.delete(url, config).then(body)

}

const Users = {

    get: (id: string) => requests.get<UserDto>(`/v1/user/by-id/${id}`),
    create: (body: UserDto) => requests.post('/v1/user/create', body),
    update: (body: UserDto) => requests.patch('/v1/user', body),
    delete: (id: string, password: string) => requests.delete('/v1/user/', { params: { id, password } }),
    verifyEmail: (idLink: string) => requests.post(`/v1/user/verify-email/${idLink}`,{}),
    updatePassword: (body: PasswordUpdateDto) => requests.patch('/v1/user/password', body),
    resetPassword: (body: PasswordUpdateDto) => requests.patch('/v1/user/password-id-link', body),
    login: (username: string, password: string) => requests.post<UserDto>('/v1/auth', { username, password }),
    refresh: () => requests.post('/v1/auth/token', {}),
    Logout: () => requests.post('/v1/auth/logout', {}),
    resetPasswordRequest: (username: string) => requests.post('/v1/user/password/reset', { username }),

}

const Circles = {

    getCirclesByUser: (id: string) => requests.get<CirclesByUserDto>(`/v1/circles/${id}`),
    getUsersInCircle: (id: string) => requests.get<UsersByCircleDto>(`/v1/circles/users/${id}`),
    searchUsers: (search: string) => requests.get<CirclesByUserDto[]>(`/v1/circles/search/${search}`),
    create: (body: CircleDto) => requests.post('/v1/circles', body),
    update: (body: CircleDto) => requests.patch('/v1/circles', body),
    delete: (id: string) => requests.delete('/v1/circles', { params: { id } }),
    confirmJoin: (body: CircleDto) => requests.post('/v1/circles/confirm', body),
    addUsers: (body: CircleDto) => requests.post('/v1/circles/addusers', body),
    removeUsers: (body: CircleDto) => requests.post('/v1/circles/removeusers', body)

}

const Appointments = {

    create: (body: AppointmentDto) => requests.post('/v1/appointments', body),
    update: (body: AppointmentDto) => requests.patch('/v1/appointments', body),
    delete: (id: string) => requests.delete(`/v1/appointments/${id}`),
    createDetails: (body: AppointmentDto) => requests.post('/v1/appointments/details', body),
    updateDetails: (body: AppointmentDto) => requests.patch('/v1/appointments/details', body),
    deleteDetails: (id: string) => requests.delete(`/v1/appointments/details/${id}`),
    getByCircle: (id: string, dateFrom: string, dateTo: string) => requests.get<AppointmentDto[]>(`/v1/appointments/circle/${id}`, {
        params: { dateFrom, dateTo }
    }),
    getByUser: (id: string, dateFrom: string, dateTo: string) => requests.get<AppointmentDto[]>(`/v1/appointments/user/${id}`, {
        params: { dateFrom, dateTo }
    })

}

const Tasks = {

    create: (body: TaskDto) => requests.post('/v1/tasks', body),
    update: (body: TaskDto) => requests.put('/v1/tasks', body),
    complete: (body: CompleteTaskDto) => requests.patch('/v1/tasks', body),
    delete: (id: string) => requests.delete(`/v1/tasks/${id}`),
    getByCircle: (id: string) => requests.get<TaskDto[]>(`/v1/tasks/circle/${id}`),
    getByUser: (id: string, searchByCircles: boolean) => requests.get<TaskDto[]>(`/v1/tasks/user/${id}`, {
        params: { searchByCircles }
    })

}

axios.defaults.withCredentials = true;
axios.interceptors.response.use(
    response => response,
    async error => {
        if (error.response.status === 401 && error.response.data !== 'Invalid username or password') {
            try {
                await apiClient.Users.refresh();
                return axios.request(error.config);
            } catch (refreshError) {
                console.error(refreshError);
                window.location.href = '/login';
            }
        }
        return Promise.reject(error);
    }
);

const apiClient = {
    Users,
    Circles,
    Appointments,
    Tasks
}

export default apiClient;