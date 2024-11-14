import { Tooltip, IconButton, styled, css } from "@mui/material";
import { NotificationDto } from "../../api/dtos/notification_dtos/NotificationDto";
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import { NotificationType } from "../../api/dtos/notification_dtos/NotificationType";
import { NotificationProcessedDto } from "../../api/dtos/notification_dtos/NotificationProcessedDto";
import { EventCommandType } from "../../api/dtos/notification_dtos/EventCommandType";

interface Props {
    notification: NotificationDto;
    removeNotification: (command: NotificationProcessedDto, comandType: NotificationType) => void;
}

const Notification = ({ notification, removeNotification }: Props) => {
    const navigate = useNavigate();

    const handleNotificationClick = () => {
        switch (notification.Body.Type) {
            case NotificationType.JoinRequest:
                navigate('/settings/#join-request');
                removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.DeleteNotifications }, NotificationType.JoinRequest);
                break;
            case NotificationType.Appointment:
                navigate(`/view/appointment/${notification.Body.TargetId}`);
                removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.DeleteNotifications }, NotificationType.Appointment);
                break;
            case NotificationType.Task:
                navigate(`/task/${notification.Body.TargetId}`);
                removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.DeleteNotifications }, NotificationType.Task);
                break;
            case NotificationType.Reminder:
                navigate(`/view/appointment/${notification.Body.TargetId}`);
                removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.MarkReminderAsSeen }, NotificationType.Reminder);
                break;
            default:
                console.log('Unknown notification type');
        }
    };

    const handleCloseClick = () => {
        if (notification.Body.Type === NotificationType.Reminder) {
            removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.MarkReminderAsSeen }, NotificationType.Reminder);
        } else if (notification.Body.Type === NotificationType.JoinRequest) {
            removeNotification({ NotificationId: notification.Id, CommandType: EventCommandType.DeleteNotifications }, NotificationType.JoinRequest);
        }
    };

    return (
        <div className="notification">
            <Tooltip title="">
                <StyledNotificationDiv onClick={handleNotificationClick}>
                    {notification && notification.Body && notification.Body.Message}
                </StyledNotificationDiv>
            </Tooltip>
            <Tooltip title="Close Notification">
                <IconButton onClick={handleCloseClick}>
                    <FontAwesomeIcon icon={notification.Body.Type === NotificationType.Reminder? faBan : faTrashAlt} size="xs" />
                </IconButton>
            </Tooltip>
        </div>
    );
};

export default Notification;

const StyledNotificationDiv = styled('div')(
    ({ theme }) => css`
            width: 100%;
            padding: 0.25rem 1rem 0.25rem 0;
            border-right: 1px solid ${theme.palette.mode === 'dark' ? grey[50] : grey[900]};
            &:hover {
                background-color: ${theme.palette.mode === 'dark' ? grey[800] : grey[200]};
                cursor: pointer;
            }
        `,
);

const grey = {
    50: 'rgba(255, 255, 255, 0.12)',
    100: '#E5EAF2',
    200: '#DAE2ED',
    300: '#C7D0DD',
    400: '#B0B8C4',
    500: '#9DA8B7',
    600: '#6B7A90',
    700: '#434D5B',
    800: '#303740',
    900: '#1C2025',
};