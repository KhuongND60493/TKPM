import React, { useEffect, useState } from 'react'
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Grid,
  Typography,
  Container,
} from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { attemptRequest } from '@/app/api'
import { ISumaryAttemptExam } from '@/features/exam/type.ts'
import { useDispatch } from 'react-redux'
import { AppDispatch } from '@/app/store'
import { resetAnswer } from '@/features/exam/attemptSlice.ts'

const ExamList: React.FC = () => {
  const dispatch: AppDispatch = useDispatch()
  const navigate = useNavigate()
  const [exams, setExams] = useState<ISumaryAttemptExam[]>([])
  useEffect(() => {
    void fetchStudentExam()
  }, [])
  const fetchStudentExam = async () => {
    const response = await attemptRequest.getExams()
    if (response && response.length > 0) {
      setExams(response)
    } else {
      setExams([])
    }
  }
  const handleStartExam = (id: string) => {
    void dispatch(resetAnswer())
    navigate(`/exam/${id}/attempts`)
  }
  return (
    <Container maxWidth="lg">
      <Box sx={{ padding: 3 }}>
        <Grid container spacing={3}>
          {exams.map((exam) => (
            <Grid item xs={12} sm={6} md={4} key={exam.id}>
              <Card variant="outlined">
                <CardHeader
                  title={exam.title}
                  subheader={`Thời gian thi:${exam.duration} phút`}
                />
                <CardContent>
                  <Typography variant="body2" color="textSecondary">
                    {exam.description}
                  </Typography>
                </CardContent>
                <CardActions>
                  <Button
                    size="small"
                    color="success"
                    variant="contained"
                    onClick={() => handleStartExam(exam.id)}
                  >
                    Bắt đầu bài thi
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Box>
    </Container>
  )
}

export default ExamList
