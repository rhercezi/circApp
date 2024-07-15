import { makeAutoObservable, runInAction } from "mobx";
import { UserDto } from "../api/dtos/user_dtos/UserDto";
import apiClient from "../api/apiClient";
import { PasswordUpdateDto } from "../api/dtos/user_dtos/PasswordUpdateDto";

export default class UserStore {
    user: UserDto | undefined = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!) : undefined;
    loading: boolean = false;
    isLoggedIn: boolean = false;
    loaderText: string = '';
    errorMap = new Map<string, string>();
    isSuccess: boolean = false;



    constructor() {
        makeAutoObservable(this);

        if (this.user) {
            this.isLoggedIn = true;
        } 
    }

    createUser = async (user: UserDto) => {
        this.loaderText = 'Setting new user...';
        this.loading = true;
        this.isSuccess = false;
        try {
            await apiClient.Users.create(user);
            runInAction(() => {
                this.errorMap.delete('signup');
                this.isSuccess = true;
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('signup', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('signup', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
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

    updatePassword = async (data: PasswordUpdateDto) => {
        try {
            await apiClient.Users.updatePassword(data);
        } catch (error) {
            console.error(error);
        }
    }

    resetPassword = async (data: PasswordUpdateDto) => {
        this.loaderText = 'Resetting password...';
        this.loading = true;
        try {
            await apiClient.Users.resetPassword(data);
            runInAction(() => {
                this.errorMap.delete('resetPwd');
            });
        } catch (error) {
            runInAction(() => {
                this.errorMap.set('resetPwd', 'Something went wrong, please try again later.');
            });
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    resetPasswordRequest = async (username: string) => {
        this.loaderText = 'Requesting password reset...';
        this.loading = true;
        this.isSuccess = false;
        try {
            await apiClient.Users.resetPasswordRequest(username);
            runInAction(() => {
                this.errorMap.delete('resetRqst');
                this.isSuccess = true;
            });
        } catch (error: any) {
            if (error.response && error.response.status === 422) {
                runInAction(() => this.errorMap.set('resetRqst', error.response.data));
            } else {
                runInAction(() => this.errorMap.set('resetRqst', 'Something went wrong, please try again later.'));
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    verifyEmail = async (id: string) => {

        await apiClient.Users.verifyEmail(id);

    }

    login = async (username: string, password: string) => {
        this.loaderText = 'Logging in...';
        this.loading = true;
        try {
            let data = await apiClient.Users.login(username, password);
            runInAction(() => {
                this.user = data.data;
                localStorage.setItem('user', JSON.stringify(data.data));
                this.errorMap.delete('login');
                this.isLoggedIn = true;
            });
        } catch (error: any) {
            runInAction(() => {
                this.isLoggedIn = false;
                if (error.response && error.response.status === 401) {
                    this.errorMap.set('login', 'Invalid username or password');
                } else {
                    this.errorMap.set('login', 'Something went wrong, please try again later.');
                }
            });
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    logout = async () => {
        try {
            this.loaderText = 'Logging out...';
            this.loading = true;
            await apiClient.Users.Logout();
            runInAction(() => {
                this.user = undefined;
                localStorage.removeItem('user');
                this.isLoggedIn = false;
            });
        } catch (error) {
            console.error(error);
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }
}
