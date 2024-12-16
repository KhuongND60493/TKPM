import { Box, Typography } from '@mui/material'

const PartEmpty = () => {
  return (
    <Box p={1} flex={1} height={'100%'}>
      <Box
        sx={{
          textAlign: 'center',
          padding: '30px',
          borderRadius: '8px',
          backgroundColor: 'white',
          boxShadow: 3,
          height: '100%',
        }}
      >
        <Typography
          variant="h6"
          sx={{ color: 'text.primary', marginBottom: '20px' }}
        >
          Hãy chọn 1 part
        </Typography>
        <Typography
          variant="body2"
          sx={{
            color: 'text.secondary',
            marginBottom: '20px',
            lineHeight: 1.5,
          }}
        >
          Bạn cần chọn một phần để tiếp tục. Vui lòng quay lại và chọn một part
          để tiến hành.
        </Typography>
      </Box>
    </Box>
  )
}

export default PartEmpty
