import { useMemo, useState } from 'react'
import {
  Button,
  Checkbox,
  FormControlLabel,
  List,
  ListItem,
  TextField,
  Box,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
} from '@mui/material'
import { IOption } from '@/features/exam/type'
import RestoreIcon from '@mui/icons-material/Restore'
import SaveIcon from '@mui/icons-material/Save'
import {
  areOptionsDifferent,
  optionsArrayToObject,
} from '@/features/exam/utils'
import DeleteIcon from '@mui/icons-material/Delete'

interface OptionListProps {
  questionId: number
  options: IOption[]
  onUpdateOptions: (options: IOption[]) => void
  onRemoveOption: (questionId: number, id: number) => void
}
const OptionList = (props: OptionListProps) => {
  const { options, onUpdateOptions, questionId, onRemoveOption } = props
  const [openDialog, setOpenDialog] = useState(false)
  const [editedOptions, setEditedOptions] = useState<{
    [key: number]: IOption
  }>(optionsArrayToObject(options))
  const [selectedOptionId, setSelectedOptionId] = useState<number | null>(null)

  const isEdit = useMemo(() => {
    return areOptionsDifferent(options, editedOptions)
  }, [options, editedOptions])

  const handleOptionChange = (
    id: number,
    field: 'content' | 'isCorrect',
    value: string | boolean,
  ) => {
    setEditedOptions((prev) => ({
      ...prev,
      [id]: {
        ...options.find((option) => option.id === id)!,
        [field]: value,
      },
    }))
  }

  const handleUpdateOption = () => {
    onUpdateOptions(Object.values(editedOptions))
  }
  const handleResetOption = () => {
    setEditedOptions(optionsArrayToObject(options))
  }
  const handleOpenDialog = (id: number) => {
    setSelectedOptionId(id)
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setSelectedOptionId(null)
    setOpenDialog(false)
  }

  const handleConfirmRemove = () => {
    if (selectedOptionId !== null) {
      onRemoveOption(questionId, selectedOptionId)
    }
    handleCloseDialog()
  }

  return (
    <Box position={'relative'} pt={5} borderTop={'2px dashed #3f51b5'}>
      {isEdit && (
        <Box
          position={'absolute'}
          display={'flex'}
          top={4}
          gap={2}
          right={0}
          zIndex={2}
        >
          <Button
            variant="outlined"
            color="secondary"
            startIcon={<RestoreIcon />}
            onClick={handleResetOption}
          >
            Reset
          </Button>
          <Button
            variant="outlined"
            color="warning"
            startIcon={<SaveIcon />}
            onClick={handleUpdateOption}
          >
            Cập nhật trả lời
          </Button>
        </Box>
      )}
      <List>
        {options.map((option) => {
          const editedOption = editedOptions[option.id]

          return (
            <ListItem
              key={option.id}
              sx={{
                alignItems: 'center',
                border: '1px dashed #ccc',
                borderRadius: '4px',
                mb: 2,
                p: 2,
                gap: 2,
              }}
            >
              <FormControlLabel
                control={
                  <Checkbox
                    checked={editedOption?.isCorrect}
                    onChange={(e) =>
                      handleOptionChange(
                        option.id,
                        'isCorrect',
                        e.target.checked,
                      )
                    }
                  />
                }
                label=""
              />

              <TextField
                label="Trả lời"
                value={editedOption?.content ?? option.content}
                onChange={(e) =>
                  handleOptionChange(option.id, 'content', e.target.value)
                }
                fullWidth
              />
              <Button
                variant="outlined"
                color="error"
                startIcon={<DeleteIcon />}
                onClick={() => handleOpenDialog(option.id)}
              >
                Xóa
              </Button>
            </ListItem>
          )
        })}
      </List>
      <Dialog
        open={openDialog}
        onClose={handleCloseDialog}
        aria-labelledby="confirm-dialog-title"
        aria-describedby="confirm-dialog-description"
      >
        <DialogTitle id="confirm-dialog-title">Xác nhận</DialogTitle>
        <DialogContent>
          <DialogContentText id="confirm-dialog-description">
            Bạn có chắc chắn muốn xóa option này?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="secondary">
            Hủy
          </Button>
          <Button
            onClick={handleConfirmRemove}
            color="error"
            variant="contained"
          >
            Xóa
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default OptionList
