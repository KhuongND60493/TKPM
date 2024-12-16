import APIClient, { getUrlGet } from '@/app/api/axios/AxiosClient'
import { attemptConfigs } from './configs'
import {
  IAttemptExam,
  IAttemptStudentAnswer,
  IErrorAPI,
  IReviewResultData,
  IStartStudentAttempt,
  ISubmitStudentAttempt,
  ISumaryAttemptExam,
  ISummaryStudentAttempt,
} from '@/features/exam/type'
export default class AttemptClient extends APIClient {
  getExams(cancelToken?: any): Promise<ISumaryAttemptExam[] | undefined> {
    return super
      .get(attemptConfigs.getExams, undefined, cancelToken)
      .then((r) => {
        if (r?.success) {
          return r?.data as ISumaryAttemptExam[]
        }
        return undefined
      })
  }

  getAttemptResultById(
    attemptId: string,
    cancelToken?: any,
  ): Promise<IReviewResultData | undefined> {
    const subParams = getUrlGet(attemptConfigs.getAttemptResult, {
      id: attemptId,
    })
    return super.get(subParams, undefined, cancelToken).then((r) => {
      if (r?.success) {
        return r?.data as IReviewResultData
      }
      return undefined
    })
  }
  getSummaryAttemptsByExamId(
    examId: string,
    cancelToken?: any,
  ): Promise<ISummaryStudentAttempt | undefined> {
    const subParams = getUrlGet(attemptConfigs.getAttempts, { id: examId })
    return super.get(subParams, undefined, cancelToken).then((r) => {
      if (r?.success) {
        return r?.data as ISummaryStudentAttempt
      }
      return undefined
    })
  }
  getAttemptByExamId(
    examId: string,
    cancelToken?: any,
  ): Promise<IAttemptExam[] | undefined> {
    const subParams = getUrlGet(attemptConfigs.getAttemptByExamId, { examId })
    return super.get(subParams, undefined, cancelToken).then((r) => {
      if (r?.success) {
        return r?.data as IAttemptExam[]
      }
      return undefined
    })
  }
  startAttempt(
    examId: string,
    cancelToken?: any,
  ): Promise<IStartStudentAttempt | undefined> {
    return super
      .post(attemptConfigs.startAttempt, { examId }, cancelToken)
      .then((r) => {
        if (r?.success) {
          return {
            attempId: r?.data?.id,
            examId: r?.data?.examId,
            title: r?.data?.examTitle,
            description: r?.data?.examDescription,
            status: r?.data?.status,
            duration: r?.data?.duration,
            startTime: r?.data?.startTime,
          }
        }
        return undefined
      })
  }
  sendSubmitAttempt(
    partType: number,
    attemptId: number,
    data: IAttemptStudentAnswer,
    cancelToken?: any,
  ): Promise<ISubmitStudentAttempt | undefined> {
    const subParams = getUrlGet(attemptConfigs.sendSubmit, {
      id: attemptId,
    })
    return super
      .post(subParams, { ...data, type: partType }, cancelToken)
      .then((r) => {
        if (r?.success) {
          return r?.data
        }
        return undefined
      })
  }
  sendSpeakingSubmitAttempt(
    partType: number,
    attemptId: number,
    answer: IAttemptStudentAnswer,
    cancelToken?: any,
  ): Promise<ISubmitStudentAttempt | undefined> {
    const subParams = getUrlGet(attemptConfigs.sendSubmit, {
      id: attemptId,
    })

    const formData = new FormData()
    formData.append('sectionType', answer.sectionType.toString())
    formData.append('partId', answer.partId.toString())
    formData.append('type', partType.toString())
    answer.questions.forEach((question, index) => {
      formData.append(`answers[${index}].id`, question.id.toString())
      formData.append(
        `answers[${index}].audioFile`,
        new File(
          [question.answer as string],
          `audio_question_${question.id}.wav`,
          {
            type: 'audio/wav',
          },
        ),
      )
    })

    return super
      .postFormData(subParams, formData, cancelToken)
      .then((response) => {
        if (response?.success) {
          return response?.data
        }
        return undefined
      })
  }

  finishAttempt(
    attemptId: number,
    cancelToken?: any,
  ): Promise<IStartStudentAttempt | IErrorAPI | undefined> {
    const subParams = getUrlGet(attemptConfigs.finishAttempt, {
      id: attemptId,
    })
    return super.post(subParams, undefined, cancelToken).then((r) => {
      if (r?.success) {
        return r?.data
      }
      if (r?.message) {
        return {
          success: false,
          message: r?.message,
        } as IErrorAPI
      }
      return undefined
    })
  }
}
