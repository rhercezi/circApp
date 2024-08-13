import React from "react";
import CircleStore from "./circleStore";
import UserStore from "./userStore";

interface Store {
    userStore: UserStore
    circleStore: CircleStore
}

export const store: Store = {
    userStore: new UserStore(),
    circleStore: new CircleStore()
}

export const StoreContext = React.createContext(store);

export function useStore() {
    return React.useContext(StoreContext);
}