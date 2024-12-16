const { typeSections } = require('../configs/enums')
const validateSectionPartRequest = (req, res, next) => {
  const { examId, type } = req.query
  if (!examId || isNaN(examId)) {
    return res.status(400).json({
      success: false,
      message: 'Invalid or missing examId. It must be a number.',
    })
  }

  if (
    type === undefined ||
    isNaN(type) ||
    !Object.values(typeSections).includes(Number(type))
  ) {
    return res.status(400).json({
      success: false,
      message: `Invalid or missing type. Allowed values are: ${Object.values(typeSections).join(', ')}.`,
    })
  }

  next()
}
module.exports = { validateSectionPartRequest }
