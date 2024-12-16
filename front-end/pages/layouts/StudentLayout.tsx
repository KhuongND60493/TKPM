import StudentHeader from '@/pages/layouts/StudentHeader'
import { Outlet } from 'react-router-dom'
const StudentLayout = () => {
  return (
    <>
      <StudentHeader />
      <Outlet />
    </>
  )
}
export default StudentLayout
