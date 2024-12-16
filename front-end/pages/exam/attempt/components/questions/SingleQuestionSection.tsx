import {
  Box,
  FormControl,
  FormControlLabel,
  Radio,
  RadioGroup,
  Typography,
} from '@mui/material'
import { useDispatch, useSelector } from 'react-redux'
import { saveAnswer } from '@/features/exam/attemptSlice'
import { IAttemptQuestion } from '@/features/exam/type'
import { selectQuestionsWithAnswers } from '@/features/exam/attemptSelector.ts'

interface SingleQuestionSectionProps {
  questions: IAttemptQuestion[]
}

const SingleQuestionSection = ({ questions }: SingleQuestionSectionProps) => {
  const dispatch = useDispatch()
  const qu = useSelector(selectQuestionsWithAnswers(questions))
  const handleOptionSelect = (questionId: number, optionId: number) => {
    dispatch(saveAnswer({ id: questionId, answer: optionId }))
  }

  return (
    <>
      {qu.map((question) => (
        <Box
          key={question.id}
          sx={{
            marginBottom: 3,
            padding: 2,
            borderRadius: '8px',
            backgroundColor: '#E3F2FD',
          }}
        >
          <Typography
            variant="subtitle1"
            sx={{ fontWeight: 'bold', marginBottom: 1 }}
          >
            Câu hỏi {question.id}
          </Typography>
          <Typography
            sx={{
              whiteSpace: 'pre-wrap',
              marginBottom: 1,
              color: '#37474F',
            }}
          >
            {question.questionText}
          </Typography>

          <FormControl component="fieldset">
            <RadioGroup
              value={
                question.options
                  .find((option) => option.chosen)
                  ?.id.toString() || ''
              }
              onChange={(event) => {
                const selectedOptionId = parseInt(event.target.value, 10)
                handleOptionSelect(question.id, selectedOptionId)
              }}
            >
              {question.options.map((option) => (
                <FormControlLabel
                  key={option.id}
                  value={option.id.toString()}
                  control={<Radio />}
                  label={option.content}
                />
              ))}
            </RadioGroup>
          </FormControl>
        </Box>
      ))}
    </>
  )
}

export default SingleQuestionSection
