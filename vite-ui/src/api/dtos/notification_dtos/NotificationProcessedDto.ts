import { EventCommandType } from "./EventCommandType";

export interface NotificationProcessedDto {
    NotificationId: string;
    CommandType: EventCommandType;
}