import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import NavBarNLI from "./NavBarNLI";
import NavBarLoggedIn from "./NavBarLoggedIn";

export default observer(function NavBar() {
    const { userStore } = useStore();

    if (!userStore.isLoggedIn) {
        return (
            <NavBarNLI />
        )
    }

    return (
        <NavBarLoggedIn />
    );
})
