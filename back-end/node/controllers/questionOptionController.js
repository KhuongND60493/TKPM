const { QuestionOption } = require('../../db/models')
const getOptions = async (req, res) => {
  try {
    const { questionId } = req.params
    const options = await QuestionOption.findAll({ where: { questionId } })
    res.status(200).json(options)
  } catch (err) {
    res.status(500).json({ error: 'Failed to fetch options' })
  }
}
const createEmptyOption = async (req, res) => {
  try {
    const { questionId } = req.body
    const maxOrderOption = await QuestionOption.findOne({
      where: { questionId: questionId },
      order: [['orderNum', 'DESC']],
    })
    const newOrderNumber = maxOrderOption ? maxOrderOption.orderNum + 1 : 0
    const option = await QuestionOption.create({
      content: 'Nội dung câu trả lời',
      isCorrect: false,
      questionId: questionId,
      orderNum: newOrderNumber,
    })
    res.status(201).json({
      data: option,
      message: 'Create question option successfully',
      success: true,
    })
  } catch (err) {
    res.status(500).json({ success: false, message: 'Failed to create option' })
  }
}

const removeOption = async (req, res) => {
  try {
    const { id } = req.params
    const option = await QuestionOption.findByPk(id)
    if (!option) {
      return res
        .status(404)
        .json({ success: false, message: 'Option not found' })
    }
    const questionId = option.questionId
    await option.destroy()
    const options = await QuestionOption.findAll({
      where: { questionId },
      order: [['orderNum', 'ASC']],
    })

    await Promise.all(
      options.map((opt, index) => {
        opt.orderNum = index
        return opt.save()
      }),
    )

    res.status(200).json({
      success: true,
      message: 'Option removed successfully and orderNum updated',
    })
  } catch (err) {
    console.error('Error removing option:', err)
    res.status(500).json({ success: false, message: 'Failed to remove option' })
  }
}

const updateOptions = async (req, res) => {
  try {
    const { options } = req.body

    if (!options || !Array.isArray(options)) {
      return res.status(400).json({
        success: false,
        message: 'Invalid options payload',
      })
    }
    const updatedOptions = []
    for (const option of options) {
      const { id, content, isCorrect } = option

      if (!id || content === undefined || isCorrect === undefined) {
        return res.status(400).json({
          success: false,
          message: 'Each option must have id, content, and isCorrect',
        })
      }
      const existingOption = await QuestionOption.findByPk(id)

      if (!existingOption) {
        return res.status(404).json({
          success: false,
          message: `Option with id ${id} not found`,
        })
      }
      existingOption.content = content
      existingOption.isCorrect = isCorrect
      await existingOption.save()

      updatedOptions.push(existingOption)
    }

    res.status(200).json({
      success: true,
      message: 'Options updated successfully',
      data: updatedOptions,
    })
  } catch (error) {
    console.error('Error updating options:', error)
    res.status(500).json({
      success: false,
      message: 'Failed to update options',
    })
  }
}

const createOption = async (req, res) => {
  try {
    const { content, isCorrect, orderNum, questionId } = req.body
    const option = await QuestionOption.create({
      content,
      isCorrect,
      orderNum,
      questionId,
    })
    res.status(201).json(option)
  } catch (err) {
    res.status(500).json({ error: 'Failed to create option' })
  }
}

// Update an option
const updateOption = async (req, res) => {
  try {
    const { id } = req.params
    const { content, isCorrect, orderNum } = req.body
    const option = await QuestionOption.findByPk(id)
    if (!option) return res.status(404).json({ error: 'Option not found' })
    await option.update({ content, isCorrect, orderNum })
    res.status(200).json(option)
  } catch (err) {
    res.status(500).json({ error: 'Failed to update option' })
  }
}

// Delete an option
const deleteOption = async (req, res) => {
  try {
    const { id } = req.params
    const option = await QuestionOption.findByPk(id)
    if (!option) return res.status(404).json({ error: 'Option not found' })
    await option.destroy()
    res.status(200).json({ message: 'Option deleted successfully' })
  } catch (err) {
    res.status(500).json({ error: 'Failed to delete option' })
  }
}
module.exports = {
  deleteOption,
  updateOption,
  createOption,
  getOptions,
  createEmptyOption,
  removeOption,
  updateOptions,
}
