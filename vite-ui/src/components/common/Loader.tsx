import { Box, CircularProgress } from "@mui/material";

interface Props {
    text: string;
    className?: string;
}

export default function Loader({ text, className='' }: Props) {
    return (
        <div className={className}>
            <CircularProgress />
            <Box sx={{ textAlign: 'center' }}>{text}</Box>
        </div>
    )
}