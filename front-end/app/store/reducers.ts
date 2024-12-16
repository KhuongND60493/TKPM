import { combineReducers } from 'redux'
import { authReducer } from '@/features/auth/authSlice'
import { persistReducer } from 'redux-persist'
import storage from 'redux-persist/lib/storage'
import { examAdminReducer } from '@/features/exam/examSlice'
import { examStudentReducer } from '@/features/exam/attemptSlice'

const persistConfig = {
  key: 'root',
  storage,
  whitelist: ['examStudent', 'auth']
}
export const rootReducer = combineReducers({
  auth: authReducer,
  examAdmin: examAdminReducer,
  examStudent: examStudentReducer
})

export const persistedReducer = persistReducer(persistConfig, rootReducer);