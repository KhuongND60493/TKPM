import { useRef, useState } from 'react'
import { Box, Button, Dialog, DialogContent, DialogTitle } from '@mui/material'
import { sectionPartRequest } from '@/app/api'
import { toast } from 'react-hot-toast'
import { ISessionPart } from '@/features/exam/type.ts'
import PartForm, {
  FormDataPart,
} from '@/pages/admin/question/components/part/PartForm.tsx'
import EditIcon from '@mui/icons-material/Edit'
import { useNavigate } from 'react-router-dom'
interface UpdatePartProps {
  part: ISessionPart
}
const ButtonUpdatePart = (props: UpdatePartProps) => {
  const { part } = props
  const navigate = useNavigate()
  const formRef = useRef<any>(null)
  const [openModal, setOpenModal] = useState(false) // Quản lý modal

  const handleOpenModal = () => {
    setOpenModal(true)
  }

  const handleCloseModal = () => {
    setOpenModal(false)
  }

  const handleUpdateContent = async (_data: FormDataPart) => {
    const updated = await sectionPartRequest.updateSessionPart(part.id, {
      title: _data.title,
      instructions: _data.instructions,
      content: part.content,
      orderNum: 0,
      sectionType: part.sectionType,
      type: part.type,
      examId: parseInt(`${part.examId}`),
    })
    if (updated) {
      toast.success('Cập nhật part thành công')
      setTimeout(() => {
        navigate(0)
      }, 200)
    } else {
      toast.error('Lỗi, hãy cập nhật lại')
    }
  }
  const handleClickUpdate = () => {
    if (formRef.current) {
      formRef.current.triggerSubmit()
    }
  }

  return (
    <Box px={2} py={1}>
      <Button
        variant={'outlined'}
        color={'warning'}
        startIcon={<EditIcon />}
        onClick={handleOpenModal}
      >
        Cập nhật hướng dẫn
      </Button>
      <Dialog
        fullWidth
        maxWidth="md"
        open={openModal}
        onClose={handleCloseModal}
      >
        <DialogTitle>Chỉnh sửa part</DialogTitle>
        <DialogContent>
          <PartForm
            ref={formRef}
            data={{ title: part.title, instructions: part.instructions }}
            onClose={handleCloseModal}
            onSubmit={handleUpdateContent}
          />
          <Box
            sx={{ display: 'flex', justifyContent: 'flex-end' }}
            gap={2}
            mt={2}
          >
            <Button onClick={handleCloseModal} color="primary">
              Cancel
            </Button>
            <Button
              variant="outlined"
              color="warning"
              startIcon={<EditIcon />}
              onClick={handleClickUpdate}
            >
              Cập nhật
            </Button>
          </Box>
        </DialogContent>
      </Dialog>
    </Box>
  )
}

export default ButtonUpdatePart
