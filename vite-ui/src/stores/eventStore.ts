import { makeAutoObservable, runInAction } from "mobx";
import apiClient from "../api/apiClient";
import { NotificationDto } from "../api/dtos/notification_dtos/NotificationDto";
import { NotificationType } from "../api/dtos/notification_dtos/NotificationType";
import { NotificationProcessedDto } from "../api/dtos/notification_dtos/NotificationProcessedDto";

export default class EventStore {
    events: NotificationDto[] = [];
    reminders: NotificationDto[] = [];
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
            if (message.Body.Type === NotificationType.Reminder) {
                if (this.reminders.some(reminder => reminder.Id === message.Id)) {
                    return;
                }
                this.reminders.push(message);
            } else {
                this.events.push(message);
            }
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

    removeNotification = (command: NotificationProcessedDto, commandType: NotificationType) => {
        runInAction(() => {
            this.sendMessage(JSON.stringify(command));
            if (commandType === NotificationType.Reminder) {
                this.reminders = this.reminders.filter((reminder) => reminder.Id !== command.NotificationId);
            } else {
                this.events = this.events.filter((event) => event.Id !== command.NotificationId);
            }
        });
    }
}
