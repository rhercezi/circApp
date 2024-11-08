import React from "react";
import CircleStore from "./circleStore";
import UserStore from "./userStore";
import AppointmentStore from "./appointmentStore";
import TaskStore from "./taskStore";
import EventStore from "./eventStore";

interface Store {
    userStore: UserStore
    circleStore: CircleStore
    appointmentStore: AppointmentStore
    taskStore: TaskStore
    eventStore: EventStore
}

export const store: Store = {
    userStore: new UserStore(),
    circleStore: new CircleStore(),
    appointmentStore: new AppointmentStore(),
    taskStore: new TaskStore(),
    eventStore: new EventStore()
}

export const StoreContext = React.createContext(store);

export function useStore() {
    return React.useContext(StoreContext);
}