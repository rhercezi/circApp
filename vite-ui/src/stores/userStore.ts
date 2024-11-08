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
    expiration: number = localStorage.getItem('exp') ? Number(localStorage.getItem('exp')) : 0;
    private refreshIntervalId: NodeJS.Timeout | null = null;



    constructor() {
        makeAutoObservable(this);

        this.setUser();

        if (this.user) {
            this.isLoggedIn = true;
        } else {
            this.isLoggedIn = false;
        }

        if (this.expiration) {
            this.startTokenRefreshTimer();
        }
    }

    setUser = async () => {
        if (!this.user && localStorage.getItem('user')) {
            this.user = JSON.parse(localStorage.getItem('user')!);
        }
    }

    getUser = async (id: string): Promise<UserDto | undefined> => {
        try {
            let data = await apiClient.Users.get(id);
            return data;
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('getUser', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('getUser', 'Something went wrong, please try again later.');
                });
            }
        }
        return undefined;
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

    updateUser = async (id: string, jsonPatch: string) => {
        this.loaderText = 'Updating user...';
        this.loading = true;
        this.isSuccess = false;
        try {
            let data = await apiClient.Users.update(id, jsonPatch);
            runInAction(() => {
                this.user = data.data;
                localStorage.setItem('user', JSON.stringify(data.data));
                this.errorMap.delete('updateUser');
                this.isSuccess = true;
            });
        } catch (error: any) {
            if (error.response && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('updateUser', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('updateUser', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
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
        this.loaderText = 'Updating password...';
        this.loading = true;
        this.isSuccess = false;
        try {
            await apiClient.Users.updatePassword(data);
            runInAction(() => {
                this.errorMap.delete('updatePwd');
                this.isSuccess = true;
            });
        } catch (error: any) {
            if (error.response && error.response.status === 422) {
                runInAction(() => this.errorMap.set('updatePwd', error.response.data));
            }
        } finally {	
            runInAction(() => {
                this.loading = false;
            });
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

    startTokenRefreshTimer = () => {
        this.stopTokenRefreshTimer();
        this.refreshIntervalId = setInterval(() => {
            const expirationTime = this.expiration * 1000;
            const currentTime = new Date().getTime();
            const timeLeft = expirationTime - currentTime;

            if (timeLeft <= 5000) {
                let data = apiClient.Users.refresh();
                runInAction(() => {
                    data.then(data => { 
                        this.expiration = data.data.exp;
                        localStorage.setItem('exp', data.data.exp.toString());
                        this.startTokenRefreshTimer();
                    });
                });
            }
        }, 1000);
    }

    stopTokenRefreshTimer = () => {
        if (this.refreshIntervalId) {
            clearInterval(this.refreshIntervalId);
            this.refreshIntervalId = null;
        }
    }

    login = async (username: string, password: string) => {
        this.loaderText = 'Logging in...';
        this.loading = true;
        try {
            let data = await apiClient.Users.login(username, password);
            runInAction(() => {
                this.user = data.data.user;
                this.expiration = data.data.exp;
                localStorage.setItem('exp', data.data.exp.toString());
                localStorage.setItem('user', JSON.stringify(data.data.user));
                this.errorMap.delete('login');
                this.isLoggedIn = true;
                this.startTokenRefreshTimer();
            });
        } catch (error: any) {
            runInAction(() => {
                this.isLoggedIn = false;
                if (error.response && error.response.status === 401 || error.response.status === 400) {
                    this.errorMap.set('login', error.response.data);
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
        } catch (error) {
            console.error(error);
        } finally {
            runInAction(() => {
                this.user = undefined;
                this.stopTokenRefreshTimer();
                localStorage.removeItem('user');
                localStorage.removeItem('exp');
                this.isLoggedIn = false;
                this.loading = false;
            });
        }
    }
}
