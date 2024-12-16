'use strict'
const { Model } = require('sequelize')
module.exports = (sequelize, DataTypes) => {
  class SectionPart extends Model {
    /**
     * Helper method for defining associations.
     * This method is not a part of Sequelize lifecycle.
     * The `models/index` file will call this method automatically.
     */
    static associate(models) {
      // define association here
      SectionPart.belongsTo(models.Exam, { foreignKey: 'examId', as: 'exam' })
      SectionPart.hasMany(models.SectionPart, {
        foreignKey: 'parentId',
        as: 'children',
      })

      SectionPart.belongsTo(models.SectionPart, {
        foreignKey: 'parentId',
        as: 'parent',
      })
      SectionPart.hasMany(models.Question, {
        foreignKey: 'passageId',
        as: 'questions',
      })
    }
  }

  SectionPart.init(
    {
      id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      title: DataTypes.STRING,
      instructions: { type: DataTypes.TEXT, allowNull: true },
      content: { type: DataTypes.TEXT, allowNull: true },
      orderNum: DataTypes.INTEGER,
      sectionType: DataTypes.INTEGER,
      type: DataTypes.INTEGER,
      examId: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
          model: 'Exams',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      parentId: {
        type: DataTypes.INTEGER,
        allowNull: true,
        references: {
          model: 'SectionParts',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
    },
    {
      sequelize,
      modelName: 'SectionPart',
    },
  )
  return SectionPart
}
