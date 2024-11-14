import { IconButton, styled, TableBody, TableCell, tableCellClasses, TableContainer, TableHead, TableRow, Tooltip } from "@mui/material";
import { Paper, Table } from "@mui/material";
import { useStore } from "../../stores/store";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSignIn, faTimesCircle } from "@fortawesome/free-solid-svg-icons";
import { observer } from "mobx-react-lite";
import { ConfirmJoinDto } from "../../api/dtos/circle_dtos/ConfirmJoinDto";
import { useEffect, useState } from "react";
import { RequestDto } from "../../api/dtos/circle_dtos/RequestDto";

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

const JoinRequests = () => {
    const { userStore, circleStore } = useStore();
    const [requestsList, setRequestsList] = useState<RequestDto[]>(circleStore.requestsList);

    useEffect(() => {
        setRequestsList(circleStore.requestsList);
    }, [circleStore.requestsList]);

    return (
        <div id="join-request" className="profile-element-column">
            <h3>Join Circle Requests</h3>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <StyledTableCell>Circle Name</StyledTableCell>
                            <StyledTableCell>Inviter</StyledTableCell>
                            <StyledTableCell>Action</StyledTableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {requestsList && requestsList.map((jr) => (
                            <StyledTableRow key={jr.requestId}>
                                <StyledTableCell>{jr.circleName}</StyledTableCell>
                                <StyledTableCell>{jr.inviterName + ' ' + jr.inviterSurname + ' ( ' + jr.inviterUserName + ' ) '}</StyledTableCell>
                                <StyledTableCell>
                                    <IconButton
                                        onClick={() => {
                                            const dto: ConfirmJoinDto = {
                                                circleId: jr.circleId,
                                                userId: userStore.user!.id,
                                                isAccepted: true
                                            }
                                            circleStore.confirmJoin(dto);
                                        }}>
                                        <Tooltip title="Join Circle">
                                            <FontAwesomeIcon icon={faSignIn} size="xs" />
                                        </Tooltip>
                                    </IconButton>
                                    <IconButton
                                        onClick={() => {
                                            const dto: ConfirmJoinDto = {
                                                circleId: jr.circleId,
                                                userId: userStore.user!.id,
                                                isAccepted: false
                                            }
                                            circleStore.confirmJoin(dto);
                                        }}>
                                        <Tooltip title="Discard Request">
                                            <FontAwesomeIcon icon={faTimesCircle} size="xs" />
                                        </Tooltip>
                                    </IconButton>
                                </StyledTableCell>
                            </StyledTableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    );
};

export default observer(JoinRequests);