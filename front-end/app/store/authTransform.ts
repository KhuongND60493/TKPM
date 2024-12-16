import { createTransform } from 'redux-persist'
import { AuthState } from '@/features/auth/authSlice'

const authTransform = createTransform<AuthState, Partial<AuthState>>(
  (inboundState: AuthState) => ({
    accessToken: inboundState.accessToken,
    refreshToken: inboundState.refreshToken,
  }),
  (outboundState: Partial<AuthState>) => ({
    accessToken: outboundState.accessToken ?? null,
    refreshToken: outboundState.refreshToken ?? null,
    user: null,
  }),
  { whitelist: ['auth'] },
)

export default authTransform
