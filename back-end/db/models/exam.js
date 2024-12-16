'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class Exam extends Model {
    /**
     * Helper method for defining associations.
     * This method is not a part of Sequelize lifecycle.
     * The `models/index` file will call this method automatically.
     */
    static associate(models) {
      // define association here
      Exam.belongsTo(models.User, { foreignKey: 'userId' })
      Exam.hasMany(models.SectionPart, { foreignKey: 'examId' })
      Exam.hasMany(models.StudentAttempt, { foreignKey: 'examId' })
    }
  }

  Exam.init(
    {
      title: DataTypes.STRING,
      description: DataTypes.STRING,
      duration: {
        type: DataTypes.INTEGER,
        allowNull: false,
        defaultValue: 30,
      },
    },
    {
      sequelize,
      modelName: 'Exam',
    },
  )
  return Exam
}
