import { observer } from 'mobx-react-lite';
import './App.css'
import NavBar from './components/common/NavBar';
import { useStore } from './stores/store';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Loader from './components/common/Loader';
import { Outlet } from 'react-router-dom';


function App() {
  const {userStore} = useStore();
  const theme = createTheme({
    palette: {
      mode: 'dark'
    },
  })

  if (userStore.loading) {
    return <Loader text={userStore.loaderText} className='loader'/>
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <NavBar />
        <Outlet />
    </ThemeProvider>
    
  )
}

export default observer(App);
