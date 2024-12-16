import APIClient, { getUrlGet } from '@/app/api/axios/AxiosClient'
import {
  sectionPartsPathConfigs,
  questionPathConfigs,
} from '@/features/exam/configs'
import { IOption, IQuestion } from '@/features/exam/type'
export default class QuestionClient extends APIClient {
  questionsByPartId(partId: number, cancelToken?: any) {
    const subParams = getUrlGet(sectionPartsPathConfigs.questions, { partId })
    return super.get(subParams, undefined, cancelToken).then((r) => {
      if (r?.success) {
        return r?.data as IQuestion[]
      }
      return undefined
    })
  }

  createEmptyOption(questionId: number, cancelToken?: any) {
    return super
      .post(questionPathConfigs.createEmptyOption, { questionId }, cancelToken)
      .then((r) => {
        if (r?.success) {
          return r?.data as IOption
        }
        return undefined
      })
  }

  createEmptyQuestion(partId: number, cancelToken?: any) {
    return super
      .post(questionPathConfigs.createEmptyQuestion, { partId }, cancelToken)
      .then((r) => {
        if (r?.success && r?.data?.question) {
          return r?.data?.question as IQuestion
        }
        return undefined
      })
  }

  deleteOption(optionId: number, cancelToken?: any) {
    const subParams = getUrlGet(questionPathConfigs.deleteOption, {
      optionId,
    })
    return super.delete(subParams, undefined, cancelToken).then((r) => {
      return r?.success || false
    })
  }
  deleteQuestion(questionId: number, cancelToken?: any) {
    const subParams = getUrlGet(questionPathConfigs.deleteQuestion, {
      id: questionId,
    })
    return super.delete(subParams, undefined, cancelToken).then((r) => {
      return r?.success || false
    })
  }

  updateQuestion(question: IQuestion, cancelToken?: any) {
    const subParams = getUrlGet(questionPathConfigs.updateQuestion, {
      questionId: question.id,
    })
    return super
      .put(
        subParams,
        { point: question.point, questionText: question.questionText },
        cancelToken,
      )
      .then((r) => {
        return r?.success || false
      })
  }
  updateOptions(questionId: number, options: IOption[], cancelToken?: any) {
    const subParams = getUrlGet(questionPathConfigs.updateOptions, {
      questionId: questionId,
    })
    return super.put(subParams, { options }, cancelToken).then((r) => {
      return r?.success || false
    })
  }
}
