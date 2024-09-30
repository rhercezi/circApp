import React, { useState } from "react";
import { Avatar, Box, Divider, Drawer, IconButton, ListItemIcon, Menu, MenuItem, Tooltip } from "@mui/material";
import { Logout, Settings } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../stores/store";
import CircleDrawer from "../circles/CircleDrawer";
import MyButton from "./buttons/MyButton";
import DialogHandler from "./DialogHandler";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHome } from "@fortawesome/free-solid-svg-icons";

export default function NavBarLoggedIn() {
    const { userStore } = useStore();
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const [openCirclesDrower, setOpenCirclesDrower] = useState(false);
    const [elementChange, setElementChange] = useState<[string, boolean]>(['', false]);

    const handleCirclesDrower = (openDrower: boolean) => () => {
        setOpenCirclesDrower(openDrower);
    };
    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };
    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        userStore.logout();
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
                    <MyButton onClick={handleCirclesDrower(true)}>Circles</MyButton>
                    <Drawer sx={
                        {
                            backgroundImage: "none",
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "space-between"
                        }
                    } open={openCirclesDrower} onClick={handleCirclesDrower(false)}>
                        <CircleDrawer setOpenCirclesDrower={setOpenCirclesDrower} setElementChange={setElementChange} />
                    </Drawer>
                    <DialogHandler elementChange={elementChange} />
                </div>
                <div className="nav-bar-avatar">
                    <IconButton onClick={() => navigate('/dashboard')}>
                        <FontAwesomeIcon icon={faHome} />
                    </IconButton>
                    <Tooltip title="Account settings">
                        <IconButton
                            onClick={handleClick}
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
                        onClose={handleClose}
                        onClick={handleClose}
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