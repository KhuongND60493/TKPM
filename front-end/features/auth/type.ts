import { Role } from './configs.ts'

export interface IUser {
  id: number
  role: Role
  lastName: string
  firstName: string
  fullName?: string
}

export interface IResLogin {
  accessToken: string
  refreshToken: string
  user: IUser
}
