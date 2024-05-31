import axios, { Axios, AxiosResponse } from "axios"
import { UserDto } from "./dtos/UserDto";
import { CircleDto } from "./dtos/CircleDto";
import { AppointmentDto } from "./dtos/AppointmentDto";
import { TaskDto } from "./dtos/TaskDto";
import { CompleteTaskDto } from "./dtos/CompleteTaskDto";

axios.defaults.baseURL = "http://localhost:5028";

const body = (response: AxiosResponse<any, any>) => response.data;

const requests = {

    get: (url: string, config?: any) => axios.get(url, config).then(body),
    post: (url: string, body: any) => axios.post(url, body).then(body),
    put: (url: string, body: any) => axios.put(url, body).then(body),
    patch: (url: string, body: any) => axios.patch(url, body).then(body),
    delete: (url: string, config?: any) => axios.delete(url, config).then(body)

}

const Users = {

    create: (body: UserDto) => requests.post('/v1/user', body),
    update: (body: UserDto) => requests.patch('/v1/user', body),
    delete: (id: string, password: string) => requests.delete('/v1/user/', { params: { id, password } }),
    updatePassword: (body: UserDto) => requests.patch('/v1/user/password', body),
    login: (username: string, password: string) => requests.post('/v1/auth', { params: { username, password } }),
    refresh: (token: string) => requests.post('/v1/auth/token', { params: { token } })

}

const Circles = {

    getCirclesByUser: (id: string) => requests.get(`/v1/circles/${id}`),
    getUsersInCircle: (id: string) => requests.get(`/v1/circles/users/${id}`),
    searchUsers: (search: string) => requests.get(`/v1/circles/search/${search}`),
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
    getByCircle: (id: string, dateFrom: string, dateTo: string) => requests.get(`/v1/appointments/circle/${id}`, { params: { dateFrom, dateTo } }),
    getByUser: (id: string, dateFrom: string, dateTo: string) => requests.get(`/v1/appointments/user/${id}`, { params: { dateFrom, dateTo } })

}

const Tasks = {
    
        create: (body: TaskDto) => requests.post('/v1/tasks', body),
        update: (body: TaskDto) => requests.put('/v1/tasks', body),
        complete: (body: CompleteTaskDto) => requests.patch('/v1/tasks', body),
        delete: (id: string) => requests.delete(`/v1/tasks/${id}`),
        getByCircle: (id: string) => requests.get(`/v1/tasks/circle/${id}`),
        getByUser: (id: string) => requests.get(`/v1/tasks/user/${id}`)

}

const apiClient = {
    Users,
    Circles,
    Appointments,
    Tasks
}

export default apiClient;