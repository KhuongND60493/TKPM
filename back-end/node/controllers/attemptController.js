const {
  SectionPart,
  Exam,
  Question,
  QuestionOption,
} = require('../../db/models')
const getAttemptByExamId = async (req, res) => {
  try {
    const { id } = req.params
    if (!id) {
      return res.status(400).json({
        success: false,
        message: 'Exam ID is required.',
      })
    }

    const examExists = await Exam.findOne({ where: { id } })
    if (!examExists) {
      return res.status(404).json({
        success: false,
        message: 'Exam not found.',
      })
    }
    const sections = await SectionPart.findAll({
      attributes: ['id', 'title', 'content', 'instructions', 'sectionType'],
      where: { examId: id, parentId: null },
      include: [
        {
          model: SectionPart,
          as: 'children',
          attributes: ['id', 'content', 'instructions', 'title', 'type'],
          order: [['orderNum', 'ASC']],
          include: [
            {
              model: Question,
              attributes: ['id', 'questionText'],
              as: 'questions',
              include: [
                {
                  model: QuestionOption,
                  as: 'options',
                  attributes: ['id', 'content'],
                },
              ],
            },
          ],
        },
      ],
      order: [['orderNum', 'ASC']],
    })

    res.status(200).json({
      success: true,
      data: sections.map((section) => {
        const updatedChildren = section.children.map((child) => {
          const randomizedQuestions = shuffleArray(
            child.questions.map((question) => {
              const randomizedOptions = shuffleArray(question.options)
              return {
                ...question.toJSON(),
                options: randomizedOptions,
              }
            }),
          )
          return {
            ...child.toJSON(),
            questions: randomizedQuestions,
          }
        })

        return {
          id: section.id,
          title: section?.title || '',
          instructions: section?.instructions || '',
          sectionType: section?.sectionType || null,
          parts: updatedChildren,
        }
      }),
      message: 'Get attempts successfully',
    })
  } catch (err) {
    console.error(err)
    res.status(500).json({
      success: false,
      message: 'Failed to fetch questions',
    })
  }
}

module.exports = {
  getAttemptByExamId,
}
function shuffleArray(array) {
  for (let i = array.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    ;[array[i], array[j]] = [array[j], array[i]]
  }
  return array
}
