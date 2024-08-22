import { Avatar, Box, IconButton, Tooltip, Typography } from "@mui/material";
import { NavLink } from "react-router-dom";

export default function NavBarNLI() {    
    return (
        <>
            <Box className='nav-bar'>
                <div className="nav-bar-left">
                    <NavLink to='/signup'>
                        <Typography sx={{ minWidth: 100 }}>Sign Up</Typography>
                    </NavLink>
                    <NavLink to='/login'>
                        <Typography sx={{ minWidth: 100 }}>Log In</Typography>
                    </NavLink>
                </div>
                <div className="nav-bar-avatar">
                    <Tooltip title="Log in or sign up">
                        <IconButton
                            size="small"
                            sx={{ ml: 2 }}>
                            <Avatar sx={{ width: 32, height: 32 }}>?</Avatar>
                        </IconButton>
                    </Tooltip>
                </div>
            </Box>
        </>
    )
}