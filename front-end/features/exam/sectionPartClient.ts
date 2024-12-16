import APIClient from '@/app/api/axios/AxiosClient'
import { sectionPartsPathConfigs } from './configs'
import { IExam, IReqPostSessionPart, ISessionPart } from '@/features/exam/type'

export default class SectionPartClient extends APIClient {
  sectionPartsById(id: number, cancelToken?: any) {
    return super
      .get(
        `${sectionPartsPathConfigs.listByType}/${id}`,
        undefined,
        cancelToken,
      )
      .then((r) => {
        if (r?.success && r?.data) {
          const { exam, sessions } = r.data as {
            exam: IExam
            sessions: ISessionPart[]
          }
          return { exam, sessions }
        }
        return undefined
      })
  }
  partsBySectionId(id: number, cancelToken?: any) {
    return super
      .get(
        `${sectionPartsPathConfigs.parts}/${id}/parts`,
        undefined,
        cancelToken,
      )
      .then((r) => {
        if (r?.success && r?.data) {
          const { parts } = r.data as {
            parts: ISessionPart[]
          }
          return parts
        }
        return undefined
      })
  }
  getPartById(id: number, cancelToken?: any) {
    return super
      .get(`${sectionPartsPathConfigs.partById}/${id}`, undefined, cancelToken)
      .then((r) => {
        if (r?.success && r?.data) {
          const part = r.data as ISessionPart
          return part
        }
        return undefined
      })
  }

  createSessionPart(data: IReqPostSessionPart, cancelToken?: any) {
    return super
      .post(`${sectionPartsPathConfigs.listByType}`, data, cancelToken)
      .then((r) => {
        if (r?.success) {
          return `${r.data}`
        } else {
          return undefined
        }
      })
  }

  updateSessionPart(id: number, data: IReqPostSessionPart, cancelToken?: any) {
    return super
      .put(`${sectionPartsPathConfigs.listByType}/${id}`, data, cancelToken)
      .then((r) => {
        if (r?.success) {
          return `${r.data}`
        } else {
          return undefined
        }
      })
  }
  uploadAudio(id: string, data: any, cancelToken?: any) {
    return super
      .putFormData(
        `${sectionPartsPathConfigs.partById}/${id}/audio`,
        data,
        cancelToken,
      )
      .then((r) => {
        if (r?.success) {
          return `${r.data}`
        } else {
          return undefined
        }
      })
  }
  updatePartContent(id: string, data: any, cancelToken?: any) {
    return super
      .put(
        `${sectionPartsPathConfigs.partById}/${id}/content`,
        { content: data },
        cancelToken,
      )
      .then((r) => {
        if (r?.success) {
          return `${r.data}`
        } else {
          return undefined
        }
      })
  }
  deleteSessionPart(id: number, cancelToken?: any) {
    return super
      .delete(
        `${sectionPartsPathConfigs.listByType}/${id}`,
        undefined,
        cancelToken,
      )
      .then((r) => {
        return r?.success
      })
  }
}
