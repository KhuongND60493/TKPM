const express = require('express')
const attemptController = require('../controllers/attemptController')
const router = express.Router()
router.get('/:id', attemptController.getAttemptByExamId)
module.exports = router
