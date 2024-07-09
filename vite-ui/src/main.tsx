import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import { StoreContext, store } from './stores/store.ts'
import { RouterProvider } from 'react-router-dom'
import { router } from './router/Routes.tsx'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <StoreContext.Provider value={store}>
      <RouterProvider router={router} />
    </StoreContext.Provider>
  </React.StrictMode>
)
