import React, { useState } from 'react'
import { useForm } from 'react-hook-form'
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Grid,
  Paper,
  TextField,
  Typography,
} from '@mui/material'
import backgroundImage from '@/assets/background_1.webp'
import { yupResolver } from '@hookform/resolvers/yup'
import * as Yup from 'yup'
import { useNavigate } from 'react-router-dom'
import { useDispatch } from 'react-redux'
import { setCredentials } from '@/features/auth/authSlice.ts'
import { authRequest } from '@/app/api'
import { IResLogin } from '@/features/auth/type.ts'
import { Role } from '@/features/auth/configs.ts'
import useAuthRedirect from '@/hooks/useAuthRedirect.ts'

const validationSchema = Yup.object().shape({
  email: Yup.string()
    .email('Email không hợp lệ')
    .required('Vui lòng nhập email'),
  password: Yup.string()
    .min(6, 'Mật khẩu phải có ít nhất 6 ký tự')
    .required('Vui lòng nhập mật khẩu'),
})

const Login: React.FC = () => {
  useAuthRedirect()
  const [loginError, setLoginError] = useState<string | null>(null)
  const dispatch = useDispatch()
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: yupResolver(validationSchema),
  })

  const onSubmit = async (data: { email: string; password: string }) => {
    try {
      setLoading(true)
      setLoginError(null)
      const rs = await authRequest.login(data)
      if (rs && rs.success) {
        const response = rs.data as IResLogin
        dispatch(
          setCredentials({
            accessToken: response.accessToken,
            refreshToken: response.refreshToken,
            user: response.user,
          }),
        )
        if (response.user.role === Role.ADMIN) {
          navigate('/admin')
        } else {
          navigate('/exam')
        }
      } else {
        setLoginError(`${rs?.message || 'Server error'}`)
      }
    } catch (e: any) {
      setLoginError(`${e.toString()}`)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Grid container component="main" sx={{ height: '100vh' }}>
      <Grid
        item
        xs={false}
        sm={4}
        md={7}
        sx={{
          backgroundImage: `url(${backgroundImage})`,
          backgroundRepeat: 'no-repeat',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      />
      <Grid
        item
        xs={12}
        sm={8}
        md={5}
        component={Paper}
        elevation={6}
        display={'flex'}
        square
        alignItems={'center'}
        justifyContent={'center'}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            width: '100%',
            maxWidth: '400px',
            borderRadius: 2,
            p: 4,
          }}
        >
          <Typography
            component="h1"
            variant="h4"
            color="primary"
            sx={{ mb: 2 }}
          >
            VSTEP B2 Test
          </Typography>
          <Typography
            variant="body1"
            color="textSecondary"
            align="center"
            sx={{ mb: 3 }}
          >
            Hãy đăng nhập để tiếp tục bài thi VSTEP B2. Đây là bước đầu tiên
            trong hành trình đánh giá khả năng tiếng Anh của bạn.
          </Typography>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ mt: 1 }}
          >
            {loginError && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {loginError}
              </Alert>
            )}
            <TextField
              label="Email"
              variant="outlined"
              fullWidth
              margin="normal"
              {...register('email')}
              error={!!errors.email}
              helperText={errors.email?.message}
            />
            <TextField
              label="Mật khẩu"
              variant="outlined"
              fullWidth
              margin="normal"
              type="password"
              {...register('password')}
              error={!!errors.password}
              helperText={errors.password?.message}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              sx={{ mt: 3, mb: 2, gap: 2 }}
              disabled={loading}
            >
              {loading && <CircularProgress size="20px" />}
              Sign In
            </Button>
          </Box>
        </Box>
      </Grid>
    </Grid>
  )
}

export default Login
