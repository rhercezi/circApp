import { makeAutoObservable } from "mobx";
import { UserDto } from "../api/dtos/UserDto";
import apiClient from "../api/apiClient";
import TokenDto from "../api/dtos/TokenDto";

export default class UserStore {
    token: string | undefined;
    refreshToken: string | undefined;


    constructor() {
        makeAutoObservable(this);
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
            let data = await apiClient.Users.login(username, password);
            this.token = data.data.accessToken;
            this.refreshToken = data.data.refreshToken;
            console.log(this.token);
        } catch (error) {
            console.error(error);
        }
    }

    refresh = async (token: string) => {
        try {
            let data = await apiClient.Users.refresh(token);
            this.token = data.data.accessToken;
            this.refreshToken = data.data.refreshToken;
        } catch (error) {
            console.error(error);
        }
    }
}