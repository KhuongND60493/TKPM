import { useState, useEffect } from 'react'
import { Box, List, ListItem, ListItemButton, Typography } from '@mui/material'
import { useSelector } from 'react-redux'
import { selectSections } from '@/features/exam/attemptSelector'
import { useSearchParams, useNavigate } from 'react-router-dom'

const AttemptSections = () => {
  const sections = useSelector(selectSections)
  const [activeSkill, setActiveSkill] = useState<number | null>(null)
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()

  useEffect(() => {
    if (!searchParams.has('sectionId') && sections.length > 0) {
      const firstSectionId = sections[0].id
      setSearchParams({ sectionId: String(firstSectionId) })
      setActiveSkill(0)
    } else {
      const currentSectionId = Number(searchParams.get('sectionId'))
      const activeIndex = sections.findIndex(
        (section) => section.id === currentSectionId,
      )
      if (activeIndex !== -1) {
        setActiveSkill(activeIndex)
      } else {
        navigate('/404')
      }
    }
  }, [sections, searchParams, setSearchParams])
  const handleSkillClick = (index: number) => {
    const selectedSectionId = sections[index].id
    setActiveSkill(index)
    setSearchParams({ sectionId: String(selectedSectionId) })
  }

  return (
    <Box display="flex" justifyContent="space-between">
      <Box
        sx={{
          width: '100%',
          border: '1px solid #ccc',
          borderRadius: '8px',
          padding: 2,
          backgroundColor: '#F8F8FF',
          boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.1)',
        }}
      >
        <Typography
          sx={{ marginBottom: 2, fontWeight: 'bold', color: 'text.secondary' }}
        >
          Danh sách kỹ năng
        </Typography>
        <List>
          {sections.map((section, index) => {
            return (
              <ListItem key={index} disablePadding>
                <ListItemButton
                  onClick={() => handleSkillClick(index)}
                  sx={{
                    backgroundColor:
                      activeSkill === index ? 'text.secondary' : 'inherit',
                    borderRadius: '8px',
                    '&:hover': {
                      backgroundColor: '#D8BFD8',
                      color: 'text.secondary',
                    },
                    padding: '10px',
                  }}
                >
                  <Typography
                    sx={{
                      fontSize: 14,
                      color: activeSkill === index ? '#FFFFFF' : 'inherit',
                      fontWeight: activeSkill === index ? 'bold' : 'normal',
                    }}
                  >
                    {section.title}
                  </Typography>
                </ListItemButton>
              </ListItem>
            )
          })}
        </List>
      </Box>
    </Box>
  )
}

export default AttemptSections
