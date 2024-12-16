import { useState } from 'react'
import { Dialog, DialogContent, DialogTitle, Button, Box } from '@mui/material'
import AddCircleIcon from '@mui/icons-material/AddCircle'
import EditIcon from '@mui/icons-material/Edit'
import SessionForm, { FormDataSession } from './SessionForm'
import { sectionPartRequest } from '@/app/api'
import { SectionPartTypes } from '@/features/exam/configs'

import { toast } from 'react-hot-toast'
interface CreateOrUpdateParentSectionProps {
  id?: number
  examId: string
  onRefresh: (newValue: string) => void
}
const CreateOrUpdateSection = (props: CreateOrUpdateParentSectionProps) => {
  const { id = undefined, examId, onRefresh } = props
  const [open, setOpen] = useState(false)
  const handleClickOpen = () => {
    setOpen(true)
  }
  const handleClose = () => {
    setOpen(false)
  }
  const handleSubmit = async (_data: FormDataSession) => {
    if (id) {
    } else {
      const created = await sectionPartRequest.createSessionPart({
        title: _data.title,
        instructions: _data.instructions,
        content: _data.content,
        orderNum: 0,
        sectionType: _data.sectionType,
        type: SectionPartTypes.Section,
        examId: parseInt(`${examId}`),
      })
      if (created) {
        handleClose()
        toast.success('Tạo session thành công')
        setTimeout(() => {
          onRefresh(created)
        }, 1000)
      } else {
        toast.error('Lỗi, hãy tạo lại')
      }
    }
  }
  let ButtonCRUD = (
    <Button
      variant="outlined"
      color="primary"
      startIcon={<AddCircleIcon />}
      onClick={handleClickOpen}
      sx={{
        border: '2px dashed',
        color: 'text.secondary',
        borderColor: 'text.secondary',
        borderRadius: '4px',
        padding: '8px',
        transition: 'background-color 0.3s',
        width: '100%',
      }}
    >
      Tạo mới kỹ năng
    </Button>
  )
  if (id) {
    ButtonCRUD = (
      <Button
        color={'warning'}
        startIcon={<EditIcon />}
        onClick={handleClickOpen}
        sx={{
          fontSize: '16px',
        }}
      >
        Cập nhật
      </Button>
    )
  }
  return (
    <Box
      sx={{
        width: id ? '' : '100%',
      }}
    >
      {ButtonCRUD}
      <Dialog open={open} fullWidth maxWidth="md">
        <DialogTitle> {!id ? 'Tạo mới' : 'Cập nhật'} Section</DialogTitle>
        <DialogContent>
          <SessionForm onClose={handleClose} onSubmit={handleSubmit} />
        </DialogContent>
      </Dialog>
    </Box>
  )
}

export default CreateOrUpdateSection
