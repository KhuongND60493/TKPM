import { Box, Button } from '@mui/material'
import RemoveSession from './RemoveSession'
import { useNavigate } from 'react-router-dom'
import SessionForm, {
  FormDataSession,
} from '@/pages/admin/question/components/section-part/SessionForm'
import { ISessionPart } from '@/features/exam/type'
import EditIcon from '@mui/icons-material/Edit'
import { useRef } from 'react'
import { sectionPartRequest } from '@/app/api'
import { SectionPartTypes } from '@/features/exam/configs.ts'
import { toast } from 'react-hot-toast'

interface SessionGeneralProps {
  session: ISessionPart
  examId: number
}
const SessionGeneral = (props: SessionGeneralProps) => {
  const { session, examId } = props
  const formRef = useRef<any>(null)
  const navigate = useNavigate()
  const handleRefresh = () => {
    navigate(`/admin/questions/${examId}`)
    window.location.reload()
  }
  const handleClickUpdate = () => {
    if (formRef.current) {
      formRef.current.triggerSubmit()
    }
  }
  const handleSubmitUpdateSection = async (_data: FormDataSession) => {
    const updated = await sectionPartRequest.updateSessionPart(session.id, {
      title: _data.title,
      instructions: _data.instructions,
      content: _data.content,
      orderNum: 0,
      sectionType: _data.sectionType,
      type: SectionPartTypes.Section,
      examId: parseInt(`${examId}`),
    })
    if (updated) {
      toast.success('Cập nhật section thành công')
      setTimeout(() => {
        navigate(0)
      }, 500)
    } else {
      toast.error('Lỗi, hãy cập nhật lại')
    }
  }
  return (
    <Box
      position={'relative'}
      flex={1}
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
      }}
      pt={4}
    >
      <Box
        flex={1}
        px={2}
        sx={{
          overflowX: 'auto',
          maxHeight: 'calc(100% - 50px)',
        }}
      >
        <SessionForm
          ref={formRef}
          data={{
            title: session.title,
            sectionType: session.sectionType,
            instructions: session.instructions,
            content: session.content,
            type: session.type,
            orderNum: 0,
          }}
          onClose={() => {}}
          onSubmit={handleSubmitUpdateSection}
        />
      </Box>
      <Box
        sx={{ display: 'flex', justifyContent: 'flex-end' }}
        position={'absolute'}
        right={0}
        top={0}
        gap={2}
        mt={2}
      >
        <Button
          variant="outlined"
          color="warning"
          startIcon={<EditIcon />}
          onClick={handleClickUpdate}
        >
          Cập nhật
        </Button>
        <RemoveSession id={session.id} onRefresh={handleRefresh} />
      </Box>
    </Box>
  )
}
export default SessionGeneral
