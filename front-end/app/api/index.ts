import instance, { cancelToken } from './axios/AxiosInstance'
import {
  AuthRequest,
  ExamRequest,
  SectionPartRequest,
  QuestionRequest,
  AttemptRequest,
} from './requests'

const baseApiUrl = `${import.meta.env.VITE_BASE_URL || ''}/api`

export interface IResp<T> {
  data?: T
  status: boolean
  message?: string
}

const authRequest = new AuthRequest(baseApiUrl, instance)
const examRequest = new ExamRequest(baseApiUrl, instance)
const attemptRequest = new AttemptRequest(baseApiUrl, instance)
const questionRequest = new QuestionRequest(baseApiUrl, instance)
const sectionPartRequest = new SectionPartRequest(baseApiUrl, instance)
export {
  authRequest,
  examRequest,
  questionRequest,
  sectionPartRequest,
  attemptRequest,
  cancelToken,
}
