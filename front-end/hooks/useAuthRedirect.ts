import { useEffect } from 'react'
import { useSelector } from 'react-redux'
import { useNavigate, useLocation } from 'react-router-dom'
import { RootState } from '@/app/store'
import { allowedRoutes } from '@/app/routes'

const useAuthRedirect = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { accessToken, user } = useSelector((state: RootState) => state.auth)

  useEffect(() => {
    if (!accessToken) {
      if (location.pathname !== '/users/login') {
        navigate('/users/login')
      }
    } else if (user) {
      const userAllowedRoutes = allowedRoutes[user.role]
      if (userAllowedRoutes) {
        const isAllowed = userAllowedRoutes.some((route) =>
          location.pathname.match(
            new RegExp(`^${route.replace(/:\w+/g, '\\w+')}$`),
          ),
        )

        if (!isAllowed) {
          navigate('/exam')
        }
      }
    }
  }, [accessToken, user, navigate, location.pathname])
}

export default useAuthRedirect
