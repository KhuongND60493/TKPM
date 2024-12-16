import { useForm } from 'react-hook-form'
import { Button, DialogActions, Grid, TextField } from '@mui/material'
import { forwardRef, useEffect, useImperativeHandle } from 'react'

export type FormDataPart = {
  title: string
  instructions: string
}
interface PartFormProps {
  onClose: () => void
  onSubmit: (data: FormDataPart) => void
  data?: FormDataPart
}
const PartForm = forwardRef((props: PartFormProps, ref: any) => {
  const { onClose, onSubmit, data } = props
  const {
    setValue,
    register,
    handleSubmit,
    formState: { errors },
    trigger,
  } = useForm<FormDataPart>()
  useEffect(() => {
    if (data) {
      setValue('title', data.title)
      setValue('instructions', data.instructions)
    }
  }, [data, setValue])
  const onSubmitForm = (data: FormDataPart) => {
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
            {...register('instructions')}
            error={!!errors.instructions}
            helperText={errors.instructions ? errors.instructions.message : ''}
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

export default PartForm
