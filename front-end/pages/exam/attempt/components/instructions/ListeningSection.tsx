import { IAttemptPart } from '@/features/exam/type'

const baseApiUrl = `${import.meta.env.VITE_BASE_URL || ''}/api`
interface ListeningSectionProps {
  part: IAttemptPart
}

const ListeningSection = (props: ListeningSectionProps) => {
  const { part } = props
  return (
    <>
      <audio
        controls
        src={`${baseApiUrl}/${part.content}`}
        style={{ width: '100%', marginBottom: 16 }}
      />
    </>
  )
}

export default ListeningSection
