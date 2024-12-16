import { useEffect, useState } from 'react'
import { toast } from 'react-hot-toast'
import { attemptRequest } from '@/app/api'
import { useNavigate, useLocation, useParams } from 'react-router-dom'
import { IAttemptExam, ISummaryStudentAttempt } from '@/features/exam/type'
import { AttemptStatusType } from '@/features/exam/configs'
import { useDispatch } from 'react-redux'
import { setAttempt } from '@/features/exam/attemptSlice'
import {
  Box,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  Container,
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import ReviewModal from '@/pages/exam/attempt/components/attempts/ReviewModal.tsx'

const AttemptStudent = () => {
  const dispatch = useDispatch()
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const location = useLocation()
  const [examConfigs, setExamConfigs] = useState<IAttemptExam[]>([])
  const [examAttempt, setExamAttempt] = useState<
    ISummaryStudentAttempt | undefined
  >(undefined)
  const [isLoading, setIsLoading] = useState(true)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [selectedAttemptId, setSelectedAttemptId] = useState<string | null>(
    null,
  )

  useEffect(() => {
    if (!id || isNaN(Number(id))) {
      toast.error('Invalid exam ID!')
      navigate('/exam')
      return
    }
    function getDataStudentExam() {
      if (id) {
        setIsLoading(true)
        Promise.allSettled([
          attemptRequest.getAttemptByExamId(id),
          attemptRequest.getSummaryAttemptsByExamId(id),
        ])
          .then((rs) => {
            const [rsExam, rsAttempt] = rs
            if (rsExam.status === 'fulfilled') {
              handleExamConfig(rsExam.value)
            }
            if (rsAttempt.status === 'fulfilled') {
              handleExamAttempt(rsAttempt.value)
            }
          })
          .finally(() => {
            setIsLoading(false)
          })
      }
    }
    getDataStudentExam()
  }, [id, navigate])
  const navigateToStart = async () => {
    if (examAttempt) {
      const rsStart = await attemptRequest.startAttempt(`${id}`)
      if (rsStart) {
        const currentPath = location.pathname
        const newPath = `${currentPath}/start`
        dispatch(
          setAttempt({
            examId: parseInt(`${id}`),
            attempt: rsStart,
            sections: examConfigs,
          }),
        )
        navigate(newPath)
      } else {
        toast.error('Không thể tạo bài thi. Thử lại nhé')
      }
    } else {
      toast.error('Hiện tại đang thiếu thông tin để start bài thi')
    }
  }

  const handleOpenReviewModal = (attemptId: string) => {
    setSelectedAttemptId(attemptId)
    setIsModalOpen(true)
  }

  const handleCloseReviewModal = () => {
    setIsModalOpen(false)
    setSelectedAttemptId(null)
  }
  const handleExamConfig = (response: IAttemptExam[] | undefined) => {
    try {
      if (!response) {
        toast.error('Invalid exam ID!')
        // navigate('/exam')
        return
      } else {
        setExamConfigs(response)
      }
    } catch (error) {
      toast.error('Không tìm thấy đề thi')
      // navigate('/exam')
    }
  }

  const handleExamAttempt = (response: ISummaryStudentAttempt | undefined) => {
    try {
      if (!response) {
        toast.error('Không tạo được bài thi')
        // navigate('/exam')
        return
      } else {
        setExamAttempt(response)
      }
    } catch (error) {
      toast.error('Không tìm thấy đề thi')
      // navigate('/exam')
    }
  }
  console.log('aaaa', examAttempt)
  if (isLoading) return <Box></Box>
  if (!examAttempt) return <Box>Không tạo được bài thi</Box>
  return (
    <Container
      sx={{
        margin: '0 auto',
        padding: 4,
      }}
    >
      <ReviewModal
        open={isModalOpen}
        onClose={handleCloseReviewModal}
        attemptId={selectedAttemptId}
      />
      <Box display="flex" alignItems="center">
        <Button
          variant="text"
          color="darkColor"
          onClick={() => navigate('/exam')}
          startIcon={<ArrowBackIcon />}
          sx={{ textTransform: 'none', mr: 2 }}
        />
        <Typography variant="h4" fontWeight="bold" gutterBottom>
          {examAttempt.examTitle}
        </Typography>
      </Box>

      <Typography variant="body1" color="textSecondary" gutterBottom>
        {examAttempt.examDescription}
      </Typography>
      <Box mt={4}>
        <Typography variant="h5" gutterBottom>
          Tổng quan các lần làm bài trước của bạn
        </Typography>
        <TableContainer
          component={Paper}
          sx={{
            maxHeight: 400,
          }}
        >
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                <TableCell align="center">Lần làm</TableCell>
                <TableCell align="center">Trạng thái</TableCell>
                <TableCell align="center">Điểm</TableCell>
                <TableCell align="center">Xem lại</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {examAttempt.attempts.length > 0 ? (
                examAttempt.attempts.map((attempt, index) => (
                  <TableRow key={index}>
                    <TableCell align="center">{index + 1}</TableCell>
                    <TableCell align="center">Kết thúc</TableCell>
                    <TableCell align="center">
                      {attempt.finalScore}/10
                    </TableCell>
                    <TableCell align="center">
                      <Button
                        variant="text"
                        onClick={() => handleOpenReviewModal(attempt.id)}
                      >
                        Xem kết quả
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell align="center" colSpan={4}>
                    Bạn chưa có kết quả nào
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>

      <Box
        mt={4}
        display="flex"
        justifyContent="center"
        alignItems="center"
        gap={2}
      >
        {examAttempt?.inprocess?.status === AttemptStatusType.Started ? (
          <>
            <Button
              variant="contained"
              color="warning"
              fullWidth
              onClick={navigateToStart}
            >
              Tiếp tục làm bài thi
            </Button>
            <Button
              variant="outlined"
              color="success"
              fullWidth
              onClick={() => toast.success('Bài làm đã kết thúc!')}
            >
              Kết thúc bài làm
            </Button>
          </>
        ) : (
          <Button
            variant="contained"
            color="success"
            fullWidth
            onClick={navigateToStart}
          >
            Bắt đầu mới bài thi
          </Button>
        )}
      </Box>
    </Container>
  )
}

export default AttemptStudent
