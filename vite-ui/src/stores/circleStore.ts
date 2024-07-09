import { makeAutoObservable } from "mobx";
import { CircleDto } from "../api/dtos/CircleDto";
import apiClient from "../api/apiClient";
import { UserDto } from "../api/dtos/UserDto";


export default class CircleStore {
    circlesMap = new Map<string, CircleDto>();
    user: UserDto | undefined = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!) : undefined;
    createCircleFormOpen: boolean = false;

    constructor() {
        makeAutoObservable(this);

        if (this.user) {
            this.getCirclesByUSer();
        }
    }

    getCirclesByUSer = async () => {
        if (this.user) {
            try {
                let data = await apiClient.Circles.getCirclesByUser(this.user!.id);
                data.circles.forEach((circle: CircleDto) => {
                    this.circlesMap.set(circle.circleId, circle);
                });
            } catch (error) {
                console.error(error);
            }
            console.log(this.circlesMap);
        }
    }

    createCircle = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.create(circle);
            this.circlesMap.set(circle.circleId, circle);
        } catch (error) {
            console.error(error);
        }
    }

    updateCircle = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.update(circle);
            this.circlesMap.set(circle.circleId, circle);
        } catch (error) {
            console.error(error);
        }
    }

    deleteCircle = async (circleId: string) => {
        try {
            await apiClient.Circles.delete(circleId);
            this.circlesMap.delete(circleId);
        } catch (error) {
            console.error(error);
        }
    }

    confirmJoin = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.confirmJoin(circle);
            this.circlesMap.set(circle.circleId, circle);
        } catch (error) {
            console.error(error);
        }
    }

    addUsers = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.addUsers(circle);
            this.circlesMap.set(circle.circleId, circle);
        } catch (error) {
            console.error(error);
        }
    }

    removeUsers = async (circle: CircleDto) => {
        try {
            await apiClient.Circles.removeUsers(circle);
            this.circlesMap.set(circle.circleId, circle);
        } catch (error) {
            console.error(error);
        }
    }
}