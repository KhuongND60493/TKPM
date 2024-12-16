import React, { useEffect, useState } from 'react'
import {
  Box,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import { examRequest } from '@/app/api'
import { IExam } from '@/features/exam/type'
import { Quiz } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import AddCircleIcon from '@mui/icons-material/AddCircle'
import { toast } from 'react-hot-toast'
import EditExamDialog from './components/EditExamDialog'
import ConfirmDeleteDialog from './components/ConfirmDeleteDialog'

const ExamManagement: React.FC = () => {
  const [exams, setExams] = useState<IExam[]>([])
  const [openDialog, setOpenDialog] = useState(false)
  const [editExam, setEditExam] = useState<IExam | null>(null)
  const [deleteExamId, setDeleteExamId] = useState<number | null>(null)

  const navigate = useNavigate()
  useEffect(() => {
    void handleGetExams()
  }, [])
  const handleGetExams = async () => {
    const data = await examRequest.exams()
    if (data) {
      setExams(data)
    }
  }
  const handleOpenDialog = (exam?: IExam) => {
    if (exam) setEditExam(exam)
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setEditExam(null)
    setOpenDialog(false)
  }

  const handleSaveExam = async (editedExam: IExam) => {
    try {
      const updatedExam = await examRequest.updateExam(
        editedExam.id,
        editedExam,
      )
      if (updatedExam) {
        toast.success('Cập nhật đề thi thành công')
        setExams((prevExams) =>
          prevExams.map((exam) =>
            exam.id === updatedExam.id ? updatedExam : exam,
          ),
        )

        handleCloseDialog()
      } else {
        toast.error('Cập nhật đề thi thất bại')
      }
    } catch (e) {}
  }
  const handleManageQuestions = (id: number) => {
    navigate(`/admin/questions/${id}`)
  }

  const handleCreateNewExam = async () => {
    try {
      const rs = await examRequest.createNewExam()
      if (rs) {
        setExams((prevState) => [rs, ...prevState])
        toast.success('Tạo đề thi thành công')
      } else {
        toast.error('Tạo đề thi thất bại')
      }
    } catch (e) {}
  }
  const handleDeleteExam = async (id: number) => {
    try {
      const response = await examRequest.deleteExam(id) // Gọi API để xóa
      if (response?.success) {
        setExams((prevExams) => prevExams.filter((exam) => exam.id !== id))
        toast.success('Đã xóa đề thi thành công')
      } else {
        toast.error(response?.message || 'Không thể xóa đề thi')
      }
    } catch (e) {
      toast.error('Không thể xóa đề thi. Vui lòng thử lại.')
    } finally {
      setDeleteExamId(null)
    }
  }
  const handleOpenDeleteDialog = (id: number) => {
    setDeleteExamId(id)
  }

  const handleCloseDeleteDialog = () => {
    setDeleteExamId(null)
  }

  return (
    <Box sx={{ p: 3 }}>
      <Box display={'flex'} justifyContent={'flex-end'}>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<AddCircleIcon />}
          onClick={() => handleCreateNewExam()}
          sx={{
            border: '2px dashed',
            color: 'text.secondary',
            borderColor: 'text.secondary',
            borderRadius: '4px',
            padding: '8px',
            transition: 'background-color 0.3s',
          }}
        >
          Tạo mới đề thi
        </Button>
      </Box>

      <TableContainer sx={{ mt: 3 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell></TableCell>
              <TableCell>Tên đề thi</TableCell>
              <TableCell>Mô tả</TableCell>
              <TableCell align="center">Tác vụ</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {exams.map((exam) => (
              <TableRow key={exam.id}>
                <TableCell>
                  <Tooltip title="Manage Questions">
                    <Button
                      variant="contained"
                      color="secondary"
                      startIcon={<Quiz />}
                      onClick={() => handleManageQuestions(exam.id)}
                      sx={{ mb: 1 }}
                    >
                      Câu hỏi
                    </Button>
                  </Tooltip>
                </TableCell>
                <TableCell>{exam.title}</TableCell>
                <TableCell>{exam.description}</TableCell>
                <TableCell align="center">
                  <IconButton
                    color="primary"
                    onClick={() => handleOpenDialog(exam)}
                  >
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    color="error"
                    onClick={() => handleOpenDeleteDialog(exam.id)}
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      {openDialog && editExam && (
        <EditExamDialog
          open={openDialog}
          handleClose={handleCloseDialog}
          currentExam={editExam}
          onSave={handleSaveExam}
        />
      )}
      {!!deleteExamId && (
        <ConfirmDeleteDialog
          open={!!deleteExamId}
          title="Bạn có chắc chắn muốn xóa đề thi này?"
          onConfirm={() => handleDeleteExam(deleteExamId!)}
          onClose={handleCloseDeleteDialog}
        />
      )}
    </Box>
  )
}

export default ExamManagement
