import { Box, Typography, Button, Divider } from '@mui/material'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { selectPartsBySectionId } from '@/features/exam/attemptSelector.ts'
import { useSelector } from 'react-redux'
import QuizIcon from '@mui/icons-material/Quiz'
const AttemptQuestionMenu = () => {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const sectionId = searchParams.get('sectionId') || ''
  const parts = useSelector(selectPartsBySectionId(sectionId))

  const handlePartClick = (partId: number) => {
    const newSearchParams = new URLSearchParams(searchParams)
    newSearchParams.set('partId', String(partId))
    navigate(`?${newSearchParams.toString()}`)
  }
  return (
    <>
      <Box
        sx={{
          padding: 2,
          width: '100%',
          border: '1px solid #ccc',
          borderRadius: '8px',
        }}
      >
        <Typography
          sx={{ marginBottom: 2, fontWeight: 'bold', color: 'text.secondary' }}
        >
          Bảng câu hỏi
        </Typography>
        <Divider sx={{ marginBottom: 2 }} />
        {parts.map((part) => (
          <Box key={part.id} sx={{ marginBottom: 3 }}>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                marginBottom: 1,
                cursor: 'pointer',
              }}
              onClick={() => handlePartClick(part.id)}
            >
              <QuizIcon sx={{ marginRight: 1, color: 'text.secondary' }} />
              <Typography
                color="text.secondary"
                component="a"
                fontWeight={'bold'}
                sx={{
                  textDecoration: 'none',
                  fontWeight: 'bold',
                  whiteSpace: 'nowrap',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  maxWidth: '100%',
                  '&:hover': { textDecoration: 'underline' },
                }}
              >
                {part.title}
              </Typography>
            </Box>
            <Box
              sx={{
                display: 'flex',
                flexWrap: 'wrap',
                gap: 1,
              }}
            >
              {part.questions.map((question) => (
                <Button
                  key={question.id}
                  variant="contained"
                  size="small"
                  onClick={() => {}}
                  sx={{
                    backgroundColor: question.completed
                      ? '#4caf50'
                      : question.completed
                        ? '#f44336'
                        : '#9e9e9e',
                    color: '#fff',
                    '&:hover': {
                      backgroundColor: question.completed
                        ? '#388e3c'
                        : question.completed
                          ? '#d32f2f'
                          : '#757575',
                    },
                    borderRadius: '4px',
                    minWidth: '40px',
                    textAlign: 'center',
                  }}
                >
                  {question.id}
                </Button>
              ))}
            </Box>
          </Box>
        ))}
      </Box>
    </>
  )
}

export default AttemptQuestionMenu
