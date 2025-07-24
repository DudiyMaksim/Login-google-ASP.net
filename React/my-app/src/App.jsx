import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';
import axios from 'axios';

function App() {
    const handleSuccess = async (credentialResponse) => {
        try {
            const res = await axios.post(
                'http://localhost:5264/api/Account/google-login',
                { token: credentialResponse.credential },
                {
                    headers: { 'Content-Type': 'application/json' },
                    withCredentials: true
                }
            );
            console.log('Success:', res.data);
        } catch (error) {
            console.error('Error:', error.response?.data || error.message);
        }
    };

    return (
        <GoogleOAuthProvider clientId="455497108519-7sta00sn03o61l21fedqq62veujt58i0.apps.googleusercontent.com">
            <GoogleLogin
                onSuccess={handleSuccess}
                onError={() => console.log('Login Failed')}
                useOneTap
                auto_select
            />
        </GoogleOAuthProvider>
    );
}

export default App;  // Це обов'язково