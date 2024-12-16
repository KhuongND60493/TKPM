import { useNavigate } from 'react-router-dom'
import { List, ListItemButton, ListItemIcon, ListItemText } from '@mui/material'
import AssignmentIcon from '@mui/icons-material/Assignment'

const AdminDefaultMenu = () => {
  const navigate = useNavigate()
  const handleAccessToPage = (path: string) => {
    navigate(path)
  }
  return (
    <List>
      <ListItemButton
        sx={{ bgcolor: 'rgba(216,191,216, 0.5)' }}
        onClick={() => handleAccessToPage('/admin/exams')}
      >
        <ListItemIcon>
          <AssignmentIcon />
        </ListItemIcon>
        <ListItemText primary="Quản lý đề thi" />
      </ListItemButton>
    </List>
  )
}
export default AdminDefaultMenu
