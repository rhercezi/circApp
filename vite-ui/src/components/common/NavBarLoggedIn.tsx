import React, { useEffect, useState } from "react";
import { Avatar, Box, ClickAwayListener, Divider, Drawer, IconButton, ListItemIcon, Menu, MenuItem, Popper, Tooltip } from "@mui/material";
import { Logout, Settings } from "@mui/icons-material";
import { styled, css } from '@mui/system';
import { useNavigate } from "react-router-dom";
import { useStore } from "../../stores/store";
import CircleDrawer from "../circles/CircleDrawer";
import MyButton from "./buttons/MyButton";
import DialogHandler from "./DialogHandler";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBell, faClipboardList, faHome } from "@fortawesome/free-solid-svg-icons";
import { observer } from "mobx-react-lite";
import { reaction } from "mobx";
import Notification from "../notifications/Notification";

const NavBarLoggedIn = () => {
    const { userStore, eventStore } = useStore();
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [anchorElNotify, setAnchorElNotify] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const [openCirclesDrawer, setOpenCirclesDrawer] = useState(false);
    const [elementChange, setElementChange] = useState<[string, boolean]>(['', false]);
    const [itemsCount, setItemsCount] = useState(eventStore.events.length);
    const sendMessage = eventStore.sendMessage;
    const removeNotification = eventStore.removeNotification;

    useEffect(() => {
        const dispose = reaction(
            () => eventStore.events.length,
            () => {
                setItemsCount(eventStore.events.length);
                const latestEvent = eventStore.events[eventStore.events.length - 1];
                console.log("Latest event:", latestEvent);
            }
        );

        return () => dispose();
    }, [eventStore]);

    const handleCirclesDrawer = (openDrawer: boolean) => () => {
        setOpenCirclesDrawer(openDrawer);
    };

    const handleClickAccount = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };
    const handleCloseAccount = () => {
        setAnchorEl(null);
    };

    const handleClickAway = () => {
        setAnchorElNotify(null);
    }
    const handleClickNotify = (event: React.MouseEvent<HTMLElement>) => {
        event.stopPropagation();
        setAnchorElNotify(anchorElNotify ? null : event.currentTarget);
    };
    const openNotify = Boolean(anchorElNotify);
    const id = openNotify ? 'simple-popper' : undefined

    const handleLogout = () => {
        userStore.logout();
        eventStore.disconnect();
        navigate('/login');
    }

    const openProfile = () => {
        navigate('/profile');
    }

    const openSettings = () => {
        navigate('/settings');
    }

    const user = userStore.user;


    return (
        <>
            <Box className='nav-bar'>
                <div className="nav-bar-left">
                    <MyButton onClick={handleCirclesDrawer(true)}>Circles</MyButton>
                    <Drawer sx={
                        {
                            backgroundImage: "none",
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "space-between"
                        }
                    } open={openCirclesDrawer} onClick={handleCirclesDrawer(false)}>
                        <CircleDrawer setOpenCirclesDrawer={setOpenCirclesDrawer} setElementChange={setElementChange} />
                    </Drawer>
                    <DialogHandler elementChange={elementChange} />
                </div>
                <div className="nav-bar-avatar">
                    <Tooltip title={itemsCount > 0 ? `${itemsCount} new event pending` : 'Nothing new at the moment'}>
                        <IconButton onClick={handleClickNotify}>
                            <Box
                                component="span"
                                sx={{
                                    position: 'absolute',
                                    top: 0,
                                    right: 0,
                                    backgroundColor: itemsCount > 0 ? 'red' : 'green',
                                    color: 'white',
                                    borderRadius: '50%',
                                    width: 20,
                                    height: 20,
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    fontSize: '0.75rem',
                                }}
                            >
                                {itemsCount}
                            </Box>
                            <FontAwesomeIcon icon={faBell} />
                        </IconButton>
                    </Tooltip>
                    {
                        itemsCount > 0 && (
                            <ClickAwayListener onClickAway={handleClickAway}>
                                <Popper id={id} open={openNotify} anchorEl={anchorElNotify}>
                                    <StyledPopperDiv>
                                        {eventStore.events.map((event, index) => (
                                            <div key={event.Id}>
                                                {index > 0 && <Divider />}
                                                <Notification key={event.Id} notification={event} sendMessage={sendMessage} removeNotification={removeNotification} />
                                            </div>
                                        ))}
                                    </StyledPopperDiv>
                                </Popper>
                            </ClickAwayListener>
                        )
                    }
                    <Tooltip title="Tasks">
                        <IconButton onClick={() => navigate('/tasks')}>
                            <FontAwesomeIcon icon={faClipboardList} />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Home">
                        <IconButton onClick={() => navigate('/dashboard')}>
                            <FontAwesomeIcon icon={faHome} />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Account settings">
                        <IconButton
                            onClick={handleClickAccount}
                            size="small"
                            sx={{ ml: 2 }}
                            aria-controls={open ? 'account-menu' : undefined}
                            aria-haspopup="true"
                            aria-expanded={open ? 'true' : undefined}>
                            <Avatar sx={{ width: 32, height: 32 }}>{user && user.firstName && user.familyName
                                ? user?.firstName?.charAt(0) + user?.familyName?.charAt(0)
                                : ''}
                            </Avatar>
                        </IconButton>
                    </Tooltip>
                    <Menu
                        anchorEl={anchorEl}
                        id="account-menu"
                        open={open}
                        onClose={handleCloseAccount}
                        onClick={handleCloseAccount}
                        PaperProps={{
                            elevation: 0,
                            sx: {
                                overflow: 'visible',
                                filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.32))',
                                mt: 1.5,
                                '& .MuiAvatar-root': {
                                    width: 32,
                                    height: 32,
                                    ml: -0.5,
                                    mr: 1,
                                },
                                '&::before': {
                                    content: '""',
                                    display: 'block',
                                    position: 'absolute',
                                    top: 0,
                                    right: 14,
                                    width: 10,
                                    height: 10,
                                    bgcolor: 'background.paper',
                                    transform: 'translateY(-50%) rotate(45deg)',
                                    zIndex: 0,
                                },
                            },
                        }}
                        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
                        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
                    >
                        <MenuItem onClick={openProfile}>
                            <Avatar /> Profile
                        </MenuItem>
                        <Divider />
                        <MenuItem onClick={openSettings}>
                            <ListItemIcon>
                                <Settings fontSize="small" />
                            </ListItemIcon>
                            Settings
                        </MenuItem>
                        <MenuItem onClick={handleLogout}>
                            <ListItemIcon>
                                <Logout fontSize="small" />
                            </ListItemIcon>
                            Logout
                        </MenuItem>
                    </Menu>
                </div>
            </Box>
        </>
    )
}

export default observer(NavBarLoggedIn);

const grey = {
    50: '#F3F6F9',
    100: '#E5EAF2',
    200: '#DAE2ED',
    300: '#C7D0DD',
    400: '#B0B8C4',
    500: '#9DA8B7',
    600: '#6B7A90',
    700: '#434D5B',
    800: '#303740',
    900: '#1C2025',
};


const StyledPopperDiv = styled('div')(
    ({ theme }) => css`
      border-radius: 8px;
      background-color: ${theme.palette.mode === 'dark' ? '#121212' : grey[50]};
      display: block;
      position: absolute
      right: 1rem;
      border: 1px solid ${theme.palette.mode === 'dark' ? grey[700] : grey[200]};
      box-shadow: ${theme.palette.mode === 'dark'
            ? `0px 4px 8px rgb(0 0 0 / 0.7)`
            : `0px 4px 8px rgb(0 0 0 / 0.1)`};
      padding: 0.75rem;
      color: ${theme.palette.mode === 'dark' ? grey[100] : grey[700]};
      font-size: 0.875rem;
      font-weight: 500;
      opacity: 1;
      margin: 0.75rem 0;
      min-width: 200px;
      max-width: 300px;
      max-height: 60vh;
      overflow-y: scroll;
    `,
);