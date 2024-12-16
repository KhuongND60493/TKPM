'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class Answer extends Model {
    /**
     * Helper method for defining associations.
     * This method is not a part of Sequelize lifecycle.
     * The `models/index` file will call this method automatically.
     */
    static associate(models) {
      // define association here
      Answer.belongsTo(models.StudentAttempt, { foreignKey: 'attemptId' })
      Answer.belongsTo(models.Question, { foreignKey: 'questionId' })
      Answer.belongsTo(models.QuestionOption, {
        foreignKey: 'questionOptionId',
      })
      Answer.hasOne(models.WritingAssessment, {
        foreignKey: 'answerId',
      })
    }
  }

  Answer.init(
    {
      essayAnswer: {
        type: DataTypes.TEXT,
        allowNull: true, // Chỉ áp dụng cho câu hỏi dạng viết
      },
      aiFeedback: {
        type: DataTypes.TEXT,
        allowNull: true, // Phản hồi của AI (chỉ cho câu hỏi dạng viết)
      },
      score: {
        type: DataTypes.INTEGER,
        allowNull: true, // Điểm của câu trả lời
      },
    },
    {
      sequelize,
      modelName: 'Answer',
    },
  )
  return Answer
}
