import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from '@/app/store'
import { IUser } from './type.ts'
export interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  user: IUser | null
}

const initialState: AuthState = {
  accessToken: null,
  refreshToken: null,
  user: null,
}
export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{
        accessToken: string
        refreshToken: string
        user: IUser
      }>,
    ) => {
      state.accessToken = action.payload.accessToken
      state.refreshToken = action.payload.refreshToken
      state.user = {
        ...action.payload.user,
        fullName: `${action.payload.user.firstName} ${action.payload.user.lastName}`,
      }
    },
    clearCredentials: (state) => {
      state.accessToken = null
      state.refreshToken = null
      state.user = null
    },
  },
})
export const { setCredentials, clearCredentials } = authSlice.actions

export const selectAuth = (state: RootState) => state.auth || initialState
export const selectIsAuthenticated = (state: RootState) =>
  state?.auth?.accessToken && state?.auth?.accessToken !== ''

export const authReducer = authSlice.reducer
