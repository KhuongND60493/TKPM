import { Paper, Typography } from '@mui/material'
import { PropsWithChildren } from 'react'

interface IManagementWithTitleProps extends PropsWithChildren {
  title: string
}
const ManagementWithTitle = (props: IManagementWithTitleProps) => {
  const { title, children } = props
  return (
    <Paper elevation={3} sx={{ padding: 2, position: 'relative' }}>
      <Typography
        variant="h6"
        sx={{
          textTransform: 'capitalize',
          fontWeight: 'bold',
          marginBottom: 2,
          color: '#1976d2',
        }}
      >
        {title}
      </Typography>
      {children}
    </Paper>
  )
}
export default ManagementWithTitle
