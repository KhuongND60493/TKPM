import React, { useState, useEffect } from 'react'
import {
  Modal,
  Backdrop,
  Fade,
  Box,
  Typography,
  CircularProgress,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material'
import { toast } from 'react-hot-toast'
import { attemptRequest } from '@/app/api'
import { IReviewAnswer } from '@/features/exam/type.ts'

interface ReviewModalProps {
  open: boolean
  onClose: () => void
  attemptId: string | null
}

const ReviewModal: React.FC<ReviewModalProps> = ({
  open,
  onClose,
  attemptId,
}) => {
  const [loading, setLoading] = useState<boolean>(false)
  const [resultData, setResultData] = useState<any>(null)

  useEffect(() => {
    if (open && attemptId) {
      void fetchResult(attemptId)
    }
  }, [open, attemptId])

  const fetchResult = async (attemptId: string) => {
    setLoading(true)
    try {
      const response = await attemptRequest.getAttemptResultById(attemptId)
      if (response) {
        setResultData(response)
      }
    } catch (error) {
      toast.error('Không thể tải kết quả')
    } finally {
      setLoading(false)
    }
  }
  const renderResultCell = (answer: IReviewAnswer) => {
    if (answer.sectionType === 1 || answer.sectionType === 2) {
      // Listening & Reading
      return answer.isCorrect ? 'Đúng' : 'Sai'
    } else if (answer.sectionType === 3 && answer.writingScore) {
      // Writing
      return (
        <Box>
          <Typography variant="body2" color="textSecondary">
            {`Task: ${answer.writingScore.taskAchievement} | Cohesion: ${answer.writingScore.coherenceCohesion}`}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            {`Lexical: ${answer.writingScore.lexicalResource} | Grammar: ${answer.writingScore.grammarAccuracy}`}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            {`Total: ${answer.writingScore.totalScore}`}
          </Typography>
          <Typography variant="caption" color="primary">
            {answer.aiFeedback || 'No Feedback'}
          </Typography>
        </Box>
      )
    } else if (answer.sectionType === 4) {
      // Speaking
      return (
        <a
          href={answer.aiFeedback || '#'}
          target="_blank"
          rel="noopener noreferrer"
        >
          Nghe Feedback
        </a>
      )
    }
    return 'N/A'
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      closeAfterTransition
      BackdropComponent={Backdrop}
      BackdropProps={{ timeout: 500 }}
    >
      <Fade in={open}>
        <Box
          sx={{
            position: 'absolute',
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            width: 900,
            maxHeight: '90vh',
            overflowY: 'auto',
            bgcolor: 'background.paper',
            boxShadow: 24,
            p: 4,
            borderRadius: 2,
          }}
        >
          {loading ? (
            <Box display="flex" justifyContent="center">
              <CircularProgress />
            </Box>
          ) : resultData ? (
            <Box>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Kết quả bài thi: {resultData.examTitle}
              </Typography>
              <Typography variant="body1" gutterBottom>
                Bắt đầu: {new Date(resultData.startTime).toLocaleString()}
              </Typography>
              <Typography variant="body1" gutterBottom>
                Kết thúc: {new Date(resultData.endTime).toLocaleString()}
              </Typography>
              <Typography variant="body1" gutterBottom>
                Điểm tổng: {resultData.finalScore} / 10
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Listening: {resultData.sectionScores?.Listening || 0}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Reading: {resultData.sectionScores?.Reading || 0}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Writing: {resultData.sectionScores?.Writing || 0}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Speaking: {resultData.sectionScores?.Speaking || 0}
              </Typography>
              <TableContainer component={Paper} sx={{ maxHeight: 400, mt: 2 }}>
                <Table stickyHeader>
                  <TableHead>
                    <TableRow>
                      <TableCell align="center">#</TableCell>
                      <TableCell align="center">Câu hỏi</TableCell>
                      <TableCell align="center">Nội dung</TableCell>
                      <TableCell align="center">Điểm</TableCell>
                      <TableCell align="center">Kết quả</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {resultData.answers.map(
                      (answer: IReviewAnswer, index: number) => (
                        <TableRow key={answer.id}>
                          <TableCell align="center">{index + 1}</TableCell>
                          <TableCell align="center">
                            {answer.questionText}
                          </TableCell>
                          <TableCell align="center">
                            {answer.passageTitle}
                          </TableCell>
                          <TableCell align="center">
                            {answer.writingScore?.totalScore ||
                              answer.score ||
                              'N/A'}
                          </TableCell>
                          <TableCell align="center">
                            {renderResultCell(answer)}
                          </TableCell>
                        </TableRow>
                      ),
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
              <Box mt={2} display="flex" justifyContent="flex-end">
                <Button variant="contained" color="primary" onClick={onClose}>
                  Đóng
                </Button>
              </Box>
            </Box>
          ) : (
            <Typography textAlign="center" color="error">
              Không tìm thấy dữ liệu kết quả
            </Typography>
          )}
        </Box>
      </Fade>
    </Modal>
  )
}

export default ReviewModal
