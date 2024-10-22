import { makeAutoObservable, runInAction } from "mobx";
import { TaskDto } from "../api/dtos/task_dtos/TaskDto";
import apiClient from "../api/apiClient";
import { CompleteTaskDto } from "../api/dtos/task_dtos/CompleteTaskDto";

export default class TaskStore {
    tasks: TaskDto[] = [];
    loading: boolean = false;
    loaderText: string = '';
    errorMap = new Map<string, string>();

    constructor() {
        makeAutoObservable(this);
    }

    createTask = async (task: TaskDto) => {
        try {
            await apiClient.Tasks.create(task);
            runInAction(() => {
                this.errorMap.delete('createTask');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('createTask', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('createTask', 'Something went wrong, please try again later.');
                });
            }
        }
    }

    updateTask = async (task: TaskDto) => {
        try {
            await apiClient.Tasks.update(task);
            runInAction(() => {
                this.errorMap.delete('updateTask');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('updateTask', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('updateTask', 'Something went wrong, please try again later.');
                });
            }
        }
    }

    deleteTask = async (taskId: string) => {
        this.loaderText = 'Deleting task...';
        this.loading = true;
        try {
            await apiClient.Tasks.delete(taskId);
            runInAction(() => {
                this.errorMap.delete('deleteTask');
                this.tasks = this.tasks.filter(task => task.id !== taskId);
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('deleteTask', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('deleteTask', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    completeTask = async (body: CompleteTaskDto) => {
        this.loaderText = 'Completing task...';
        this.loading = true;
        try {
            await apiClient.Tasks.complete(body);
            runInAction(() => {
                this.errorMap.delete('completeTask');
                const task = this.tasks.find(task => task.id === body.taskId);
                if (task && body.circleId) {
                    task.isCompleted = true;
                } else if (task && body.userId) {
                    const user = task.userModels?.find(user => user.id === body.userId);
                    if (user) {
                        user.isCompleted = true;
                    } else if (task.ownerId === body.userId) {
                        task.isCompleted = true;
                    }
                }
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('completeTask', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('completeTask', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    getTasks(userId: string, includeCompleted: boolean, searchByCircles: boolean): Promise<void>;
    getTasks(circleId: string, includeCompleted: boolean): Promise<void>;

    async getTasks(arg1: string, arg2: boolean, arg3?: boolean): Promise<void> {
        this.loaderText = 'Getting tasks...';
        this.loading = true;
        try {
            let data;
            if (typeof arg3 === 'boolean') {
                data = await apiClient.Tasks.getByUser(arg1, arg2, arg3);
            } else {
                data = await apiClient.Tasks.getByCircle(arg1, arg2);
            }
            runInAction(() => {
                this.tasks = data;
                this.errorMap.delete('getTasks');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('getTasks', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('getTasks', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }
}