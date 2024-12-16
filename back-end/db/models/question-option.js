'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class QuestionOption extends Model {
    /**
     * Helper method for defining associations.
     * This method is not a part of Sequelize lifecycle.
     * The `models/index` file will call this method automatically.
     */
    static associate(models) {
      // define association here
      QuestionOption.belongsTo(models.Question, {
        foreignKey: 'questionId',
        as: 'question',
      })
    }
  }

  QuestionOption.init(
    {
      content: DataTypes.TEXT,
      isCorrect: {
        type: DataTypes.BOOLEAN,
        allowNull: false,
        defaultValue: false,
      },
      orderNum: DataTypes.INTEGER,
    },
    {
      sequelize,
      modelName: 'QuestionOption',
    },
  )
  return QuestionOption
}
