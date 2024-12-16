import { useSearchParams } from 'react-router-dom'
import PartEmpty from './PartEmpty'
import { SessionType } from '@/features/exam/configs.ts'
import ListeningSection from '../listening/ListeningSection'
import ReadingSection from '../reading/ReadingSection'
import SpeakingSection from '../speaking/SpeakingSection'
import WritingSection from '../writing/WritingSection'

interface PartContentProps {
  type: SessionType
}
const PartContent = (props: PartContentProps) => {
  const { type } = props
  const [searchParams] = useSearchParams()
  const partId = searchParams.get('part')
  if (!partId) {
    return <PartEmpty />
  }
  if (type === SessionType.Listening) return <ListeningSection />
  if (type === SessionType.Reading) return <ReadingSection />
  if (type === SessionType.Speaking) return <SpeakingSection />
  if (type === SessionType.Writing) return <WritingSection />
  return <div>Lỗi không xác định được loại Session</div>
}
export default PartContent
