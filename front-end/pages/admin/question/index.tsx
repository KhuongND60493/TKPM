import React, { useEffect, useState } from 'react'
import { Box, Tabs, Tab } from '@mui/material'
import { useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { sectionPartRequest } from '@/app/api'
import { ISessionPart } from '@/features/exam/type'
import { manageExam, manageSection } from '@/features/exam/examSlice'

import { useDispatch } from 'react-redux'
import SessionManagement from './components/section-part'
const ExamQuestionManagement: React.FC = () => {
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const { id } = useParams<{ id: string }>()
  const [sectionParts, setSectionParts] = useState<ISessionPart[] | undefined>(
    undefined,
  )
  const [section, setSession] = useState<ISessionPart | undefined>(undefined)
  const [searchParams, setSearchParams] = useSearchParams()
  const sTab = searchParams.get('section')
  const handleTabChange = (_: React.SyntheticEvent, newValue: string) => {
    const currentParams = new URLSearchParams(searchParams)
    currentParams.set('section', newValue)
    currentParams.delete('part')
    setSearchParams(currentParams)
  }
  useEffect(() => {
    if (id) {
      void fetchSectionParts()
    } else {
      navigate('admin/exams')
    }
  }, [id])
  useEffect(() => {
    if (sectionParts && sTab) {
      const findIndex = sTab
        ? sectionParts.findIndex((x) => `${x.id}` === sTab)
        : -1
      if (findIndex === -1) {
        setSearchParams({ section: `${sectionParts[0].id}` })
      } else {
        setSession(sectionParts[findIndex])
      }
    } else {
      if (sectionParts && !sTab) {
        setSearchParams({ section: `${sectionParts[0].id}` })
      }
    }
  }, [sectionParts, sTab])
  useEffect(() => {
    dispatch(manageSection(section))
  }, [section])
  const fetchSectionParts = async () => {
    const response = await sectionPartRequest.sectionPartsById(
      parseInt(`${id}`),
    )
    if (response && response.exam && response.sessions.length > 0) {
      const { exam, sessions } = response
      dispatch(manageExam(exam))
      setSectionParts(sessions)
      if (!sTab) {
        // @ts-ignore
        handleTabChange({}, `${sessions[0].id}`)
      }
    } else {
      navigate('/404')
    }
  }
  if (!id) return <div>Error</div>
  if (!sectionParts || !sTab) return <div>Loading ...</div>
  return (
    <Box
      sx={{
        p: 3,
        display: 'flex',
        flexDirection: 'column',
        height: 'calc(100vh - 170px)',
      }}
    >
      <Box
        display="flex"
        alignItems="center"
        justifyContent="space-between"
        gap={2}
      >
        <Box sx={{ flex: 1, overflowX: 'auto' }}>
          <Tabs
            value={section?.id}
            onChange={handleTabChange}
            aria-label="Exam Sections"
            sx={{
              width: 500,
              flexGrow: 1,
              minWidth: 'fit-content',
              whiteSpace: 'nowrap',
            }}
          >
            {sectionParts.map((tab, index: number) => (
              <Tab
                key={`${tab.id}_${index}`}
                label={`${tab.title}`}
                value={tab.id}
              />
            ))}
          </Tabs>
        </Box>
      </Box>

      <Box sx={{ mt: 2, height: '100%' }}>
        {section && id && (
          <SessionManagement session={section} examId={parseInt(id)} />
        )}
      </Box>
    </Box>
  )
}

export default ExamQuestionManagement
