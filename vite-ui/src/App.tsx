import { observer } from 'mobx-react-lite';
import './App.css'
import NavBar from './components/common/navBar';
import Login from './components/users/login';
import { useStore } from './stores/store';
import UserStore from './stores/userStore';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Loader from './components/common/loader';
import { Box } from '@mui/material';


function App() {
  const {userStore} = useStore();
  const theme = createTheme({
    palette: {
      mode: 'dark'
    },
  })

  if (userStore.loading) {
    return <Loader text="Logging in..." />
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <NavBar />
      <Box sx={{
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh'
      }}>
        {
          userStore.user?.userName ? <div>Logged in</div> : <Login />
        }
      </Box>
    </ThemeProvider>
    
  )
}

export default observer(App);
