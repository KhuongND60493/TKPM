const FormData = require('form-data') // Thư viện xử lý FormData
const fs = require('fs')
const path = require('path') // Module để đọc file

const typeSections = {
  listening: 1,
  reading: 2,
  writing: 3,
  speaking: 4,
}
const typeSectionPart = {
  section: 1,
  part: 2,
  passage: 3,
}

const hostStudentSection = {
  [`${typeSections.listening}`]: 'submit-section',
  [`${typeSections.reading}`]: 'submit-section',
  [`${typeSections.writing}`]: 'submit-section',
  [`${typeSections.speaking}`]: 'submit-speaking-section',
}

const fnParseData = {
  [`${typeSections.listening}`]: function (questions) {
    return parseQuestionFromReadingListening(questions)
  },
  [`${typeSections.reading}`]: function (questions) {
    return parseQuestionFromReadingListening(questions)
  },
  [`${typeSections.writing}`]: function (questions) {
    return questions.map((x) => {
      return {
        questionId: x.id,
        essayAnswer: x.answer,
      }
    })
  },
  [`${typeSections.speaking}`]: function (userId, req) {
    const formData = new FormData()
    formData.append('UserId', userId)
    formData.append('Scope.Type', req.body['type'])
    formData.append('Scope.SectionPartId', req.body['partId'])
    req.files.forEach((file, index) => {
      const match = file.fieldname.match(/answers\[(\d+)\]\.audioFile/)
      if (match) {
        const index = match[1]
        const questionId = req.body[`answers[${index}].id`]
        formData.append(`answers[${index}].questionId`, questionId)
        formData.append(file.fieldname, fs.createReadStream(file.path))
      }
    })
    return formData
  },
}
module.exports = {
  typeSections,
  typeSectionPart,
  hostStudentSection,
  fnParseData,
}
const audioFilePath = 'audio_1733742104597.mp3'

function parseQuestionFromReadingListening(questions) {
  return questions.map((x) => {
    return {
      questionId: x.id,
      selectedOptionId: x.answer,
    }
  })
}
