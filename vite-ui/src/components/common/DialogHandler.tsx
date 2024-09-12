import { useEffect, useState } from "react";
import CreateCircleDialog from "../circles/CreateCircleDialog";
import AddUsersDialog from "../circles/AddUsersDialog";
import RemoveUsersDialog from "../circles/RemoveUsersDiaog";

export interface Props {
    elementChange: [string, boolean];
}

export default function DialogHandler({ elementChange }: Props) {

    const [openCreateCircle, setOpenCreateCircle] = useState(false);
    const [openAddUsers, setOpenAddUsers] = useState(false);
    const [openRemoveUsers, setOpenRemoveUsers] = useState(false);

    useEffect(() => {
        if (elementChange[0] === CreateCircleDialog.name) {
            setOpenCreateCircle(elementChange[1]);
        }
        if (elementChange[0] === AddUsersDialog.name) {
            setOpenAddUsers(elementChange[1]);
        }
        if (elementChange[0] === RemoveUsersDialog.name) {
            setOpenRemoveUsers(elementChange[1]);
        }
    }, [elementChange]);

    return (
        <>
            <CreateCircleDialog open={openCreateCircle} setOpen={setOpenCreateCircle} />
            <AddUsersDialog open={openAddUsers} setOpen={setOpenAddUsers} />
            <RemoveUsersDialog open={openRemoveUsers} setOpen={setOpenRemoveUsers} />
        </>
    )
}