import { makeAutoObservable, runInAction } from "mobx";
import apiClient from "../api/apiClient";
import { NotificationDto } from "../api/dtos/notification_dtos/NotificationDto";

export default class EventStore {
    events: NotificationDto[] = [];
    errorMap = new Map<string, string>();
    loading: boolean = false;
    socket: WebSocket | null = null;

    constructor() {
        makeAutoObservable(this);
        this.connect();
    }

    sendMessage = (message: string) => {
        if (this.socket && this.socket.readyState === WebSocket.OPEN) {
            this.socket.send(message);
        }
    }

    handleMessage = async (message: NotificationDto) => {
        runInAction(() => {
            this.events.push(message);
        });
    }

    connect = async () => {
        this.events = [];
        const { socket, sendMessage } = apiClient.Socket.connect(this.handleMessage);
        this.socket = socket;
        this.sendMessage = sendMessage;
        while (this.socket.readyState !== WebSocket.OPEN) {
            await new Promise((resolve) => setTimeout(resolve, 100));
        }
        this.sendMessage("");
    }

    disconnect = () => {
        if (this.socket) {
            this.socket.close();
        }
    }

    removeNotification = (id: string) => {
        this.sendMessage(id);
        runInAction(() => {
            this.events = this.events.filter((event) => event.id !== id);
        });
    }
}
