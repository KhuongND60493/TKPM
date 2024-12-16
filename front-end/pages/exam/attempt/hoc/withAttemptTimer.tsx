import React, { useEffect } from 'react'
import { AttemptTimerProps } from '../components/attempts/AttemptTimmer'
import { checkAndFinishAttempt } from '@/features/exam/utils'

const withAttemptTimer = <P extends object>(
  WrappedComponent: React.ComponentType<P>,
) => {
  const TimerHOC: React.FC<AttemptTimerProps & Omit<P, keyof P>> = ({
    minutes,
    onTimeUp,
    onBack,
    attempt,
    onEnd,
    ...props
  }) => {
    useEffect(() => {
      if (attempt) {
        const needFinished = checkAndFinishAttempt(
          attempt.startTime,
          attempt.duration,
        )
        if (needFinished && onEnd) {
          onEnd()
        }
      }
    }, [attempt])

    const handleTimeUp = () => {
      if (onEnd) {
        onEnd()
      }
    }
    if (!attempt) return <></>
    return (
      <WrappedComponent
        {...(props as P)}
        onEnd={onEnd}
        startTime={attempt.startTime}
        minutes={attempt.duration}
        onTimeUp={handleTimeUp}
      />
    )
  }

  return TimerHOC
}

export default withAttemptTimer
