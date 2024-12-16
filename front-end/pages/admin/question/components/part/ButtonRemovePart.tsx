import { useState } from 'react'
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import { sectionPartRequest } from '@/app/api'
import { toast } from 'react-hot-toast'

interface RemovePartProps {
  id: number
  onRefresh: () => void
}

const ButtonRemovePart = (props: RemovePartProps) => {
  const { id, onRefresh } = props
  const [open, setOpen] = useState<boolean>(false)
  const [loading, setLoading] = useState<boolean>(false)
  const handleClickOpen = () => {
    setOpen(true)
  }

  const handleClose = () => {
    setOpen(false)
  }

  const handleRemoveConfirm = async () => {
    setLoading(true)
    try {
      const response = await sectionPartRequest.deleteSessionPart(id)
      if (response) {
        setOpen(false)
        toast.success('Xoá session thành công')
        setTimeout(() => {
          onRefresh()
        }, 1000)
      } else {
        toast.error('Lỗi, không xoá được session')
      }
    } catch (error) {
      console.error('Error removing session', error)
    } finally {
      setLoading(false)
      setOpen(false)
    }
  }

  return (
    <Box px={2} py={1}>
      <Button
        variant="outlined"
        color="error"
        startIcon={<DeleteIcon />}
        onClick={handleClickOpen}
        disabled={loading}
        fullWidth={true}
      >
        Xoá phần kỹ năng
      </Button>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Xoá phần kỹ năng</DialogTitle>
        <DialogContent>Bạn có muốn xoá phần kỹ năng này?</DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary" disabled={loading}>
            Cancel
          </Button>
          <Button
            onClick={handleRemoveConfirm}
            color="error"
            disabled={loading}
          >
            {loading ? 'Removing...' : 'Đồng ý'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default ButtonRemovePart
