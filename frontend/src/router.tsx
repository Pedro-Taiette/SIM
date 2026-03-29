import { createBrowserRouter } from 'react-router'
import PrivateRoute from '@/components/layout/PrivateRoute'
import LoginPage from '@/pages/login/LoginPage'
import DashboardPage from '@/pages/dashboard/DashboardPage'

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    element: <PrivateRoute />,
    children: [
      { path: '/', element: <DashboardPage /> },
    ],
  },
])
