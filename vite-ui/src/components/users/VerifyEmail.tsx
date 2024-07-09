import { NavLink, useParams } from "react-router-dom";
import { useStore } from "../../stores/store";
import { useEffect, useState } from "react";
import Loader from "../common/Loader";

export default function VerifyEmail() {
    const { id } = useParams<{ id: string }>();
    const { userStore } = useStore();
    const [verificationStatus, setVerificationStatus] = useState<'loading' | 'verified' | 'error'>('loading');

    useEffect(() => {
        if (id !== undefined) {
            userStore.verifyEmail(id)
                .then(() => setVerificationStatus('verified'))
                .catch(() => setVerificationStatus('error'))
        }
    }, [id, userStore])

    if (verificationStatus === 'loading') {
        <Loader text='Verifying email...' className='loader' />
    }

    if (verificationStatus === 'verified') {
        return (
            <div>
                <h2>Your email has been verified, to log in please go to the <NavLink to='/login'>login page</NavLink>.</h2>
            </div>
        )
    }

    return (
        <div>
            <h2>Something went wrong, please try again later.</h2>
        </div>
    )
}