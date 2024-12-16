import { useEffect, useState } from 'react'
import {
  Button,
  Box,
  List,
  ListItemText,
  ListItemButton,
  Dialog,
  DialogTitle,
  DialogContent,
} from '@mui/material'
import { sectionPartRequest } from '@/app/api'
import { ISessionPart } from '@/features/exam/type'
import AddCircleIcon from '@mui/icons-material/AddCircle'
import PartForm, {
  FormDataPart,
} from '@/pages/admin/question/components/part/PartForm.tsx'
import { SectionPartTypes } from '@/features/exam/configs.ts'
import { toast } from 'react-hot-toast'
import { useNavigate } from 'react-router-dom'
interface PartManagementProps {
  examId: number
  onChoose: (id: number) => void
  section: ISessionPart
}

const PartList = (props: PartManagementProps) => {
  const { section, examId, onChoose } = props
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)
  const [selectedPart, setSelectedPart] = useState<number | null>(null) // Store which part is selected
  const [parts, setParts] = useState<ISessionPart[]>([])
  useEffect(() => {
    setSelectedPart(null)
    void fetchParts()
  }, [section.id])

  const fetchParts = async () => {
    const response = await sectionPartRequest.partsBySectionId(section.id)
    if (response && response.length > 0) {
      void setParts(response)

      checkAndSelectPart(response)
    } else {
      setParts([])
    }
  }
  const checkAndSelectPart = (response: ISessionPart[]) => {
    const searchParams = new URLSearchParams(location.search)
    const partId = searchParams.get('part')
    if (partId) {
      const partExists = response.some((part) => part.id === parseInt(partId))
      if (partExists) {
        const selectedPart = response.find(
          (part) => part.id === parseInt(partId),
        )
        if (selectedPart) {
          setSelectedPart(selectedPart.id)
          onChoose(selectedPart.id)
        }
      } else {
        navigate('/404')
      }
    }
  }

  const handlePartSelect = (id: number) => {
    setSelectedPart(id)
    onChoose(id)
  }

  const handleClose = () => {
    setOpen(false)
  }
  const handleCreatePart = async (data: FormDataPart) => {
    const created = await sectionPartRequest.createSessionPart({
      ...data,
      type: SectionPartTypes.Part,
      sectionType: section.sectionType,
      examId: examId,
      content: '',
      orderNum: 0,
      parentId: section.id,
    })
    if (created) {
      handleClose()
      toast.success('Tạo part thành công')
      setTimeout(() => {
        navigate(0)
      }, 500)
    } else {
      toast.error('Lỗi, hãy tạo lại')
    }
  }

  const handleOpenCreatePart = () => {
    setOpen(true)
  }
  return (
    <Box width={'100%'}>
      <List>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<AddCircleIcon />}
          onClick={handleOpenCreatePart}
          sx={{
            border: '2px dashed',
            color: 'text.secondary',
            borderColor: 'text.secondary',
            borderRadius: '4px',
            padding: '8px',
            transition: 'background-color 0.3s',
            width: '100%',
            marginBottom: '8px',
          }}
        >
          Tạo mới part
        </Button>
        <Dialog open={open} fullWidth maxWidth="md">
          <DialogTitle> Tạo mới Part</DialogTitle>
          <DialogContent>
            <PartForm onClose={handleClose} onSubmit={handleCreatePart} />
          </DialogContent>
        </Dialog>
        {parts.map((part) => (
          <ListItemButton
            key={part.id}
            onClick={() => handlePartSelect(part.id)}
            selected={selectedPart === part.id}
            sx={{
              '&.Mui-selected': {
                backgroundColor: 'primary.main',
                color: 'white',
                '&:hover': {
                  backgroundColor: 'primary.dark',
                },
              },
              '&:hover': {
                backgroundColor: 'primary.light',
              },
              transition: 'background-color 0.3s',
            }}
          >
            <ListItemText
              primary={part.title}
              sx={{
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
            />
          </ListItemButton>
        ))}
      </List>
    </Box>
  )
}

export default PartList
