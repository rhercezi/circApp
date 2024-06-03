import { makeAutoObservable } from "mobx";
import { CircleDto } from "../api/dtos/CircleDto";
import apiClient from "../api/apiClient";


export default class CircleStore {
    circlesMap = new Map<string, CircleDto>();

    constructor() {
        makeAutoObservable(this);
    }

    getCirclesByUSer = async () => {
        try {
            let data = await apiClient.Circles.getCirclesByUser('3d7164e7-4dbb-4bb2-bef2-04b28b7e6036');
            data.circles.forEach((circle: CircleDto) => {
                this.circlesMap.set(circle.circleId, circle);
            });
        } catch (error) {
            console.error(error);
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