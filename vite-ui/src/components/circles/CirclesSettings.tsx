import { IconButton, styled, TableBody, TableCell, tableCellClasses, TableContainer, TableHead, TableRow, TextField, Tooltip } from "@mui/material";
import { Paper, Table } from "@mui/material";
import { useStore } from "../../stores/store";
import { RemoveUsersDto } from "../../api/dtos/circle_dtos/RemoveUsersDto";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheck, faSignOut, faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import { observer } from "mobx-react-lite";
import { MuiColorInput } from "mui-color-input";
import { useEffect, useState } from "react";
import * as jsonpatch from "fast-json-patch";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";

const StyledTableCell = styled(TableCell)(({ theme }) => ({
    [`&.${tableCellClasses.head}`]: {
        backgroundColor: theme.palette.common.black,
        color: theme.palette.common.white,
        width: 'auto'
    },
    [`&.${tableCellClasses.body}`]: {
        fontSize: 14,
        width: 'auto'
    },
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
    '&:nth-of-type(odd)': {
        backgroundColor: theme.palette.action.hover,
    },
    // hide last border
    '&:last-child td, &:last-child th': {
        border: 0,
    },
}));

const CirclesSettings = () => {
    const { userStore, circleStore } = useStore();
    const [nameErrors, setNameErrors] = useState<Map<string, string>>(new Map());
    const [colorErrors, setColorErrors] = useState<Map<string, string>>(new Map());
    const [circlesArray, setCirclesArray] = useState<CircleDto[]>(Array.from(circleStore.circlesMap.values()));

    useEffect(() => {
        setCirclesArray(Array.from(circleStore.circlesMap.values()));
    }, [circleStore.circlesMap]);

    function setColor(value: string, id: string) {
        setColorErrors(new Map(colorErrors.set(id, '')));
        const updatedCircles = circlesArray.map(circle => circle.id === id ? { ...circle, color: value } : circle);
        setCirclesArray(updatedCircles);
    }

    function setCircleName(value: string, id: string) {
        setNameErrors(new Map(nameErrors.set(id, '')));
        const updatedCircles = circlesArray.map(circle => circle.id === id ? { ...circle, name: value } : circle);
        setCirclesArray(updatedCircles);
    }

    async function saveChanges(id: string): Promise<void> {
        if (!/^#[0-9A-F]{6}$/i.test(circlesArray.find(circle => circle.id === id)!.color!)) {
            setColorErrors(new Map(colorErrors.set(id, 'Invalid color format')));
            return;
        }
        if (circlesArray.find(circle => circle.id === id)!.name!.trim() === '') {
            setNameErrors(new Map(nameErrors.set(id, 'Name cannot be empty')));
            return;
        }

        let circle = circleStore.circlesMap.get(id);
        let update = circlesArray.find(circle => circle.id === id);

        const patchDocument = jsonpatch.compare(circle!, update!);
        const jsonPatch = JSON.stringify(patchDocument);
        
        await circleStore.updateCircle(id, jsonPatch);
    }

    async function handleDeleteCircle(circleId: string): Promise<void> {
        await circleStore.deleteCircle(circleId);
        setCirclesArray(Array.from(circleStore.circlesMap.values()));
    }

    async function handleLeaveCircle(circleId: string): Promise<void> {
        const dto: RemoveUsersDto = {
            circleId: circleId,
            users: [userStore.user!.id]
        };
        await circleStore.removeUsers(dto);
        setCirclesArray(Array.from(circleStore.circlesMap.values()));
    }

    return (
        <div className="profile-element-column">
            <h3>Circles Settings</h3>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <StyledTableCell>Circle Name</StyledTableCell>
                            <StyledTableCell>Circle Color</StyledTableCell>
                            <StyledTableCell>Action</StyledTableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {circlesArray.map((circle) => (
                            <StyledTableRow key={circle.id}>
                                <StyledTableCell>
                                    <TextField
                                        label="Circle Name"
                                        type="text"
                                        value={circle.name}
                                        onChange={(e) => {
                                            setCircleName(e.target.value, circle.id);
                                        }}
                                        error={!!nameErrors.get(circle.id)}
                                        helperText={nameErrors.get(circle.id)}
                                    />
                                </StyledTableCell>
                                <StyledTableCell>
                                    <MuiColorInput
                                        value={circle.color || '#000000'}
                                        onChange={(newColor) => {
                                            setColor(newColor, circle.id);
                                        }}
                                        format="hex"
                                        label="Circle Color"
                                        error={!!colorErrors.get(circle.id)}
                                        helperText={colorErrors.get(circle.id)}
                                    /></StyledTableCell>
                                <StyledTableCell>
                                    <IconButton
                                        onClick={() => {
                                            if (circle.creatorId === userStore.user?.id) {
                                                handleDeleteCircle(circle.id);
                                            } else {
                                                handleLeaveCircle(circle.id);
                                            }
                                        }}>
                                        {
                                            circle.creatorId === userStore.user?.id ?
                                                (
                                                    <Tooltip title="Delete Circle">
                                                        <FontAwesomeIcon icon={faTrashAlt} size="xs" />
                                                    </Tooltip>
                                                ) :
                                                (
                                                    <Tooltip title="Leave Circle">
                                                        <FontAwesomeIcon icon={faSignOut} size="xs" />
                                                    </Tooltip>
                                                )
                                        }
                                    </IconButton>
                                    {
                                        circle.creatorId === userStore.user?.id && (
                                            <IconButton onClick={() => saveChanges(circle.id)}>
                                                <Tooltip title="Save Changes">
                                                    <FontAwesomeIcon icon={faCheck} size="xs" />
                                                </Tooltip>
                                            </IconButton>
                                        )
                                    }
                                </StyledTableCell>
                            </StyledTableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    )
}

export default observer(CirclesSettings);