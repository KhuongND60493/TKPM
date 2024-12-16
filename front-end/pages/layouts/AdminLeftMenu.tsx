import CloseIcon from '@mui/icons-material/Close'
import { AdminLayoutProps } from './AdminLayout'
import AdminExamManagementMenu from './AdminExamManagementMenu'
import AdminDefaultMenu from './AdminDefaultMenu'
import { useNavigate } from 'react-router-dom'
import {
  IconButton,
  Drawer,
  Box,
  Divider,
  List,
  ListItemButton,
  ListItemText,
  ListItemIcon,
  Typography,
  Avatar,
} from '@mui/material'
import ExitToAppIcon from '@mui/icons-material/ExitToApp'
import logoUrl from '@/assets/logo.webp'
import { clearCredentials } from '@/features/auth/authSlice'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '@/app/store'
import { useLocation } from 'react-router-dom'
import { isMatchWithManageExam } from '@/features/exam/utils.ts'
import { useMemo } from 'react'

const AdminLeftMenu = (props: AdminLayoutProps) => {
  const location = useLocation()

  const navigate = useNavigate()
  const exam = useSelector((state: RootState) => state.examAdmin.exam)

  const dispatch = useDispatch()
  const { window, width, isOpen, onDrawerToggle } = props

  const isExamManagement = useMemo(() => {
    return isMatchWithManageExam(location.pathname)
  }, [location.pathname])

  const container =
    window !== undefined ? () => window().document.body : undefined
  const handleAccessToPage = (path: string) => {
    navigate(path)
  }
  const handleLogout = () => {
    dispatch(clearCredentials())
    handleAccessToPage('/users/login')
  }
  const drawer = (
    <Box>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          padding: '12px',
        }}
      >
        <Avatar
          src={logoUrl}
          alt="Logo"
          sx={{ width: 40, height: 40, mr: 2 }}
        />
        <Box flex={1}>
          <Typography
            color="textPrimary"
            variant={'h6'}
            sx={{ fontWeight: 'bold' }}
          >
            TKPM I/2024
          </Typography>
        </Box>

        <IconButton onClick={onDrawerToggle}>
          <CloseIcon />
        </IconButton>
      </Box>
      <Divider />
      {exam && isExamManagement ? (
        <AdminExamManagementMenu exam={exam} />
      ) : (
        <AdminDefaultMenu />
      )}
    </Box>
  )

  return (
    <Drawer
      container={container}
      variant={isOpen ? 'persistent' : 'temporary'}
      open={isOpen}
      onClose={onDrawerToggle}
      ModalProps={{
        keepMounted: true,
      }}
      sx={{
        width,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width, boxSizing: 'border-box' },
      }}
    >
      {drawer}
      <Divider />
      <List>
        <ListItemButton onClick={handleLogout}>
          <ListItemIcon>
            <ExitToAppIcon />
          </ListItemIcon>
          <ListItemText primary="Đăng xuất" />
        </ListItemButton>
      </List>
    </Drawer>
  )
}
export default AdminLeftMenu
