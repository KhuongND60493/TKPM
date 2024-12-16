import { createSelector } from '@reduxjs/toolkit'
import { IAttemptExam, IAttemptQuestion } from '@/features/exam/type'

import { RootState } from '@/app/store'

export const selectSections = createSelector(
  [(state: RootState) => state.examStudent.sections],
  (sections) =>
    (sections || []).map((section: IAttemptExam) => ({
      id: section.id,
      title: section.title,
      instructions: section.instructions,
      sectionType: section.sectionType,
    })),
)

export const selectPartsBySectionId = (sectionId: string | null) =>
  createSelector(
    [(state: RootState) => state.examStudent?.sections],
    (sections) => {
      if (!sections || !sectionId || sections?.length === 0) return []
      const sectionIdNeedFind = Number(sectionId)
      const section = sections.find((sec) => sec.id === sectionIdNeedFind)
      return section
        ? section.parts.map((_part) => {
            const dataQuestions =
              _part.questions && _part.questions.length > 0
                ? _part.questions.map((x, index) => {
                    return { title: `${index + 1}`, id: x.id, completed: false }
                  })
                : []
            return {
              id: _part.id,
              title: _part.title,
              questions: dataQuestions,
            }
          })
        : []
    },
  )

export const selectPartBySectionAndPartId = (
  sectionId: number,
  partId: number | null,
) =>
  createSelector(
    [
      (state: RootState) => state.examStudent?.attempt,
      (state: RootState) => state.examStudent?.sections,
    ],
    (attempt, sections) => {
      if (!sections || !sectionId || sections.length === 0)
        return {
          part: null,
          sectionType: 0,
          attemptId: 0,
        }
      const section = sections.find((sec) => sec.id === sectionId)
      if (!section)
        return {
          part: null,
          sectionType: 0,
          attemptId: 0,
        }
      const part = partId ? section.parts.find((p) => p.id === partId) : null
      return {
        part: part || null,
        sectionType: section.sectionType,
        attemptId: attempt?.attempId,
      }
    },
  )
export const selectPreviousNextPart = (
  currentSectionId: number,
  currentPartId: number | null,
) =>
  createSelector(
    [(state: RootState) => state.examStudent.sections],
    (sections) => {
      if (
        !sections ||
        sections.length === 0 ||
        !currentSectionId ||
        !currentPartId
      )
        return { previousPart: null, nextPart: null }
      const currentSectionIndex = sections.findIndex(
        (section) => section.id === currentSectionId,
      )
      if (currentSectionIndex === -1)
        return { previousPart: null, nextPart: null }

      const currentSection = sections[currentSectionIndex]
      const currentPartIndex = currentSection.parts.findIndex(
        (part) => part.id === currentPartId,
      )
      if (currentPartIndex === -1) return { previousPart: null, nextPart: null }
      let previousPart = null
      if (currentPartIndex > 0) {
        previousPart = {
          sectionId: currentSection.id,
          partId: currentSection.parts[currentPartIndex - 1].id,
        }
      } else if (currentSectionIndex > 0) {
        const previousSection = sections[currentSectionIndex - 1]
        if (previousSection.parts.length > 0) {
          previousPart = {
            sectionId: previousSection.id,
            partId: previousSection.parts[previousSection.parts.length - 1].id,
          }
        }
      }

      let nextPart = null
      if (currentPartIndex + 1 < currentSection.parts.length) {
        nextPart = {
          sectionId: currentSection.id,
          partId: currentSection.parts[currentPartIndex + 1].id,
        }
      } else if (currentSectionIndex + 1 < sections.length) {
        const nextSection = sections[currentSectionIndex + 1]
        if (nextSection.parts.length > 0) {
          nextPart = {
            sectionId: nextSection.id,
            partId: nextSection.parts[0].id,
          }
        }
      }
      return { previousPart, nextPart }
    },
  )

export const selectQuestionsWithAnswers = (questions: IAttemptQuestion[]) =>
  createSelector(
    [(state: RootState) => state.examStudent?.answer?.questions],
    (answers) => {
      return questions.map((question) => {
        const savedAnswer = (answers || []).find(
          (answer) => answer.id === question.id,
        )
        const options = question.options?.map((op) => {
          if (op.id === savedAnswer?.answer) {
            return { ...op, chosen: true }
          }
          return { ...op, chosen: false }
        })
        return {
          ...question,
          options: options,
        }
      })
    },
  )

export const selectStudentAnswer = createSelector(
  [(state: RootState) => state.examStudent?.answer],
  (answer) => {
    return answer
  },
)

export const selectAnswerWritingSpeaking = createSelector(
  [(state: RootState) => state.examStudent?.answer?.questions],
  (answers) => {
    const answerDictionary: { [key: number]: string } = {}
    ;(answers || []).forEach((answer) => {
      answerDictionary[answer.id] = (answer.answer as string) || ''
    })
    return answerDictionary
  },
)
