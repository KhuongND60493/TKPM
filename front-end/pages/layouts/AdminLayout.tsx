import React, { useMemo, useState } from 'react'
import { Box, Container } from '@mui/material'
import Footer from '@/pages/layouts/Footer.tsx'
import { Outlet } from 'react-router-dom'
import AdminLeftMenu from '@/pages/layouts/AdminLeftMenu.tsx'
import AdminHeader from '@/pages/layouts/AdminHeader.tsx'

const drawerWidth = 300

export interface AdminLayoutProps {
  window?: () => Window
  isOpen: boolean
  onDrawerToggle: () => void
  width: number
}

interface Props {
  window?: () => Window
}

const AdminLayout: React.FC<Props> = (props) => {
  const { window } = props
  const [open, setOpen] = useState(true)

  const handleDrawerToggle = () => {
    setOpen(!open)
  }
  const propsMenu: AdminLayoutProps = useMemo(() => {
    return {
      width: drawerWidth,
      onDrawerToggle: handleDrawerToggle,
      isOpen: open,
      window: window,
    }
  }, [open, handleDrawerToggle, window, drawerWidth])
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AdminHeader {...propsMenu} />
      <AdminLeftMenu {...propsMenu} />
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          bgcolor: 'background.default',
          p: 3,
          width: { sm: `calc(100% - ${open ? drawerWidth : 0}px)` },
          marginLeft: { xs: 0, sm: `${open ? drawerWidth : 0}px` },
          transition: 'margin 0.3s',
          overflowY: 'hidden',
          mt: '64px',
        }}
      >
        <Container maxWidth="lg">
          <Outlet />
        </Container>
      </Box>
      <Footer />
    </Box>
  )
}

export default AdminLayout
