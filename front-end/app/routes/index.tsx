import { Navigate, RouteObject } from 'react-router-dom'
import { MainLayout, AdminLayout, StudentLayout } from '@/pages/layouts'
import {
  LoginPage,
  ExamPage,
  QuestionPage,
  StudentExamAttemptPage,
  StudentStartAttemptPage,
  StudentExamPage,
  NotFoundPage,
} from '@/pages'
import { PrivateRoute } from '@/app/routes/PrivateRoute'
import { Role } from '@/features/auth/configs.ts'

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <Navigate to="/users/login" replace />,
  },
  {
    path: '/admin',
    element: <AdminLayout />,
    children: [
      {
        index: true,
        element: (
          <PrivateRoute>
            <ExamPage />
          </PrivateRoute>
        ),
      },
      {
        path: 'exams',
        element: (
          <PrivateRoute>
            <ExamPage />
          </PrivateRoute>
        ),
      },
      {
        path: 'questions',
        element: (
          <PrivateRoute>
            <QuestionPage />
          </PrivateRoute>
        ),
      },
      {
        path: 'questions/:id',
        element: (
          <PrivateRoute>
            <QuestionPage />
          </PrivateRoute>
        ),
      },
    ],
  },
  {
    path: 'users/login',
    element: <MainLayout />,
    children: [
      {
        index: true,
        element: <LoginPage />,
      },
    ],
  },
  {
    path: 'exam',
    element: <StudentLayout />,
    children: [
      {
        index: true,
        element: (
          <PrivateRoute>
            <StudentExamPage />
          </PrivateRoute>
        ),
      },
      {
        path: ':id/attempts',
        element: (
          <PrivateRoute>
            <StudentExamAttemptPage />
          </PrivateRoute>
        ),
      },
      {
        path: ':id/attempts/start',
        element: (
          <PrivateRoute>
            <StudentStartAttemptPage />
          </PrivateRoute>
        ),
      },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/404" replace />,
  },
  {
    path: '/404',
    element: <NotFoundPage />,
  },
]

export const allowedRoutes = {
  [Role.STUDENT]: [
    '/exam',
    '/exam/:id/attempts',
    '/exam/:id/attempts/start',
    '/exam/:id/submit',
    '/exam/:id/result',
  ],
  [Role.ADMIN]: null,
  [Role.TEACHER]: null,
}
