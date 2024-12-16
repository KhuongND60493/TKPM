import withBasePartContent from '@/pages/admin/question/hoc/withBasePartContent'
import { Box } from '@mui/material'
import ManagementWithTitle from '@/pages/admin/question/components/ManagementWithTitle.tsx'
import ButtonCreateQuestion from '@/pages/admin/question/components/question/ButtonCreateQuestion.tsx'
import QuestionManager, {
  QuestionManagerHandle,
} from '@/pages/admin/question/components/question'
import { useRef } from 'react'
import { ISessionPart } from '@/features/exam/type.ts'

interface IWritingSectionProps {
  part: ISessionPart | null
}
const WritingSection = (props: IWritingSectionProps) => {
  const { part } = props
  const questionManagerRef = useRef<QuestionManagerHandle>(null)
  const handleCreateEmptyQuestion = () => {
    if (questionManagerRef.current) {
      questionManagerRef.current.addQuestion()
    }
  }
  if (!part?.id) return <Box>Loading ...</Box>
  return (
    <Box mt={2}>
      <ManagementWithTitle title={'Câu hỏi'}>
        <Box position={'absolute'} right={0} top={0} p={2}>
          <ButtonCreateQuestion onCreate={handleCreateEmptyQuestion} />
        </Box>
        <QuestionManager ref={questionManagerRef} part={part} />
      </ManagementWithTitle>
    </Box>
  )
}
export default withBasePartContent(WritingSection, 'Yêu cầu', 20)
