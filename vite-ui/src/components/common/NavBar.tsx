import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import NavBarNLI from "./NavBarNLI";
import NavBarLoggedIn from "./NavBarLoggedIn";
import { Divider } from "@mui/material";
import { runInAction } from "mobx";

export default observer(function NavBar() {
    const { userStore, circleStore, eventStore } = useStore();

    if (!userStore.isLoggedIn) {
        return (
            <div className="nav-bar-container">
                <NavBarNLI />
                <Divider />
            </div>
        )
    }
    else
    {
        runInAction(() => {
            circleStore.setUserId(userStore.user?.id!);
            eventStore.connect();
        });
    }

    return (
        <div className="nav-bar-container">
            <NavBarLoggedIn />
            <Divider />
        </div>
    );
})
