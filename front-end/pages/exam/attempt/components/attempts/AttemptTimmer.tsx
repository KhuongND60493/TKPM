import { useEffect, useState } from 'react'
import { Box, Typography, LinearProgress, Button } from '@mui/material'
import AccessTimeIcon from '@mui/icons-material/AccessTime'
import withAttemptTimer from '../../hoc/withAttemptTimer'
import { IStartStudentAttempt } from '@/features/exam/type'
import SendIcon from '@mui/icons-material/Send'

export interface AttemptTimerProps {
  minutes?: number
  startTime?: string
  onTimeUp?: () => void
  onBack?: (path?: string) => void
  attempt?: IStartStudentAttempt
  onEnd: () => void
}

const AttemptTimer = (props: AttemptTimerProps) => {
  const { minutes = 1, onTimeUp = () => {}, onEnd, startTime } = props

  const totalTime = minutes * 60
  const [timeLeft, setTimeLeft] = useState(0)
  const calculateTimeLeft = () => {
    const now = new Date()
    const start = startTime ? new Date(startTime) : now
    const elapsed = Math.floor((now.getTime() - start.getTime()) / 1000)
    return Math.max(totalTime - elapsed, 0)
  }

  useEffect(() => {
    const remainingTime = calculateTimeLeft()
    setTimeLeft(remainingTime)

    if (remainingTime <= 0) {
      onTimeUp()
      return
    }

    const timer = setInterval(() => {
      setTimeLeft((prevTime) => {
        const newTimeLeft = prevTime - 1
        if (newTimeLeft <= 0) {
          clearInterval(timer)
          onTimeUp()
        }
        return newTimeLeft
      })
    }, 1000)

    return () => clearInterval(timer)
  }, [startTime, minutes, onTimeUp])

  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`
  }

  const progress = ((totalTime - timeLeft) / totalTime) * 100

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        gap: 2,
        padding: 2,
        borderRadius: 2,
        backgroundColor: '#f0f4f8',
        color: '#333',
      }}
    >
      <Box sx={{ flex: 1 }}>
        <LinearProgress
          variant="determinate"
          value={progress}
          sx={{
            height: 8,
            borderRadius: 4,
            backgroundColor: '#e0e0e0',
          }}
        />
      </Box>

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <AccessTimeIcon color="primary" />
        <Typography
          color={'text.secondary'}
          sx={{ fontSize: '16px', fontWeight: 'bold' }}
        >
          {formatTime(timeLeft)}
        </Typography>
      </Box>
      <Button
        variant="contained"
        color="error"
        onClick={onEnd}
        startIcon={<SendIcon />}
        sx={{
          padding: '10px 20px',
          fontSize: '16px',
          fontWeight: 'bold',
        }}
      >
        Huỷ
      </Button>
    </Box>
  )
}

export default withAttemptTimer(AttemptTimer)
