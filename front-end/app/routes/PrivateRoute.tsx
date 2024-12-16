import { PropsWithChildren } from 'react'
import useAuthRedirect from '@/hooks/useAuthRedirect.ts'

export const PrivateRoute = ({ children }: PropsWithChildren) => {
  useAuthRedirect()
  return <>{children}</>
}
