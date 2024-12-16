import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { IExam, ISessionPart } from './type'
export interface IAdminExam {
  exam?: IExam
  section?: ISessionPart
}
const initialState: IAdminExam = { exam: undefined }
export const ExamAdminSlice = createSlice({
  name: 'examAdmin',
  initialState,
  reducers: {
    manageExam: (state, action: PayloadAction<IExam>) => {
      state.exam = action.payload
    },
    manageSection: (state, action: PayloadAction<ISessionPart | undefined>) => {
      state.section = action.payload
    },
    resetExam: (state) => {
      state.exam = undefined
      state.section = undefined
    },
  },
})
export const { manageExam, manageSection, resetExam } = ExamAdminSlice.actions
export const examAdminReducer = ExamAdminSlice.reducer
