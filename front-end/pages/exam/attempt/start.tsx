import { useEffect } from 'react'
import { Box, Grid } from '@mui/material'
import { useDispatch, useSelector } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { toast } from 'react-hot-toast'
import { AppDispatch, RootState } from '@/app/store'
import AttemptSections from './components/attempts/AttemptSections'
import AttemptQuestionMenu from './components/attempts/AttemptQuestionMenu'
import AttemptContent from './components/attempts/AttemptContent'
import AttemptTimer from './components/attempts/AttemptTimmer'
import {
  finishAttempt,
  IDataCallbackAttemptSlice,
} from '@/features/exam/attemptSlice'
import { handleAttemptError } from '@/features/exam/utils.ts'
import { KEY_SUBMIT_RESPONSE } from '@/features/exam/configs.ts'

const AttemptStart = () => {
  const dispatch: AppDispatch = useDispatch()
  const navigate = useNavigate()
  const { attempt, sections } = useSelector(
    (state: RootState) => state.examStudent,
  )

  useEffect(() => {
    if (!attempt || !sections) {
      toast.error('Dữ liệu không hợp lệ, vui lòng thử lại!')
      setTimeout(() => {
        navigate('/exam')
      }, 200)
    }
  }, [attempt, sections, navigate])

  const handleFinishedError = ({ success, key }: IDataCallbackAttemptSlice) => {
    if (!success) {
      const msg = handleAttemptError(key)
      if (msg) {
        toast.error(msg)
      }
    }
  }

  const handleFinishAttempt = async () => {
    if (attempt && attempt.attempId) {
      const resultAction = await dispatch(
        finishAttempt({ callback: handleFinishedError }),
      )
      if (finishAttempt.fulfilled.match(resultAction)) {
        toast.success('Gửi bài thi thành công!')
        setTimeout(() => {
          navigate(`/exam/${attempt.examId}/attempts`, { replace: true })
        }, 200)
      } else if (resultAction.payload) {
        if (
          resultAction.payload === KEY_SUBMIT_RESPONSE.ATTEMPT_NOT_IN_PROGRESS
        ) {
          setTimeout(() => {
            navigate(`/exam/${attempt.examId}/attempts`, { replace: true })
          }, 200)
        }
      }
    }
  }
  const handleBackStartPage = (path?: string) => {
    setTimeout(() => {
      navigate(path ? path : `/exam`, { replace: true })
    }, 100)
  }

  return (
    <Box
      display={'flex'}
      flexDirection={'column'}
      sx={{ height: 'calc(100vh - 70px)', overflow: 'hidden' }}
    >
      <AttemptTimer
        onEnd={handleFinishAttempt}
        attempt={attempt}
        onBack={handleBackStartPage}
      />
      <Grid spacing={2} container sx={{ flex: 1, padding: 2 }}>
        <Grid item xs={2} sx={{ position: 'relative' }}>
          <AttemptSections />
        </Grid>
        <Grid item xs={8} sx={{ position: 'relative' }}>
          <AttemptContent onEnd={handleFinishAttempt} />
        </Grid>
        <Grid item xs={2} sx={{ position: 'relative' }}>
          <AttemptQuestionMenu />
        </Grid>
      </Grid>
    </Box>
  )
}

export default AttemptStart
