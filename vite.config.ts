import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path';
// https://vitejs.dev/config/

const port = process.env.WEB_PORT || 4002;
export default defineConfig({
  server: {
    port:parseInt(`${port}`),
  },
  resolve: {
    alias: { '@': path.resolve(__dirname, 'front-end/') },
  },

  plugins: [react()],
})
