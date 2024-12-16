'use strict'
const express = require('express')
const router = express.Router()
const examController = require('../controllers/examController')
const authMiddleware = require('../middlewares/authMiddleware')
/**
 * @swagger
 * tags:
 *   - name: "Exams"
 *     description: "API liên quan đến các bài thi"
 */

/**
 * @swagger
 * /exams:
 *   get:
 *     tags:
 *       - "Exams"
 *     summary: Lấy danh sách các bài thi
 *     responses:
 *       200:
 *         description: Thành công
 */
router.get('/', examController.getAllExam)
router.post('/', authMiddleware, examController.createEmptyExam)
router.put('/:id', authMiddleware, examController.updateExam)
router.delete('/:id', authMiddleware, examController.deleteExam)
module.exports = router
