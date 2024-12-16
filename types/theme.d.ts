import '@mui/material/styles'

declare module '@mui/material/styles' {
  interface Palette {
    darkColor: Palette['darkColor']
  }
  interface PaletteOptions {
    darkColor?: PaletteOptions['darkColor']
  }
}

declare module '@mui/material/Button' {
  interface ButtonPropsColorOverrides {
    darkColor: true
  }
}
