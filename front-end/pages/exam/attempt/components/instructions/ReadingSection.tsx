import { Typography } from '@mui/material'
import { IAttemptPart } from '@/features/exam/type'

interface ReadingSectionProps {
  part: IAttemptPart
}

const ReadingSection = (props: ReadingSectionProps) => {
  const { part } = props
  return (
    <Typography variant="body1" gutterBottom>
      {part?.content || ''}
    </Typography>
  )
}

export default ReadingSection
