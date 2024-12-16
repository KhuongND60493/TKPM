import {
  Box,
  Button,
  Collapse,
  Divider,
  ListItem,
  ListItemText,
  TextField,
} from '@mui/material'
import OptionList from './OptionList'
import { IOption, IQuestion } from '@/features/exam/type'
import AddCircleIcon from '@mui/icons-material/AddCircle'
import ButtonRemoveQuestion from './ButtonRemoveQuestion'
import { useEffect, useMemo, useState } from 'react'
import SaveIcon from '@mui/icons-material/Save'
import RestoreIcon from '@mui/icons-material/Restore'

interface QuestionItemProps {
  opening: boolean
  onOpen: (id: number) => void
  onUpdateQuestion: (question: IQuestion) => void
  onUpdateOptions: (questionId: number, options: IOption[]) => void
  onRemoveQuestion: (id: number) => void
  onAddOption: (id: number) => void
  onRemoveOption: (questionId: number, id: number) => void
  question: IQuestion
}
const QuestionItem = (props: QuestionItemProps) => {
  const {
    opening,
    onOpen,
    onRemoveQuestion,
    onRemoveOption,
    onUpdateQuestion,
    onUpdateOptions,
    question,
    onAddOption,
  } = props
  const [currentPoint, setCurrentPoint] = useState<number>(question.point)
  const [currentQuestionText, setCurrentQuestionText] = useState<string>(
    question.questionText,
  )
  useEffect(() => {
    if (
      !opening &&
      (question.point !== currentPoint ||
        question.questionText !== currentQuestionText)
    ) {
      setCurrentPoint(question.point)
      setCurrentQuestionText(question.questionText)
    }
  }, [opening])
  const hasChanges = useMemo(() => {
    return (
      currentPoint !== question.point ||
      currentQuestionText !== question.questionText
    )
  }, [currentPoint, currentQuestionText])

  const handleReset = () => {
    setCurrentPoint(question.point)
    setCurrentQuestionText(question.questionText)
  }
  const handleUpdateQuestion = () => {
    onUpdateQuestion({
      ...question,
      point: currentPoint,
      questionText: currentQuestionText,
    })
  }
  return (
    <Box
      sx={{
        border: opening ? '2px dashed #3f51b5' : 'none',
        borderRadius: '4px',
        marginBottom: '8px',
        paddingBottom: 1,
        position: 'relative',
      }}
    >
      {opening && (
        <Box
          position={'absolute'}
          zIndex={1}
          right={5}
          top={5}
          display={'flex'}
          gap={2}
        >
          {hasChanges && (
            <>
              <Button
                variant="outlined"
                color="secondary"
                startIcon={<RestoreIcon />}
                onClick={handleReset}
              >
                Reset
              </Button>
              <Button
                variant="outlined"
                color="warning"
                startIcon={<SaveIcon />}
                onClick={handleUpdateQuestion}
              >
                Cập nhật câu hỏi
              </Button>
            </>
          )}
          <ButtonRemoveQuestion
            onRemove={() => onRemoveQuestion(question.id)}
          />
        </Box>
      )}
      <ListItem
        button
        onClick={() => onOpen(question.id)}
        sx={{
          backgroundColor: opening ? 'lightblue' : 'transparent',
          borderRadius: '4px',
        }}
      >
        <ListItemText primary={question.questionText} />
      </ListItem>
      <Collapse in={opening} timeout="auto" unmountOnExit>
        <Box sx={{ pl: 2, pr: 2 }}>
          <TextField
            label="Điểm số"
            type="number"
            value={currentPoint}
            onChange={(e) => {
              const value = Math.max(0.25, Number(e.target.value))
              setCurrentPoint(value)
            }}
            fullWidth
            margin="normal"
            inputProps={{
              step: 0.25,
              min: 0.25,
            }}
          />
          <TextField
            label="Câu hỏi"
            value={currentQuestionText}
            onChange={(e) => setCurrentQuestionText(e.target.value)}
            fullWidth
            margin="normal"
          />
          <OptionList
            questionId={question.id}
            options={question.options}
            onUpdateOptions={(options: IOption[]) =>
              onUpdateOptions(question.id, options)
            }
            onRemoveOption={onRemoveOption}
          />
          <Button
            variant="outlined"
            color="primary"
            startIcon={<AddCircleIcon />}
            onClick={() => onAddOption(question.id)}
            sx={{
              border: '2px dashed',
              color: 'text.secondary',
              borderColor: 'text.secondary',
              borderRadius: '4px',
              padding: '8px',
              transition: 'background-color 0.3s',
              width: '100%',
              marginBottom: '8px',
            }}
          >
            Tạo mới trả lời
          </Button>
        </Box>
      </Collapse>
      <Divider />
    </Box>
  )
}
export default QuestionItem
