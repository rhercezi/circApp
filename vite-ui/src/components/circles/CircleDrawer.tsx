import { Box, List } from "@mui/material";
import DrawerButton from "../common/buttonst/DrawerButton";
import CreateCircleDialog from "./CreateCircleDialog";
import { useState } from "react";

interface Props {
    setOpenCirclesDrower: (openDrower: boolean) => void;
}

export default function CircleDrawer({setOpenCirclesDrower}: Props) {
    const handleCirclesDrower = (openDrower: boolean) => () => {
        setOpenCirclesDrower(openDrower);
    };

    const [openCreateCircle, setOpenCreateCircle] = useState(false);
    const handleOpenCreateCircle = () => {
        setOpenCreateCircle(true);
    }
    const handleCloseCreateCircle = () => {
        setOpenCreateCircle(false);
    }


    return (
        <Box className='circle-drower-inner' role='presentation'>
            <List>
                <DrawerButton variant="outlined" onClick={handleOpenCreateCircle}>Create Circle</DrawerButton>
            </List>
            <CreateCircleDialog open={openCreateCircle} setOpen={handleCloseCreateCircle} />
        </Box>
    )
}