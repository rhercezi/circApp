import { observer } from "mobx-react-lite";
import MonthOverview from "../appointments/MonthOverview";
import { useLocation } from "react-router-dom";
import DayOverview from "../appointments/DayOverview"
import EditAppointment from "../appointments/EditAppointment";

const Dashboard = () => {
    const location = useLocation();
    const path = location.pathname;

    if (path === "/dashboard") {
        return <MonthOverview />;
    } else if (path.startsWith("/dashboard/day")) {
        return <DayOverview />;
    } else if (path.startsWith("/dashboard/appointment")) {
        return <EditAppointment />;
    }
};

export default observer(Dashboard);