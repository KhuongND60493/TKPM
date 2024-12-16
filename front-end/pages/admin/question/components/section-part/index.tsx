import { Box } from '@mui/material'
import SessionGeneral from './SessionGeneral'
import { ISessionPart } from '@/features/exam/type'
import { useLocation } from 'react-router-dom'
import PartContent from '../part/PartContent'
interface SessionManagementProps {
  session: ISessionPart
  examId: number
}
const SessionManagement = (props: SessionManagementProps) => {
  const { session, examId } = props
  const location = useLocation()

  const queryParams = new URLSearchParams(location.search)
  const value = queryParams.get('tab') || '0'
  return (
    <Box
      sx={{
        display: 'flex',
        width: '100%',
        height: '100%',
      }}
    >
      <Box
        sx={{
          flex: 1,
          overflowY: 'auto',
        }}
      >
        {value === '0' ? (
          <SessionGeneral session={session} examId={examId} />
        ) : (
          <PartContent type={session.sectionType} />
        )}
      </Box>
    </Box>
  )
}

export default SessionManagement
