import React from "react";
import CircleStore from "./circleStore";
import UserStore from "./userStore";

interface Store {
    circleStore: CircleStore
    userStore: UserStore
}

export const store: Store = {
    circleStore: new CircleStore(),
    userStore: new UserStore()
}

export const StoreContext = React.createContext(store);

export function useStore() {
    return React.useContext(StoreContext);
}