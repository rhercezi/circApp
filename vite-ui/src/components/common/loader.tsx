import { Box, CircularProgress } from "@mui/material";

interface Props {
    text: string;
}

export default function Loader({text}: Props) {
    return (
        <Box sx={{display: 'flex', 
                flexDirection: 'column',
                justifyContent: 'center'}}>
            <CircularProgress />
            <Box sx={{textAlign: 'center'}}>{text}</Box>
        </Box>
    )
}