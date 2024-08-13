import { makeAutoObservable, runInAction } from "mobx";
import { CircleDto } from "../api/dtos/circle_dtos/CircleDto";
import apiClient from "../api/apiClient";
import { AddUsersDto } from "../api/dtos/circle_dtos/AddUsersDto";
import { RemoveUsersDto } from "../api/dtos/circle_dtos/RemoveUsersDto";


export default class CircleStore {
    circlesMap = new Map<string, CircleDto>();
    userId: string | undefined = undefined;
    errorMap = new Map<string, string>();
    loading: boolean = false;

    sleep = (ms: number) => {
        return new Promise(resolve => setTimeout(resolve, ms));

    }

    constructor() {
        makeAutoObservable(this);
    }

    setUserId = async (userId: string) => {
        if (userId) {
            this.userId = userId;
            this.getCirclesByUser();
        }
    }

    getCirclesByUser = async () => {
        try {
            let data = await apiClient.Circles.getCirclesByUser(this.userId!);
            data.circles.forEach((circle: CircleDto) => {
                this.circlesMap.set(circle.id, circle);
            });
        } catch (error) {
            console.error(error);
        }
    }

    getUsersInCircle = async (circleId: string) => {
        try {
            let data = await apiClient.Circles.getUsersInCircle(circleId);
            return data.users;
        } catch (error) {
            console.error(error);
        }
    }

    searchUsers = async (search: string) => {
        try {
            let data = await apiClient.Circles.searchUsers(search);
            return data.users;
        } catch (error) {
            console.error(error);
        }
    }

    createCircle = async (circle: CircleDto) => {
        this.loading = true;
        try {
            await apiClient.Circles.create(circle);
            runInAction(() => {
                this.errorMap.delete('createCircle');
                this.getCirclesByUser();
            });
        } catch (error: any) {
            runInAction(() => {
                if (error.response && error.response.status === 422) {
                    this.errorMap.set('createCircle', error.response.data);
                } else {
                    this.errorMap.set('createCircle', "Something went wrong, please try again later.");
                }
            });
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    updateCircle = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.update(circle);
            this.circlesMap.set(circle.id, circle);
        } catch (error) {
            console.error(error);
        }
    }

    deleteCircle = async (circleId: string) => {
        try {
            await apiClient.Circles.delete(circleId);
            runInAction(() => {
                this.circlesMap.delete(circleId);
                this.getCirclesByUser();
            });
        } catch (error) {
            console.error(error);
        }
    }

    confirmJoin = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.confirmJoin(circle);
            this.circlesMap.set(circle.id, circle);
        } catch (error) {
            console.error(error);
        }
    }

    addUsers = async (users: AddUsersDto) => {
        this.loading = true;
        try {
            await apiClient.Circles.addUsers(users);
            runInAction(() => {
                this.errorMap.delete('addUsers');
            })
        } catch (error: any) {
            if (error.response && error.response.status === 422) {
                runInAction(() => this.errorMap.set('addUsers', error.response.data));
            } else {
                runInAction(() => this.errorMap.set('addUsers', 'Something went wrong, please try again later.'));
            }
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    removeUsers = async (dto: RemoveUsersDto) => {
        this.loading = true;
        try {
            await this.sleep(2000);
            console.log(dto);
            await apiClient.Circles.removeUsers(dto);
            runInAction(() => {
                this.errorMap.delete('removeUsers');
            })
        } catch (error: any) {
            if (error.response && error.response.status === 422) {
                runInAction(() => this.errorMap.set('removeUsers', error.response.data));
            } else {
                runInAction(() => this.errorMap.set('removeUsers', 'Something went wrong, please try again later.'));
            }
        } finally {
            runInAction(() => this.loading = false);
        }
    }
}