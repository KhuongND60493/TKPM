import {
  AppBar,
  Avatar,
  Box,
  Divider,
  IconButton,
  ListItemIcon,
  Menu,
  MenuItem,
  Toolbar,
  Typography,
} from '@mui/material'
import MenuIcon from '@mui/icons-material/Menu'
import LogoutIcon from '@mui/icons-material/Logout'
import { AdminLayoutProps } from './AdminLayout'
import { useLocation, useNavigate } from 'react-router-dom'
import { useMemo, useState } from 'react'
import { routeTitle } from '@/app/routes/configs'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from '@/app/store'
import { clearCredentials } from '@/features/auth/authSlice'

const AdminHeader = (props: AdminLayoutProps) => {
  const navigate = useNavigate()
  const dispatch = useDispatch()
  const user = useSelector((state: RootState) => state.auth.user)
  const { isOpen, width, onDrawerToggle } = props
  const { pathname } = useLocation()
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const open = Boolean(anchorEl)

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleMenuClose = () => {
    setAnchorEl(null)
  }

  const handleLogout = () => {
    dispatch(clearCredentials())
    navigate('/users/login')
  }

  const title = useMemo(() => {
    if (Object.prototype.hasOwnProperty.call(routeTitle, pathname)) {
      return routeTitle[pathname]
    }
    return 'TKPM-I/2024 Management'
  }, [pathname])
  return (
    <AppBar
      position="fixed"
      sx={{
        width: { sm: `calc(100% - ${isOpen ? width : 0}px)` },
        ml: { sm: `${isOpen ? width : 0}px` },
      }}
    >
      <Toolbar sx={{ justifyContent: 'space-between' }}>
        <Box
          flex={1}
          flexDirection={'row'}
          display={'flex'}
          alignItems={'center'}
        >
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={onDrawerToggle}
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" color="textPrimary">
            {title}
          </Typography>
        </Box>
        {user && (
          <Box display="flex" alignItems="center">
            <Typography variant="body1">
              Hi, {user.fullName || 'Admin'}
            </Typography>
            <IconButton onClick={handleMenuOpen} size="small" sx={{ ml: 2 }}>
              <Avatar sx={{ bgcolor: 'primary.main' }}>
                {user.lastName?.charAt(0).toUpperCase()}
              </Avatar>
            </IconButton>

            <Menu
              anchorEl={anchorEl}
              open={open}
              onClose={handleMenuClose}
              PaperProps={{
                elevation: 4,
                sx: {
                  mt: 1.5,
                  minWidth: 200,
                },
              }}
            >
              <Divider />
              <MenuItem onClick={handleLogout}>
                <ListItemIcon>
                  <LogoutIcon fontSize="small" />
                </ListItemIcon>
                Logout
              </MenuItem>
            </Menu>
          </Box>
        )}
      </Toolbar>
    </AppBar>
  )
}
export default AdminHeader
