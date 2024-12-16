import { Box } from '@mui/material'
import { ISessionPart } from '@/features/exam/type'
import withBasePartContent from '../../hoc/withBasePartContent'
import QuestionManager, { QuestionManagerHandle } from '../question'
import ManagementWithTitle from '../ManagementWithTitle'
import ButtonCreateQuestion from '../question/ButtonCreateQuestion'
import { useRef } from 'react'

interface IReadingSectionProps {
  part: ISessionPart | null
}
const ReadingSection = (props: IReadingSectionProps) => {
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

export default withBasePartContent(ReadingSection, 'đoạn văn', 15)
