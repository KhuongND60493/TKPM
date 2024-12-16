import React from 'react'
import { Dialog, DialogTitle, DialogActions, Button } from '@mui/material'

interface ConfirmDialogProps {
  open: boolean
  title: string
  onConfirm: () => void
  onClose: () => void
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  open,
  title,
  onConfirm,
  onClose,
}) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{title}</DialogTitle>
      <DialogActions>
        <Button onClick={onClose} color="secondary">
          Hủy
        </Button>
        <Button onClick={onConfirm} color="error">
          Xóa
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default ConfirmDialog
