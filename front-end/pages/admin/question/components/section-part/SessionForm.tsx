import { useForm, Controller } from 'react-hook-form'
import {
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  DialogActions,
  Typography,
  Box,
} from '@mui/material'
import {
  SessionType,
  SectionPartTypes,
  sessionTypeOptions,
} from '@/features/exam/configs'
import { useEffect, forwardRef, useImperativeHandle, useState } from 'react'

export type FormDataSession = {
  title: string
  instructions: string
  content: string
  orderNum: number
  sectionType: SessionType
  type: SectionPartTypes
}
interface SessionFormProps {
  onClose: () => void
  onSubmit: (data: FormDataSession) => void
  data?: FormDataSession
}
const SessionForm = forwardRef((props: SessionFormProps, ref: any) => {
  const { onClose, onSubmit, data } = props
  const {
    setValue,
    register,
    handleSubmit,
    control,
    formState: { errors },
    trigger,
  } = useForm<FormDataSession>()
  const [sectionTypeLabel, setSectionTypeLabel] = useState('')
  useEffect(() => {
    if (data) {
      setValue('title', data.title)
      setValue('instructions', data.instructions)
      setValue('content', data.content)
      setValue('sectionType', data.sectionType as SessionType)
      setValue('orderNum', data.orderNum)
      const indexSectionType = sessionTypeOptions.findIndex(
        (x) => x.value === (data.sectionType as SessionType),
      )
      if (indexSectionType !== -1) {
        setSectionTypeLabel(sessionTypeOptions[indexSectionType].label)
      }
    }
  }, [data, setValue])
  const onSubmitForm = (data: FormDataSession) => {
    onSubmit(data)
  }
  const triggerSubmit = async () => {
    const valid = await trigger()
    if (valid) {
      void handleSubmit(onSubmitForm)()
    }
  }

  useImperativeHandle(ref, () => ({
    triggerSubmit,
  }))

  return (
    <form onSubmit={handleSubmit(onSubmitForm)} noValidate>
      <Grid container spacing={3}>
        <Grid item xs={12} sm={6}>
          <FormControl fullWidth variant="outlined">
            {data ? (
              <Box>
                <Typography fontWeight={'bold'} color={'text.secondary'}>
                  Loại kỹ năng: {sectionTypeLabel}
                </Typography>
              </Box>
            ) : (
              <>
                <InputLabel>Loại kỹ năng</InputLabel>
                <Controller
                  name="sectionType"
                  control={control}
                  rules={{ required: 'Section Type is required' }}
                  render={({ field }) => (
                    <Select
                      {...field}
                      label="Section Type"
                      error={!!errors.sectionType}
                      onChange={(e) => {
                        field.onChange(e)
                        setValue(
                          'sectionType',
                          parseInt(`${e.target.value}`) as SessionType,
                        )
                      }}
                    >
                      {sessionTypeOptions.map((option) => (
                        <MenuItem key={option.value} value={option.value}>
                          {option.label}
                        </MenuItem>
                      ))}
                    </Select>
                  )}
                />
                {errors.type && (
                  <p style={{ color: 'red' }}>{errors.type.message}</p>
                )}
              </>
            )}
          </FormControl>
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Tiêu đề"
            variant="outlined"
            {...register('title', { required: 'Title is required' })}
            error={!!errors.title}
            helperText={errors.title ? errors.title.message : ''}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Hướng dẫn"
            variant="outlined"
            multiline
            rows={4}
            {...register('instructions', {
              required: 'Instructions are required',
            })}
            error={!!errors.instructions}
            helperText={errors.instructions ? errors.instructions.message : ''}
          />
        </Grid>

        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Nội dung"
            variant="outlined"
            multiline
            rows={4}
            error={!!errors.content}
            helperText={errors.content ? errors.content.message : ''}
          />
        </Grid>
      </Grid>
      {!data && (
        <DialogActions>
          <Button onClick={onClose} color="primary">
            Cancel
          </Button>
          <Button type="submit" variant="contained" color="primary">
            Submit
          </Button>
        </DialogActions>
      )}
    </form>
  )
})

export default SessionForm
