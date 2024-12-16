import { useEffect, useState, forwardRef, useImperativeHandle } from 'react'
import { List, Box } from '@mui/material'
import QuestionItem from './QuestionItem'
import { IOption, IQuestion, ISessionPart } from '@/features/exam/type'
import { questionRequest } from '@/app/api'
import { toast } from 'react-hot-toast'

export interface QuestionManagerHandle {
  addQuestion: () => Promise<void>
}

interface QuestionManagerProps {
  part: ISessionPart
}

const QuestionManager = forwardRef<QuestionManagerHandle, QuestionManagerProps>(
  (props, ref) => {
    const { part } = props
    const [questions, setQuestions] = useState<IQuestion[]>([])
    const [openQuestionId, setOpenQuestionId] = useState<number | null>(null)

    useEffect(() => {
      void fetchQuestions()
    }, [part.id])

    const fetchQuestions = async () => {
      const response = await questionRequest.questionsByPartId(part.id)
      if (response && response.length > 0) {
        void setQuestions(response)
      } else {
        setQuestions([])
      }
    }
    useImperativeHandle(ref, () => ({
      addQuestion: async () => {
        try {
          const response = await questionRequest.createEmptyQuestion(part.id)
          if (response) {
            toast.success('Tạo câu hỏi mới thành công')
            setQuestions((prevQuestions) => [...prevQuestions, response])
          } else {
            toast.error('Tạo câu hỏi mới thất bại')
          }
        } catch (e) {
          console.error('Error adding new question:', e)
        }
      },
    }))

    const handleClickQuestion = (questionId: number) => {
      setOpenQuestionId(openQuestionId === questionId ? null : questionId)
    }
    const handleRemoveQuestion = async (questionId: number) => {
      try {
        const response = await questionRequest.deleteQuestion(questionId)
        if (response) {
          toast.success('Xóa câu hỏi thành công')
          setQuestions((prevQuestions) =>
            prevQuestions.filter((question) => question.id !== questionId),
          )
        } else {
          toast.error('Xóa câu hỏi thất bại')
        }
      } catch (e) {
        console.error('Error removing question:', e)
        toast.error('Đã xảy ra lỗi khi xóa câu hỏi')
      }
    }
    const handleAddEmptyOption = async (idQuestion: number) => {
      try {
        const response = await questionRequest.createEmptyOption(idQuestion)
        if (response) {
          toast.success('Tạo câu trả lời thành công')
          setQuestions((prevQuestions) =>
            prevQuestions.map((question) => {
              if (question.id === idQuestion) {
                return {
                  ...question,
                  options: [...question.options, response],
                }
              }
              return question
            }),
          )
        } else {
          toast.error('Tạo option thất bại')
        }
      } catch (e) {
        console.error('Error creating empty option:', e)
      }
    }
    const handleRemoveOption = async (questionId: number, optionId: number) => {
      try {
        const response = await questionRequest.deleteOption(optionId)
        if (response) {
          toast.success('Xoá option thành công')
          setQuestions((prevQuestions) =>
            prevQuestions.map((question) => {
              if (question.id === questionId) {
                return {
                  ...question,
                  options: question.options.filter(
                    (option) => option.id !== optionId,
                  ),
                }
              }
              return question
            }),
          )
        } else {
          toast.error('Xoá option thất bại')
        }
      } catch (e) {
        console.error('Error removing option:', e)
        toast.error('Đã xảy ra lỗi khi xóa option')
      }
    }
    const handleUpdateQuestion = async (newQuestion: IQuestion) => {
      try {
        const response = await questionRequest.updateQuestion(newQuestion)
        if (response) {
          toast.success('Cập nhật câu hỏi thành công')
          setQuestions((prevQuestions) =>
            prevQuestions.map((question) =>
              question.id === newQuestion.id
                ? { ...question, ...newQuestion }
                : question,
            ),
          )
        } else {
          toast.error('Cập nhật câu hỏi thất bại')
        }
      } catch (error) {
        console.error('Error updating question:', error)
        toast.error('Đã xảy ra lỗi khi cập nhật câu hỏi')
      }
    }

    const handleUpdateOptions = async (
      questionId: number,
      options: IOption[],
    ) => {
      try {
        const response = await questionRequest.updateOptions(
          questionId,
          options,
        )
        if (response) {
          toast.success('Cập nhật các câu trả lời thành công')
          setQuestions((prevQuestions) =>
            prevQuestions.map((question) =>
              question.id === questionId ? { ...question, options } : question,
            ),
          )
        } else {
          toast.error('Cập nhật các câu trả lời thất bại')
        }
      } catch (error) {
        console.error('Error updating options:', error)
        toast.error('Đã xảy ra lỗi trong quá trình cập nhật các câu trả lời')
      }
    }
    const renderItem = (question: IQuestion, index: number) => {
      const opening = openQuestionId === question.id
      return (
        <div key={`${question.id}_${index}`}>
          <QuestionItem
            question={question}
            opening={opening}
            onOpen={handleClickQuestion}
            onRemoveQuestion={handleRemoveQuestion}
            onRemoveOption={handleRemoveOption}
            onAddOption={handleAddEmptyOption}
            onUpdateQuestion={handleUpdateQuestion}
            onUpdateOptions={handleUpdateOptions}
          />
        </div>
      )
    }

    return (
      <Box p={2}>
        <List>{questions.map(renderItem)}</List>
      </Box>
    )
  },
)

export default QuestionManager
