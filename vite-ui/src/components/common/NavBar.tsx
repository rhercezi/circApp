import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import NavBarNLI from "./NavBarNLI";
import NavBarLoggedIn from "./NavBarLoggedIn";
import { Divider } from "@mui/material";

export default observer(function NavBar() {
    const { userStore } = useStore();

    if (!userStore.isLoggedIn) {
        return (
            <div className="nav-bar-container">
                <NavBarNLI />
                <Divider />
            </div>
        )
    }

    return (
        <div className="nav-bar-container">
            <NavBarLoggedIn />
            <Divider />
        </div>
    );
})
