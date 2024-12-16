import APIClient from '@/app/api/axios/AxiosClient'
import { storePathConfigs } from './configs'
export default class AuthClient extends APIClient {
  login(input: { email: string; password: string }, cancelToken?: any) {
    return super.post(storePathConfigs.login, input, cancelToken)
  }
}
