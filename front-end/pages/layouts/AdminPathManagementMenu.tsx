import React, { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { RootState } from '@/app/store'
import { Box, Divider, Tab, Tabs } from '@mui/material'
import PartsList from '@/pages/admin/question/components/part'
import AdminItemActive from './AdminItemActive'

interface PathManagementMenuProps {
  examId: number
}
const AdminPathManagementMenu = (props: PathManagementMenuProps) => {
  const { examId } = props
  const [value, setValue] = useState<number>(0)
  const [searchParams, setSearchParams] = useSearchParams()
  const tabParam = searchParams.get('tab')
  const section = useSelector((state: RootState) => state.examAdmin?.section)
  const firstLetter = useMemo(() => {
    if (section?.title) return section.title.charAt(0).toUpperCase()
    return 'Part'
  }, [section?.title])
  useEffect(() => {
    if (tabParam) {
      setValue(Number(tabParam))
    } else {
      const currentParams = new URLSearchParams(searchParams)
      currentParams.set('tab', '0')
      setSearchParams(currentParams)
    }
  }, [tabParam])

  const handleChange = (_: React.SyntheticEvent, newValue: number) => {
    const newSearchParams = new URLSearchParams(searchParams)
    newSearchParams.set('tab', String(newValue))
    newSearchParams.delete('part')
    setSearchParams(newSearchParams)
    setValue(newValue)
  }
  const handleChoosePart = (idPart: number) => {
    const newSearchParams = new URLSearchParams(searchParams)
    newSearchParams.set('part', String(idPart))
    setSearchParams(newSearchParams)
  }
  return (
    <Box pl={4} pt={1}>
      <AdminItemActive
        level={2}
        firstLetter={firstLetter}
        title={section?.title || 'Part'}
      />
      <Divider />
      <Tabs
        orientation="vertical"
        value={value}
        onChange={handleChange}
        aria-label="session management tabs"
        sx={{
          display: 'flex',
          flexDirection: 'column',
          '& .MuiTab-root': {
            padding: '6px 16px',
            fontSize: '14px',
            color: 'text.primary',
            transition: 'background-color 0.3s ease, color 0.3s ease',
            display: 'flex',
            alignItems: 'flex-start',
            textAlign: 'left',
            width: '100%',
          },
          '& .Mui-selected': {
            color: 'white',
            backgroundColor: 'text.secondary',
          },
          '& .MuiTabs-flexContainer': {
            alignItems: 'flex-start',
          },
        }}
      >
        <Tab
          sx={{
            width: '100%',
            borderBottom: '1px solid rgba(0,0,0,0.12)',
          }}
          label="Quản lý kỹ năng"
          id="vertical-tab-0"
          aria-controls="vertical-tabpanel-0"
        />
        <Tab
          sx={{ width: '100%' }}
          label="Quản lý phần kỹ năng"
          id="vertical-tab-1"
          aria-controls="vertical-tabpanel-1"
        />
        {value === 1 && section && (
          <PartsList
            section={section}
            examId={examId}
            onChoose={handleChoosePart}
          />
        )}
      </Tabs>
    </Box>
  )
}
export default AdminPathManagementMenu
