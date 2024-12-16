import { AttemptStatusType, SectionPartTypes } from './configs'
export interface IExam {
  id: number
  title: string
  duration: number
  createdAt?: string
  description: string
}
export interface Question {
  orderNum: number
  point: number
  id: number
  type: 'multiple-choice' | 'essay' | 'audio' | 'speaking'
  questionText: string
  options?: string[]
  correctAnswer?: string
  audioUrl?: string
  speakingPrompt?: string
}
export interface QuestionOption {
  content: string
  isCorrect: any
  orderNum: string
  id: number
}
export enum SectionType {
  Listening = 1,
  Reading = 2,
  Writing = 3,
  Speaking = 4,
}
//ROMIO
interface ISessionPartBase {
  title: string
  instructions: string
  content: string
  orderNum: number
  sectionType: number
  examId: number
  type: number
}
export interface ISessionPart extends ISessionPartBase {
  id: number
}
export interface IReqPostSessionPart extends ISessionPartBase {
  parentId?: number
}

export interface IOption {
  id: number
  content: string
  isCorrect: boolean
}

export interface IQuestion {
  id: number
  questionText: string
  point: number
  options: IOption[]
}

export interface ISumaryAttemptExam {
  id: string
  title: string
  description?: string
  duration: number
}

export interface IAttemptExam {
  id: number
  title: string
  instructions?: string
  sectionType: number
  parts: IAttemptPart[]
}

export interface IAttemptPart {
  id: number
  title: string
  content?: string
  instructions?: string
  type: SectionPartTypes
  questions: IAttemptQuestion[]
}

export interface IAttemptQuestion {
  id: number
  questionText: string
  options: IAttemptOption[]
}

export interface IAttemptOption {
  id: number
  content: string
  chosen?: boolean
}

export interface IStartStudentAttempt {
  attempId: number
  examId: number
  title: string
  description: string
  duration: number
  startTime: string
  status: AttemptStatusType
}

export interface ISummaryStudentAttempt {
  examId: number
  examTitle?: string
  examDescription?: string
  inprocess: IStartStudentAttempt
  attempts: ISummaryAttempt[]
}
export interface IAttemptAnswer {
  id: number
  answer: number | string | File
}

export interface ISummaryAttempt {
  id: number
  startTime: string
  endTime?: string
  finalScore: number
}
export interface IAttemptStudentAnswer {
  sectionType: SectionType
  partType: SectionPartTypes
  partId: number
  questions: IAttemptAnswer[]
}

export interface ISubmitStudentAttempt {
  attemptId: number
  scope: ISubmitStudentAttemptScope
  scores: ISubmitStudentAttemptScore
  submittedCount: number
  validationErrors: null | any
}
interface ISubmitStudentAttemptScope {
  type: number
  sectionPartId: number
  title: string
}
interface ISubmitStudentAttemptScore {
  TotalPoints: number
  EarnedPoints: number
  Percentage: number
}

export interface IErrorAPI {
  success: boolean
  message: string
}
export interface IReviewSectionScores {
  Listening: number
  Reading: number
  Writing: number
  Speaking: number
}

export interface IReviewWritingScore {
  taskAchievement: number
  coherenceCohesion: number
  lexicalResource: number
  grammarAccuracy: number
  totalScore: number
}

export interface IReviewAnswer {
  id: number
  questionId: number
  questionText: string
  passageTitle: string
  passageContent: string
  questionOptionId: number | null
  essayAnswer: string | null
  aiFeedback: string | null
  score: number | null
  isCorrect: boolean
  sectionType: number
  writingScore: IReviewWritingScore | null
  speakingScore: number | null
}

export interface IReviewResultData {
  id: number
  examTitle: string
  startTime: string
  endTime: string
  sectionScores: IReviewSectionScores
  finalScore: number
  answers: IReviewAnswer[]
}
