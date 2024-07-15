import { Box, List } from "@mui/material";
import DrawerButton from "../common/buttonst/DrawerButton";
import CreateCircleDialog from "./CreateCircleDialog";
import AddUsersDialog from "./AddUsersDialog";
import RemoveUsersDialog from "./RemoveUsersDiaog";

interface Props {
    setOpenCirclesDrower: (openDrower: boolean) => void;
    setElementChange: (elementChange: [string, boolean]) => void;
}

export default function CircleDrawer({ setElementChange }: Props) {

    const handleOpen = (name: string, isOpen: boolean) => {
        setElementChange([name, isOpen])
    }


    return (
        <Box sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "right",
          alignItems: "end",
          minWidth: "250px",
          width: "15vw"
        }} >
            <List sx={
                {
                    display: "flex",
                    flexDirection: "column",
                    width: "100%",
                    gap: "1rem"
                }
            }>
                <DrawerButton variant="outlined" onClick={() => handleOpen(CreateCircleDialog.name, true)}>Create Circle</DrawerButton>
                <DrawerButton variant="outlined" onClick={() => handleOpen(AddUsersDialog.name, true)}>Add Users</DrawerButton>
                <DrawerButton variant="outlined" onClick={() => handleOpen(RemoveUsersDialog.name, true)}>Remove Users</DrawerButton>
            </List>
        </Box>
    )
}