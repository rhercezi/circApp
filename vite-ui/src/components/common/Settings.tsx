import { Divider } from "@mui/material";
import CirclesSettings from "../circles/CirclesSettings";
import JoinRequests from "../circles/JoinRequests";

export default function Settings() {
    return (
        <div className="profile-container">
            <CirclesSettings />
            <Divider />
            <JoinRequests />
        </div>
    )
}