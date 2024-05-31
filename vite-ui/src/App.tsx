import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import axios from 'axios';

function App() {
  const [user, setCircles] = useState<UserDto>();
  const token = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzZDcxNjRlNy00ZGJiLTRiYjItYmVmMi0wNGIyOGI3ZTYwMzYiLCJnaXZlbl9uYW1lIjoiUGVybyIsInVuaXF1ZV9uYW1lIjoiUGVyaWMiLCJqdGkiOiI0YTgzZWQwOS1mNzI1LTRhOGEtODI1MC1kNzI1Nzk3NzBjYWIiLCJpYXQiOjE3MTYwNDA3MDQsIm5iZiI6MTcxNjA0MDcwNCwiZXhwIjoxNzE2MjU2NzA0LCJpc3MiOiJodHRwczovL2NpcmMuYXBwLmNvbSJ9.4Zi_4CO8tj2riKsQLHwKT6tZRI-uPY7t8rHSSRYOORC5R62slL7kUG_j_XIhm4NqxeX1GbzB4R8fOjvryBn_UA';
  interface CircleDto {
    id: string;
    name: string;
    color: string;
    users: UserDto[];
  }
  interface UserDto {
    id: string;
    username: string;
    email: string;
    firstname: string;
    familyname: string;
    circles: CircleDto[];
  }


  useEffect(() => {
    axios.get<UserDto>('http://localhost:5028/v1/circles/3d7164e7-4dbb-4bb2-bef2-04b28b7e6036', { headers: { Authorization: `Bearer ${token}` } }).then((res) => {
      setCircles(res.data)
    })
   }, []);

  return (
    <>
      <div>
        <a href="https://vitejs.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <ul>
      {user && user.circles && user.circles.map(circle => (
    <li key={circle.id}>{circle.name}</li>
  ))}
      </ul>
    </>
  )
}

export default App
