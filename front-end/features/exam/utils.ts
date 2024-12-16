import { IErrorAPI, IOption } from '@/features/exam/type.ts'
import { KEY_SUBMIT_RESPONSE } from '@/features/exam/configs.ts'

export const isMatchWithManageExam = (pathname: string) => {
  return /^\/admin\/questions\/\d+$/.test(pathname)
}

export const optionsArrayToObject = (
  options: IOption[],
): { [key: number]: IOption } => {
  return options.reduce(
    (acc, option) => {
      acc[option.id] = option
      return acc
    },
    {} as { [key: number]: IOption },
  )
}
export const areOptionsDifferent = (
  optionsArray: IOption[],
  optionsObject: { [key: number]: IOption },
): boolean => {
  if (optionsArray.length !== Object.keys(optionsObject).length) {
    return true
  }

  for (const option of optionsArray) {
    const correspondingOption = optionsObject[option.id]
    if (
      !correspondingOption ||
      correspondingOption.content !== option.content ||
      correspondingOption.isCorrect !== option.isCorrect
    ) {
      return true
    }
  }
  return false
}
export function formatDateTime(dateTime: string | null | undefined): string {
  if (!dateTime) {
    return ''
  }
  try {
    const date = new Date(dateTime)
    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    }).format(date)
  } catch (error) {
    console.error('Invalid date format:', error)
    return ''
  }
}

export const checkAndFinishAttempt = (startTime: string, duration: number) => {
  const startTimeUTC = new Date(startTime)
  const endTimeUTC = new Date(startTimeUTC.getTime() + duration * 60 * 1000)
  const currentTimeUTC = new Date(new Date().toISOString())
  return endTimeUTC <= currentTimeUTC
}

const fnErrorToast: { [key: string]: string } = {
  [`${KEY_SUBMIT_RESPONSE.ANSWER_ATTEMPT_EMPTY}`]:
    'Không init được.Hãy thử lại!!!',
  [`${KEY_SUBMIT_RESPONSE.QUESTION_EMPTY}`]: 'Bạn chưa trả lời câu hỏi nào cả',
  [`${KEY_SUBMIT_RESPONSE.API_BACK_ERROR}`]:
    'Không nhận được response từ server',
  [`${KEY_SUBMIT_RESPONSE.ATTEMPT_NOT_IN_PROGRESS}`]:
    'Bài thi có thể đã kết thúc',
}
export const handleAttemptError = (key?: string) => {
  if (key) {
    let msg = 'Đã xảy ra lỗi không xác định'
    if (key in fnErrorToast) {
      msg = fnErrorToast[key]
    } else {
      return key
    }
    return msg
  }
}

export function isErrorAPI(obj: any): obj is IErrorAPI {
  return (
    obj &&
    typeof obj === 'object' &&
    obj.success === false &&
    typeof obj.message === 'string'
  )
}
