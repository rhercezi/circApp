import UpdateUser from "./UpdateUser";
import UpdatePassword from "./UpdatePassword";
import Divider from '@mui/material/Divider';
import { useStore } from "../../stores/store";
import Loader from "../common/Loader";
import { Alert } from "@mui/material";

export default function Profile() {
    const { userStore } = useStore();
    if (userStore.loading) {
        return <Loader text={userStore.loaderText} className='loader' />
    }

    let errorElement = undefined;

    if (!userStore.loading && !userStore.isSuccess) {
        if (userStore.errorMap.has('updateUser')) {
            errorElement = (
                <>
                    <Alert severity="error">{userStore.errorMap.get('updateUser')}</Alert>
                    <Divider />
                </>
            )
        }
        if (userStore.errorMap.has('updatePwd')) {
            errorElement = (
                <>
                    <Alert severity="error">{userStore.errorMap.get('updatePwd')}</Alert>
                    <Divider />
                </>
            )
        }
    }

    if (userStore.isSuccess) {
        errorElement = (
            <>
                <Alert severity="success">Updated successfully</Alert>
                <Divider />
            </>
        )
    }

    return (
        <div className="profile-container">
            {errorElement}
            <UpdateUser />
            <Divider />
            <UpdatePassword />
        </div>
    )
}