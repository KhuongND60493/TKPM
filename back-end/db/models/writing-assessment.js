'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class WritingAssessment extends Model {
    /**
     * Helper method for defining associations.
     * This method is not a part of Sequelize lifecycle.
     * The `models/index` file will call this method automatically.
     */
    static associate(models) {
      // define association here
      // WritingAssessment.belongsTo(models.Answer, {
      //   foreignKey: 'answerId',
      // })

      WritingAssessment.belongsTo(models.Answer, {
        foreignKey: 'answerId',
        as: 'answer',
      })
    }
  }

  WritingAssessment.init(
    {
      taskAchievement: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      coherenceCohesion: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      lexicalResource: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      grammarAccuracy: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      detailedFeedback: {
        type: DataTypes.TEXT,
        allowNull: true,
      },
      assessedAt: {
        type: DataTypes.DATE,
        allowNull: true,
      },
    },
    {
      sequelize,
      modelName: 'WritingAssessment',
    },
  )
  return WritingAssessment
}
