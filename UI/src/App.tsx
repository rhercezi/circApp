import { useEffect, useState } from 'react'
import './App.css'
import axios from 'axios'

function App() {
  const [user, setUser] = useState({ firstName: '', familyName: '' })

  useEffect(() => {
    axios.get('http://localhost:5011/api/User/ById/93CCCE56-45A6-4694-BE92-187DF5DFE0A8')
    .then(response => {
      setUser(response.data)
    })
  }, [])

  return (
    <>
      <div>
        <h1>Wellcome {user.firstName} {user.familyName}</h1>
      </div>
    </>
  )
}

export default App
