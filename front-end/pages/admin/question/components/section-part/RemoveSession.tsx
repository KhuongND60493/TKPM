import React, { useState } from 'react'
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import { sectionPartRequest } from '@/app/api'
import { toast } from 'react-hot-toast'

interface RemoveSessionProps {
  id: number
  onRefresh: () => void
}

const RemoveSession: React.FC<RemoveSessionProps> = (
  props: RemoveSessionProps,
) => {
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
    <div>
      <Button
        variant="outlined"
        color="error"
        startIcon={<DeleteIcon />}
        onClick={handleClickOpen}
        disabled={loading}
      >
        Xoá
      </Button>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Xoá session</DialogTitle>
        <DialogContent>Bạn có muốn xoá session này?</DialogContent>
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
    </div>
  )
}

export default RemoveSession
