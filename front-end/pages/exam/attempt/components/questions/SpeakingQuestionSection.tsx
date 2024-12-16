import { Box, Button, Typography } from '@mui/material'
import MicIcon from '@mui/icons-material/Mic'
import StopIcon from '@mui/icons-material/Stop'
import PlayArrowIcon from '@mui/icons-material/PlayArrow'
import PauseIcon from '@mui/icons-material/Pause'
import { IAttemptQuestion } from '@/features/exam/type'
import { useDispatch, useSelector } from 'react-redux'
import { saveAnswer } from '@/features/exam/attemptSlice'
import { selectAnswerWritingSpeaking } from '@/features/exam/attemptSelector'
import { useReactMediaRecorder } from 'react-media-recorder'
import { useState } from 'react'

interface SingleQuestionSectionProps {
  questions: IAttemptQuestion[]
}

const SpeakingQuestionSection = ({ questions }: SingleQuestionSectionProps) => {
  const dispatch = useDispatch()
  const answerDictionary = useSelector(selectAnswerWritingSpeaking)
  const [playingQuestionId, setPlayingQuestionId] = useState<number | null>(
    null,
  )
  const [currentTime, setCurrentTime] = useState<number>(0)

  const handleSaveAudio = (questionId: number, blobUrl: string) => {
    dispatch(saveAnswer({ id: questionId, answer: blobUrl }))
  }

  const handlePlayAudio = (questionId: number, audioUrl: string) => {
    const audio = new Audio(audioUrl)
    setPlayingQuestionId(questionId)
    audio.play()

    audio.ontimeupdate = () => {
      setCurrentTime(audio.currentTime)
    }

    audio.onended = () => {
      setPlayingQuestionId(null)
      setCurrentTime(0)
    }
  }

  return (
    <Box>
      {questions.map((question, index) => (
        <Box
          key={question.id}
          sx={{
            marginBottom: 3,
            padding: 2,
            borderRadius: '8px',
            backgroundColor: '#E3F2FD',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              marginBottom: 1,
            }}
          >
            <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
              Câu hỏi {index + 1}
            </Typography>
          </Box>
          <Typography
            sx={{
              whiteSpace: 'pre-wrap',
              marginBottom: 1,
              color: '#37474F',
            }}
          >
            {question.questionText}
          </Typography>

          <AudioRecorder questionId={question.id} onSave={handleSaveAudio} />

          {answerDictionary[question.id] && (
            <Box
              sx={{
                display: 'flex',
                justifyContent: 'center',
                gap: 2,
                marginTop: 2,
                alignItems: 'center',
              }}
            >
              <Button
                variant="contained"
                startIcon={
                  playingQuestionId === question.id ? (
                    <PauseIcon />
                  ) : (
                    <PlayArrowIcon />
                  )
                }
                onClick={() => {
                  if (playingQuestionId === question.id) {
                    setPlayingQuestionId(null)
                  } else {
                    handlePlayAudio(question.id, answerDictionary[question.id])
                  }
                }}
                sx={{ fontSize: '16px', padding: '10px 20px' }}
              >
                {playingQuestionId === question.id
                  ? 'Đang phát...'
                  : 'Nghe lại'}
              </Button>
              {playingQuestionId === question.id && (
                <Typography variant="body2" sx={{ color: '#37474F' }}>
                  {`Thời gian: ${currentTime.toFixed(1)} giây`}
                </Typography>
              )}
            </Box>
          )}
        </Box>
      ))}
    </Box>
  )
}

interface AudioRecorderProps {
  questionId: number
  onSave: (questionId: number, blobUrl: string) => void
}

const AudioRecorder = ({ questionId, onSave }: AudioRecorderProps) => {
  const { startRecording, stopRecording, mediaBlobUrl, status } =
    useReactMediaRecorder({ audio: true })

  if (mediaBlobUrl && status === 'stopped') {
    onSave(questionId, mediaBlobUrl)
  }

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        marginTop: 2,
      }}
    >
      {status !== 'recording' ? (
        <Button
          variant="contained"
          startIcon={<MicIcon />}
          onClick={startRecording}
          sx={{ fontSize: '16px', padding: '10px 20px' }}
        >
          Ghi âm
        </Button>
      ) : (
        <Button
          variant="contained"
          color="error"
          startIcon={<StopIcon />}
          onClick={stopRecording}
          sx={{ fontSize: '16px', padding: '10px 20px' }}
        >
          Dừng
        </Button>
      )}
    </Box>
  )
}

export default SpeakingQuestionSection
