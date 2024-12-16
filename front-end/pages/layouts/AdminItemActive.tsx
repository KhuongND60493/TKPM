import {
  Avatar,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
} from '@mui/material'

const AdminItemActive = ({
  firstLetter,
  title,
  level = 1,
}: {
  firstLetter: string
  title: string
  level?: number
}) => {
  return (
    <ListItemButton
      sx={{ bgcolor: level === 1 ? 'rgba(216,191,216, 0.5)' : 'lightblue' }}
    >
      <ListItemIcon>
        <Avatar
          sx={{
            bgcolor: 'primary.main',
            width: 24,
            height: 24,
            display: 'flex',
            justifyContent: 'center',
            borderRadius: 0,
            alignItems: 'center',
          }}
        >
          <Typography variant="h6" sx={{ color: 'white' }}>
            {firstLetter}
          </Typography>
        </Avatar>
      </ListItemIcon>
      <ListItemText>
        <Typography
          sx={{
            whiteSpace: 'nowrap',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
          }}
        >
          {title}
        </Typography>
      </ListItemText>
    </ListItemButton>
  )
}
export default AdminItemActive
