import { routes } from './routes'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'

const App = () => {
  const rootRoutes = createBrowserRouter(routes)
  return <RouterProvider router={rootRoutes} />
}

export default App
