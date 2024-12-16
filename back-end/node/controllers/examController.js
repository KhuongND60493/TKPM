'use strict'
const models = require('../../db/models')
const { formatResponse } = require('../utils/responseFormatter')

const { typeSections, typeSectionPart } = require('../configs/enums')
const examController = {}
examController.getAllExam = async (req, res) => {
  const exams = await models.Exam.findAll()
  res
    .status(200)
    .json(formatResponse(true, 'Get list exam successfully', exams))
}
examController.createEmptyExam = async (req, res) => {
  const transaction = await models.sequelize.transaction()
  try {
    // Tạo Exam
    const newExam = await models.Exam.create(
      {
        title: 'Nội dung đề thi',
        description: 'Mô tả đề thi',
        duration: 30,
        userId: req.user.id,
      },
      { transaction },
    )

    const sectionParts = [
      {
        title: 'Kỹ năng nghe',
        instructions: 'Mô tả kỹ năng nghe',
        examId: newExam.id,
        orderNum: 1,
        sectionType: typeSections.listening,
        type: typeSectionPart.section,
      },
      {
        title: 'Kỹ năng đọc',
        instructions: 'Mô tả kỹ năng đọc',
        examId: newExam.id,
        orderNum: 2,
        sectionType: typeSections.reading,
        type: typeSectionPart.section,
      },
      {
        title: 'Kỹ năng viết',
        instructions: 'Mô tả kỹ năng viết',
        examId: newExam.id,
        orderNum: 3,
        sectionType: typeSections.writing,
        type: typeSectionPart.section,
      },
      {
        title: 'Kỹ năng nói',
        instructions: 'Mô tả kỹ năng nói',
        examId: newExam.id,
        orderNum: 4,
        sectionType: typeSections.speaking,
        type: typeSectionPart.section,
      },
    ]

    await models.SectionPart.bulkCreate(sectionParts, { transaction })
    await transaction.commit()

    res.status(201).json({
      success: true,
      message: 'Tạo đề thi và các phần thành công',
      data: newExam,
    })
  } catch (error) {
    await transaction.rollback()
    console.error('Error creating exam and sections:', error)
    res.status(500).json({
      success: false,
      message: 'Tạo đề thi hoặc các phần bị lỗi',
      error: error.message,
    })
  }
}
examController.updateExam = async (req, res) => {
  try {
    const { id } = req.params
    const { title, description, duration } = req.body

    const exam = await models.Exam.findByPk(id)
    if (!exam) {
      return res.status(404).json({
        success: false,
        message: 'Exam not found',
      })
    }

    exam.title = title || exam.title
    exam.description = description || exam.description
    exam.duration = duration
    await exam.save()
    res.status(200).json({
      success: true,
      message: 'Exam updated successfully',
      data: exam,
    })
  } catch (error) {
    res.status(500).json({
      success: false,
      message: 'Failed to update exam',
      error: error.message,
    })
  }
}
examController.deleteExam = async (req, res) => {
  try {
    const examId = parseInt(req.params.id, 10)
    if (!examId) {
      return res
        .status(400)
        .json({ success: false, message: 'Invalid exam ID' })
    }

    const attempts = await models.StudentAttempt.findAll({ where: { examId } })
    if (attempts.length > 0) {
      return res.status(400).json({
        success: false,
        message: 'Không thể xóa đề thi vì đã có thí sinh làm bài',
      })
    }

    const transaction = await models.sequelize.transaction()
    try {
      await models.SectionPart.destroy({
        where: { examId },
        transaction,
      })
      const deleted = await models.Exam.destroy({
        where: { id: examId },
        transaction,
      })

      if (deleted) {
        await transaction.commit()
        return res.status(200).json({
          success: true,
          message: 'Đã xóa đề thi và các phần thành công',
        })
      } else {
        await transaction.rollback()
        return res
          .status(404)
          .json({ success: false, message: 'Không tìm thấy đề thi để xóa' })
      }
    } catch (err) {
      await transaction.rollback()
      console.error('Error during deletion:', err)
      return res.status(500).json({
        success: false,
        message: 'Xóa đề thi thất bại',
      })
    }
  } catch (error) {
    console.error('Error deleting exam:', error)
    return res.status(500).json({ success: false, message: 'Server error' })
  }
}

module.exports = examController
