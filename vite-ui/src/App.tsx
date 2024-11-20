import { observer } from 'mobx-react-lite';
import './App.css'
import NavBar from './components/common/NavBar';
import { useStore } from './stores/store';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Loader from './components/common/Loader';
import { Outlet } from 'react-router-dom';
import useMediaQuery from '@mui/material/useMediaQuery';


function App() {
  const { userStore } = useStore();
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');

  const theme = createTheme({
    palette: {
      mode: prefersDarkMode ? 'dark' : 'light',
    },
  });

  if (userStore.loading) {
    return <Loader text={userStore.loaderText} className='loader' />
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <NavBar />
      <div className='container'>
        <Outlet />
      </div>
    </ThemeProvider>

  )
}

export default observer(App);
