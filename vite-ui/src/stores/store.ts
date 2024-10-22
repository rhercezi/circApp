import React from "react";
import CircleStore from "./circleStore";
import UserStore from "./userStore";
import AppointmentStore from "./appointmentStore";
import TaskStore from "./taskStore";

interface Store {
    userStore: UserStore
    circleStore: CircleStore
    appointmentStore: AppointmentStore
    taskStore: TaskStore
}

export const store: Store = {
    userStore: new UserStore(),
    circleStore: new CircleStore(),
    appointmentStore: new AppointmentStore(),
    taskStore: new TaskStore()
}

export const StoreContext = React.createContext(store);

export function useStore() {
    return React.useContext(StoreContext);
}