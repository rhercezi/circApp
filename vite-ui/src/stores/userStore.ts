import { makeAutoObservable, reaction, runInAction } from "mobx";
import { UserDto } from "../api/dtos/UserDto";
import apiClient from "../api/apiClient";

export default class UserStore {
    user: UserDto | undefined = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!) : undefined;
    loading: boolean = false;
    usr2: UserDto | undefined = undefined;


    constructor() {
        makeAutoObservable(this);
    }

    getUser = async (id: string) => {
        if (id) {
            try {
                let data = await apiClient.Users.get(id);
                runInAction(() => {
                    this.usr2 = data;
                });
            } catch (error) {
                console.error(error);
            }
        }
    }

    createUser = async (user: UserDto) => {
        try {
            await apiClient.Users.create(user);
        } catch (error) {
            console.error(error);
        }
    }

    updateUser = async (user: UserDto) => {
        try {
            await apiClient.Users.update(user);
        } catch (error) {
            console.error(error);
        }
    }

    deleteUser = async (userId: string, password: string) => {
        try {
            await apiClient.Users.delete(userId, password);
        } catch (error) {
            console.error(error);
        }
    }

    updatePassword = async (user: UserDto) => {
        try {
            await apiClient.Users.updatePassword(user);
        } catch (error) {
            console.error(error);
        }
    }

    login = async (username: string, password: string) => {
        try {
            this.loading = true;
            let data = await apiClient.Users.login(username, password);
            runInAction(() => {
                this.user = data.data;
                localStorage.setItem('user', JSON.stringify(data.data));
                this.loading = false;
            });
        } catch (error) {
            console.error(error);
        }
    }

    logout = async () => {
        try {
            await apiClient.Users.Logout();
            runInAction(() => {
                this.user = undefined;
                localStorage.removeItem('user');
        });
        } catch (error) {
            console.error(error);
        }
    }
}
