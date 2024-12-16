import { Box, Typography, Button } from '@mui/material'
import { Link } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { RootState } from '@/app/store'
import { selectIsAuthenticated } from '@/features/auth/authSlice'
import { Role } from '@/features/auth/configs'

const NotFoundPage = () => {
  const isAuthenticated = useSelector(selectIsAuthenticated)
  const user = useSelector((state: RootState) => state.auth.user)
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '100vh',
        textAlign: 'center',
        backgroundColor: '#f4f6f8',
        padding: 3,
      }}
    >
      <Typography
        variant="h1"
        component="div"
        sx={{ fontWeight: 'bold', color: 'primary' }}
      >
        404
      </Typography>
      <Typography variant="h5" sx={{ marginTop: 2, color: '#555' }}>
        Trang bạn đang tìm kiếm không tồn tại.
      </Typography>
      <Typography variant="body1" sx={{ marginTop: 1, color: '#777' }}>
        Trang có thể đã bị xóa, đổi tên hoặc không tồn tại.
      </Typography>
      <Box sx={{ marginTop: 4 }}>
        <Button
          variant="contained"
          color="primary"
          component={Link}
          to={
            isAuthenticated
              ? user?.role === Role.ADMIN || user?.role === Role.TEACHER
                ? '/admin'
                : '/exam'
              : '/users/login'
          }
          sx={{
            textTransform: 'none',
            fontWeight: 'bold',
            padding: '10px 20px',
            fontSize: '16px',
          }}
        >
          Trở về trang chủ
        </Button>
      </Box>
    </Box>
  )
}

export default NotFoundPage
