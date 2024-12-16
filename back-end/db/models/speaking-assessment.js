'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class SpeakingAssessment extends Model {
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

      SpeakingAssessment.belongsTo(models.Answer, {
        foreignKey: 'answerId',
        as: 'answer',
      })
    }
  }

  SpeakingAssessment.init(
    {
      pronunciation: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      fluency: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      vocabulary: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      grammar: {
        type: DataTypes.INTEGER,
        allowNull: true,
      },
      wordDetails: {
        type: DataTypes.JSONB,
        allowNull: true,
      },
      detailedFeedback: {
        type: DataTypes.TEXT,
        allowNull: true,
      },
      accuracy: {
        type: DataTypes.DECIMAL(4, 2),
        allowNull: false,
      },
      topicScore: {
        type: DataTypes.DECIMAL(4, 2),
        allowNull: false,
      },
      prosody: {
        type: DataTypes.DECIMAL(4, 2),
        allowNull: false,
      },
      transcribedText: {
        type: DataTypes.TEXT,
        allowNull: true,
      },
      transcribedFeedback: {
        type: DataTypes.TEXT,
        allowNull: true,
      },
      audioUrl: DataTypes.STRING,
      assessedAt: {
        type: DataTypes.DATE,
        allowNull: true,
      },
    },
    {
      sequelize,
      modelName: 'SpeakingAssessment',
    },
  )
  return SpeakingAssessment
}
