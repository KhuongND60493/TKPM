import { Button } from '@mui/material'
import AddCircleIcon from '@mui/icons-material/AddCircle'

interface ButtonCreateQuestionProps {
  onCreate: () => void
}
const ButtonCreateQuestion = (props: ButtonCreateQuestionProps) => {
  return (
    <Button
      variant="outlined"
      color="primary"
      startIcon={<AddCircleIcon />}
      onClick={props.onCreate}
      sx={{
        border: '2px dashed',
        color: 'text.secondary',
        borderColor: 'text.secondary',
        borderRadius: '4px',
        padding: '8px',
        transition: 'background-color 0.3s',
        width: '100%',
        marginBottom: '8px',
      }}
    >
      Tạo mới câu hỏi
    </Button>
  )
}
export default ButtonCreateQuestion
