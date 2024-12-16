const express = require('express')
const { body } = require('express-validator')
const {
  createListeningSectionPart,
  getSectionParts,
  createSectionPart,
  updateSectionPart,
  deleteSectionPart,
  getParts,
  createPart,
  updatePart,
  deletePart,
  getPartsBySection,
  getPartsById,
  updateAudioSectionPart,
  updateContentSectionPart,
  upload,
} = require('../controllers/sectionPartControllers')
const questionController = require('../controllers/questionController')

const sectionPartRouter = express.Router()

sectionPartRouter.post(
  '/listening',
  [
    body('examId').isInt().withMessage('Exam ID must be an integer'),
    body('instructions')
      .optional()
      .isString()
      .withMessage('Instructions must be a string'),
  ],
  createListeningSectionPart,
)

sectionPartRouter.get('/:id', getSectionParts)
sectionPartRouter.get('/:id/questions', questionController.getQuestions)
sectionPartRouter.get('/section/:id/parts', getPartsBySection)
sectionPartRouter.get('/part/:id', getPartsById)
sectionPartRouter.put(
  '/part/:id/audio',
  upload.single('audio'),
  updateAudioSectionPart,
)
sectionPartRouter.put('/part/:id/content', updateContentSectionPart)
sectionPartRouter.post('/', createSectionPart)
sectionPartRouter.put('/:id', updateSectionPart)
sectionPartRouter.delete('/:id', deleteSectionPart)
sectionPartRouter.get('/:parentId/children', getParts)
sectionPartRouter.post('/:parentId/children', createPart)
sectionPartRouter.put('/:id', updatePart)
sectionPartRouter.delete('/:id', deletePart)
module.exports = sectionPartRouter
