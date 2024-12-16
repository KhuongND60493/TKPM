import { Box, Typography } from '@mui/material'

const Footer = () => {
  return (
    <Box
      component="footer"
      sx={{
        py: 2,
        px: 3,
        mt: 'auto',
        backgroundColor: (theme) =>
          theme.palette.mode === 'light'
            ? theme.palette.grey[200]
            : theme.palette.grey[800],
        textAlign: 'center',
        width: '100%',
      }}
    >
      <Typography variant="body2" color="textSecondary">
        © 2024 TKPM - KHTN. All rights reserved.
      </Typography>
    </Box>
  )
}

export default Footer
