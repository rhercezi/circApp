import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Divider, Slide } from "@mui/material";
import { TransitionProps } from "@mui/material/transitions";
import { observer } from "mobx-react-lite"
import React, { useEffect } from "react";
import { useStore } from "../../stores/store";
import Notification from "../notifications/Notification";

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement<any, any>;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

interface Props {
    open: boolean;
    setOpen: (value: boolean) => void;
}

const ReminderDialog = ({open, setOpen}: Props) => {
    const { eventStore } = useStore();
    const [reminders, setReminders] = React.useState(eventStore.reminders);
    const [itemsCount, setItemsCount] = React.useState(eventStore.reminders.length);

    const removeNotification = eventStore.removeNotification;

    useEffect(() => {
        setItemsCount(eventStore.reminders.length);
        setReminders(eventStore.reminders);
        setOpen(itemsCount > 0);
    }, [eventStore.reminders]);

    useEffect(() => {
        if (itemsCount > 0) {
            setOpen(true);
        } else {
            setOpen(false);
        }
    }, [itemsCount]);

    const handleClose = () => {
        setOpen(false);
    };

    return (
        <>
            <Dialog
                open={open}
                TransitionComponent={Transition}
                keepMounted
                onClose={handleClose}
                aria-describedby="Reminders dialog"
            >
                <DialogTitle>{'Unseen reminders:'}</DialogTitle>
                <DialogContent>
                    {reminders.map((event, index) => (
                        <div key={event.Id}>
                            {index > 0 && <Divider />}
                            <Notification key={event.Id} notification={event} removeNotification={removeNotification} />
                        </div>
                    ))}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Close</Button>
                </DialogActions>
            </Dialog>
        </>
    );
}

export default observer(ReminderDialog)