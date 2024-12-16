import { Outlet } from 'react-router-dom'
import AdminLayout from './AdminLayout'
import StudentLayout from './StudentLayout'

export const MainLayout = () => {
  return <Outlet />
}

export { AdminLayout, StudentLayout }
