import { NotificationType } from "./NotificationType"

export interface NotificationDto {
    Id: string
    UserId: string
    IsRead: boolean
    Body: Body
}

export interface Body {
    TargetId: string
    Message: string
    Time: string
    Type: NotificationType
}