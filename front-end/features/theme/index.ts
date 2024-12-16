import { createTheme } from '@mui/material/styles'

const theme = createTheme({
  palette: {
    darkColor: {
      main: '#6C3483',
      contrastText: '#FFFFFF',
    },
    primary: {
      light: '#EED2EE',
      main: '#D8BFD8',
      dark: '#CDA0C5',
    },
    secondary: {
      light: '#F0E6FA',
      main: '#E6E6FA',
    },
    background: {
      default: '#F8F8FF',
    },
    text: {
      primary: '#2C3E50',
      secondary: '#6C3483',
    },
    success: {
      main: '#4CAF50', // Màu xanh lá cây cho success
      contrastText: '#FFFFFF',
    },
    warning: {
      main: '#FFC107', // Màu vàng cho warning
      contrastText: '#FFFFFF',
    },
    error: {
      main: '#F44336', // Màu đỏ cho error
      contrastText: '#FFFFFF',
    },
  },
  typography: {
    fontFamily: 'Roboto, sans-serif',
    h6: {
      color: '#2C3E50',
    },
    body1: {
      color: '#2C3E50',
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          borderRadius: '8px',
        },
        outlined: {
          backgroundColor: '#fff', // Màu nền trắng
          border: '1px solid #ccc',
          '&:hover': {
            backgroundColor: '#f5f5f5', // Màu nền khi hover
            borderColor: '#aaa', // Viền khi hover
          },
        },
        containedPrimary: {
          backgroundColor: '#D8BFD8',
          color: '#FFFFFF',
          '&:hover': {
            backgroundColor: '#CDA0C5',
          },
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            borderRadius: '8px',
            backgroundColor: '#FFFFFF',
            '& fieldset': {
              borderColor: '#D8BFD8',
            },
            '&:hover fieldset': {
              borderColor: '#CDA0C5',
            },
            '&.Mui-focused fieldset': {
              borderColor: '#CDA0C5',
            },
          },
          '& .MuiInputLabel-root': {
            color: '#D8BFD8',
            '&.Mui-focused': {
              color: '#CDA0C5',
            },
          },
        },
      },
    },
  },
})

export default theme
