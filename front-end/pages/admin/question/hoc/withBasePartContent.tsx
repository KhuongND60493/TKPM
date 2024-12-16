import React, { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { ISessionPart } from '@/features/exam/type.ts'
import { sectionPartRequest } from '@/app/api'
import { toast } from 'react-hot-toast'
import { Box, Button, TextField, Typography, Fab, Menu } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import ButtonUpdatePart from '../components/part/ButtonUpdatePart'
import ButtonRemovePart from '../components/part/ButtonRemovePart'
import ManagementWithTitle from '../components/ManagementWithTitle'

interface WithPartProps {
  part: ISessionPart | null
}

function withBasePartContent<P extends object>(
  WrappedComponent: React.ComponentType<P & WithPartProps>,
  title: string | undefined = 'đoạn văn',
  countRow: number | undefined = 4,
  showContent: boolean | undefined = true,
) {
  return (props: P) => {
    const navigate = useNavigate()
    const [part, setPart] = useState<ISessionPart | null>(null)
    const [newContent, setNewContent] = useState<string>('')
    const [isEditing, setIsEditing] = useState<boolean>(false)
    const [loading, setLoading] = useState(false)
    const [searchParams, setSearchParams] = useSearchParams()
    const partId = searchParams.get('part')
    const [menuAnchor, setMenuAnchor] = useState<null | HTMLElement>(null) // Menu anchor state

    useEffect(() => {
      if (!partId) {
        navigate('/404', { replace: true })
      } else {
        void fetchPart()
      }
    }, [partId, navigate])

    const fetchPart = async () => {
      setLoading(true)
      try {
        const res = await sectionPartRequest.getPartById(parseInt(partId!))
        if (res) {
          setPart(res)
          setNewContent(res.content || '')
        } else {
          navigate('/404', { replace: true })
        }
      } catch (error) {
        console.error(error)
        toast.error('Không thể tải thông tin part')
        navigate('/404', { replace: true })
      } finally {
        setLoading(false)
      }
    }

    const handleContentChange = (
      event: React.ChangeEvent<HTMLInputElement>,
    ) => {
      setNewContent(event.target.value)
    }

    const handleUpdateContent = async () => {
      if (!newContent.trim()) {
        toast.error('Nội dung không được để trống!')
        return
      }
      setLoading(true)
      try {
        const res = await sectionPartRequest.updatePartContent(
          partId!,
          newContent,
        )
        if (res) {
          toast.success('Cập nhật đoạn văn thành công!')
          setIsEditing(false)
          setTimeout(() => {
            window.location.reload()
          }, 500)
        } else {
          toast.error('Cập nhật thất bại, vui lòng thử lại!')
        }
      } catch (error) {
        console.error(error)
        toast.error('Có lỗi xảy ra khi cập nhật nội dung')
      } finally {
        setLoading(false)
      }
    }

    const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
      setMenuAnchor(event.currentTarget)
    }

    const handleMenuClose = () => {
      setMenuAnchor(null)
    }

    const handleRemovePart = () => {
      const currentParams = new URLSearchParams(searchParams)
      currentParams.delete('part')
      setSearchParams(currentParams)
      setTimeout(() => {
        window.location.reload()
      }, 200)
      toast.success('Xóa part thành công!')
      setMenuAnchor(null)
    }

    if (loading || !part) {
      return <div>Loading...</div>
    }

    return (
      <Box p={2} position={'relative'}>
        {showContent && (
          <ManagementWithTitle title={title}>
            {!isEditing ? (
              <Typography
                variant="body1"
                onClick={() => setIsEditing(true)}
                sx={{
                  cursor: 'pointer',
                  padding: 1,
                  backgroundColor: '#f9f9f9',
                  borderRadius: '4px',
                  border: '2px dashed #3f51b5',
                }}
              >
                {newContent || 'Nhấn để chỉnh sửa...'}
              </Typography>
            ) : (
              <Box>
                <TextField
                  fullWidth
                  label={title.charAt(0).toUpperCase() + title.slice(1)}
                  variant="outlined"
                  multiline
                  value={newContent}
                  onChange={handleContentChange}
                  rows={countRow}
                />
                <Box
                  gap={2}
                  mt={2}
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'flex-end',
                  }}
                >
                  <Button color="primary" onClick={() => setIsEditing(false)}>
                    Hủy
                  </Button>
                  <Button variant="contained" onClick={handleUpdateContent}>
                    Cập nhật {title}
                  </Button>
                </Box>
              </Box>
            )}
          </ManagementWithTitle>
        )}

        <Fab
          color="primary"
          aria-label="add"
          onClick={handleMenuOpen}
          sx={{
            position: 'fixed',
            bottom: 16,
            right: 16,
          }}
        >
          <AddIcon />
        </Fab>

        <Menu
          anchorEl={menuAnchor}
          open={Boolean(menuAnchor)}
          onClose={handleMenuClose}
        >
          <ButtonUpdatePart part={part} />
          <ButtonRemovePart id={part.id} onRefresh={handleRemovePart} />
        </Menu>

        <WrappedComponent {...props} part={part} />
      </Box>
    )
  }
}

export default withBasePartContent
