import React from "react";
import CircleStore from "./circleStore";
import UserStore from "./userStore";
import AppointmentStore from "./AppointmentStore";

interface Store {
    userStore: UserStore
    circleStore: CircleStore
    appointmentStore: AppointmentStore
}

export const store: Store = {
    userStore: new UserStore(),
    circleStore: new CircleStore(),
    appointmentStore: new AppointmentStore()
}

export const StoreContext = React.createContext(store);

export function useStore() {
    return React.useContext(StoreContext);
}