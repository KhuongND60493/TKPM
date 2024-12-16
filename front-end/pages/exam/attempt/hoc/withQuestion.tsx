import React from 'react'
import { IAttemptPart } from '@/features/exam/type'
import {
  Box,
  FormControlLabel,
  Radio,
  RadioGroup,
  Typography,
} from '@mui/material'
interface WithPartProps {}

function withQuestion<
  P extends {
    part: IAttemptPart
    answers: Record<string, string>
    onAnswer: (questionId: number, answer: number) => void
  },
>(WrappedComponent: React.ComponentType<P & WithPartProps>) {
  return (props: P) => {
    const { part, answers, onAnswer } = props
    return (
      <Box p={2} position={'relative'}>
        <WrappedComponent {...props} />
        {part.questions.map((question) => (
          <Box key={question.id} sx={{ mt: 2 }}>
            <Typography variant="body2" gutterBottom>
              {question.questionText}
            </Typography>
            <RadioGroup
              value={answers[question.id] || ''}
              onChange={(e) =>
                onAnswer(question.id, parseInt(`${e.target.value}`))
              }
            >
              {question.options?.map((option, index) => (
                <FormControlLabel
                  key={index}
                  value={option.id}
                  control={<Radio />}
                  label={option.content}
                />
              ))}
            </RadioGroup>
          </Box>
        ))}
      </Box>
    )
  }
}

export default withQuestion
