const axios = require('axios')
const { getGatewayJwtToken } = require('../configs/jwt')
const {
  hostStudentSection,
  typeSections,
  fnParseData,
} = require('../configs/enums')

const hostNetCore = process.env.NETCORE_HOST
const multer = require('multer')
const path = require('path')
const storage = multer.diskStorage({
  destination: function (req, file, cb) {
    const pathDestination = path.resolve(__dirname, '../uploads/speaking')
    cb(null, pathDestination)
  },
  filename: function (req, file, cb) {
    const uniqueName = `speaking_${req.params.attemptId}_${file.originalname}`
    cb(null, uniqueName)
  },
})

const upload = multer({ storage })
const openAIController = {}

openAIController.testConnection = async (req, res) => {
  try {
    const host = `${hostNetCore}/StudentAttempt/test`
    const token = getGatewayJwtToken()
    const response = await axios({
      method: req.method,
      url: host,
      data: req.body,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
    res.status(response.status).json(response.data)
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}

openAIController.allAttempts = async (req, res) => {
  try {
    let examId = isNaN(req.params.examId) ? 0 : parseInt(`${req.params.examId}`)
    const host = `${hostNetCore}/StudentAttempt/attempts`
    const token = getGatewayJwtToken()
    const response = await axios({
      method: req.method,
      url: host,
      data: { ...req.body, userId: req.user.id, examId },
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
    res.status(response.status).json({
      data: response.data,
      success: true,
      message: 'Get attempts successfully',
    })
  } catch (error) {
    console.error('Error', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}
openAIController.allExams = async (req, res) => {
  try {
    const host = `${hostNetCore}/StudentAttempt/exams`
    const token = getGatewayJwtToken()
    const response = await axios({
      method: req.method,
      url: host,
      data: req.body,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
    res.status(response.status).json({
      data: response.data,
      success: true,
      message: 'Get exam successfully',
    })
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}
openAIController.startStudentAttempt = async (req, res) => {
  try {
    const host = `${hostNetCore}/StudentAttempt/start`
    const token = getGatewayJwtToken()

    const response = await axios({
      method: req.method,
      url: host,
      data: { ...req.body, userId: req.user.id },
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    res.status(response.status).json({
      data: response.data,
      success: true,
      message: 'Start attempts successfully',
    })
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res.status(error.response ? error.response.status : 500).json({
      message: 'Error calling .NET Core API',
      success: false,
    })
  }
}
openAIController.submitStudentAnswer = async (req, res) => {
  try {
    let id = isNaN(req.params.attemptId)
      ? 0
      : parseInt(`${req.params.attemptId}`)
    if (id > 0) {
      const host = `${hostNetCore}/StudentAttempt/${id}/submit-answer`
      const token = getGatewayJwtToken()
      const response = await axios({
        method: req.method,
        url: host,
        data: req.body,
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      res.status(response.status).json(response.data)
    } else {
      res.status(500).json({ error: 'Please send your attemptId' })
    }
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}
openAIController.finishStudentAnswer = async (req, res) => {
  try {
    let id = isNaN(req.params.attemptId)
      ? 0
      : parseInt(`${req.params.attemptId}`)
    if (id > 0) {
      const host = `${hostNetCore}/StudentAttempt/${id}/finish`
      const token = getGatewayJwtToken()
      const response = await axios({
        method: req.method,
        url: host,
        data: { ...req.body, userId: req.user.id },
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      res.status(response.status).json({
        data: response.data,
        success: true,
        message: 'Finish attempts successfully',
      })
    } else {
      res.status(500).json({ error: 'Please send your attemptId' })
    }
  } catch (error) {
    if (error.response) {
      res.status(error.response.status).json({
        success: false,
        message: error.response.data?.code || 'Error calling .NET Core API',
        data: error.response.data?.code || 'UNKNOWN_ERROR',
      })
    } else {
      res.status(500).json({
        success: false,
        message: error.message,
      })
    }
  }
}
openAIController.resultStudentAnswer = async (req, res) => {
  try {
    let id = isNaN(req.params.attemptId)
      ? 0
      : parseInt(`${req.params.attemptId}`)
    const userId = req.user.id
    if (id > 0 && userId) {
      const host = `${hostNetCore}/StudentAttempt/${id}/result?userId=${userId}`
      const token = getGatewayJwtToken()
      const response = await axios({
        method: req.method,
        url: host,
        data: req.body,
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      res.status(response.status).json({
        data: response.data,
        success: true,
        message: 'Get result successfully',
      })
    } else {
      res.status(500).json({ error: 'Please send your attemptId and userId' })
    }
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}
openAIController.submitStudentSection = async (req, res) => {
  try {
    const { sectionType = '0', type = '0', partId = '0', questions } = req.body
    let id = isNaN(req.params.attemptId)
      ? 0
      : parseInt(`${req.params.attemptId}`)
    const token = getGatewayJwtToken()
    let typeSection = isNaN(sectionType)
      ? typeSections.listening
      : parseInt(`${sectionType}`)

    let sectionPartId = isNaN(partId) ? 0 : parseInt(`${partId}`)
    const userId = req.user.id
    const host = `${hostNetCore}/StudentAttempt/${id}/${hostStudentSection[typeSection]}`
    if (id <= 0 || sectionPartId === 0) {
      return res
        .status(500)
        .json({ success: false, message: 'Please send your attemptId' })
    } else if (typeSection === typeSections.speaking) {
      if (req.files.length > 0) {
        const form = fnParseData[typeSection](userId, req)
        const response = await axios.post(host, form, {
          headers: {
            'Content-Type': 'multipart/form-data',
            Authorization: `Bearer ${token}`,
          },
        })
        res.status(response.status).json({ data: response.data, success: true })
      } else {
        return res
          .status(500)
          .json({ success: false, message: 'Please send with your audio' })
      }
    } else if (!questions || questions.length === 0) {
      return res.status(500).json({
        message: 'Need partId, TypeSection, Options',
        success: false,
      })
    } else {
      const data = {
        userId: userId,
        scope: {
          type: type,
          sectionPartId: sectionPartId,
        },
        answers: fnParseData[typeSection](questions),
      }
      const response = await axios({
        method: req.method,
        url: host,
        data: data,
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      res.status(response.status).json({ data: response.data, success: true })
    }
  } catch (error) {
    console.error('Error calling .NET Core API:', error.message)
    res
      .status(error.response ? error.response.status : 500)
      .json({ error: 'Error calling .NET Core API' })
  }
}
openAIController.upload = upload
module.exports = openAIController
