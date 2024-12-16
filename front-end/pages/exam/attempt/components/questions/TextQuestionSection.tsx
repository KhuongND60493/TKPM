import { Box, Typography } from '@mui/material'
import { IAttemptQuestion } from '@/features/exam/type'
import ReactQuill from 'react-quill'
import 'react-quill/dist/quill.snow.css'
import { useSelector, useDispatch } from 'react-redux'
import { selectAnswerWritingSpeaking } from '@/features/exam/attemptSelector'
import { saveAnswer } from '@/features/exam/attemptSlice.ts'
interface SingleQuestionSectionProps {
  questions: IAttemptQuestion[]
}
const TextQuestionSection = (props: SingleQuestionSectionProps) => {
  const { questions } = props
  const dispatch = useDispatch()
  const answerDictionary = useSelector(selectAnswerWritingSpeaking)
  const handleInputChange = (questionId: number, value: string) => {
    dispatch(saveAnswer({ id: questionId, answer: value }))
  }
  return questions.map((question, index) => (
    <Box
      key={index}
      sx={{
        marginBottom: 3,
        padding: 2,
        borderRadius: '8px',
        backgroundColor: '#E3F2FD',
      }}
    >
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 1,
        }}
      >
        <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
          Câu hỏi {index + 1}
        </Typography>
      </Box>
      <Typography
        sx={{
          whiteSpace: 'pre-wrap',
          marginBottom: 1,
          color: '#37474F',
        }}
      >
        {question.questionText}
      </Typography>
      <ReactQuill
        theme="snow"
        onChange={(value) => handleInputChange(question.id, value)}
        value={answerDictionary[question.id] || ''}
        modules={{
          toolbar: [
            [{ header: [1, 2, 3, false] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ list: 'ordered' }, { list: 'bullet' }],
            ['link', 'image'],
            ['clean'],
          ],
        }}
        formats={[
          'header',
          'bold',
          'italic',
          'underline',
          'strike',
          'list',
          'bullet',
          'link',
          'image',
        ]}
        style={{
          backgroundColor: '#FFFFFF',
          borderRadius: '8px',
        }}
      />
    </Box>
  ))
}
export default TextQuestionSection
