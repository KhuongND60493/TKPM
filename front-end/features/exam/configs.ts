export const examPathConfigs = {
  list: 'exams',
  createEmptyExam: 'exams',
  updateExam: 'exams/{id}',
  deleteExam: 'exams/{id}',
}

export const sectionPartsPathConfigs = {
  listByType: 'section-parts',
  questions: 'section-parts/{partId}/questions',
  parts: 'section-parts/section',
  partById: 'section-parts/part',
}
export const questionPathConfigs = {
  createEmptyOption: 'questions/option',
  createEmptyQuestion: 'questions',
  deleteOption: 'questions/option/{optionId}',
  deleteQuestion: 'questions/{id}',
  updateQuestion: 'questions/{questionId}',
  updateOptions: 'questions/{questionId}/options',
}
export const attemptConfigs = {
  getAttemptByExamId: 'attempts/{examId}',
  getAttemptResult: 'ai/{id}/review',
  startAttempt: 'ai/start',
  finishAttempt: 'ai/{id}/finish',
  getAttempts: 'ai/{id}/attempts',
  sendSubmit: 'ai/{id}/submit',
  getExams: 'ai/exams',
}

export enum SessionType {
  Listening = 1,
  Reading = 2,
  Writing = 3,
  Speaking = 4,
}
export const sessionTypeOptions = [
  { label: 'Nghe', value: SessionType.Listening },
  { label: 'Đọc', value: SessionType.Reading },
  { label: 'Viết', value: SessionType.Writing },
  { label: 'Nói', value: SessionType.Speaking },
]

export enum SectionPartTypes {
  Section = 1,
  Part = 2,
  Passage = 3,
}

export enum AttemptStatusType {
  Started = -1,
  InProgress = 0,
  Completed = 1,
}

export enum KEY_SUBMIT_RESPONSE {
  ANSWER_ATTEMPT_EMPTY = 'ANSWER_ATTEMPT_EMPTY',
  QUESTION_EMPTY = 'QUESTION_EMPTY',
  API_BACK_ERROR = 'API_BACK_ERROR',
  ATTEMPT_NOT_IN_PROGRESS = 'Attempt.NotInProgress',
}
