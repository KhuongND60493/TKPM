'use strict'

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.bulkInsert('SectionParts', [
      // Sections for Exam 1
      {
        title: 'Listening Section',
        instructions: 'Listen carefully to the audio and answer the questions.',
        content: null,
        orderNum: 1,
        sectionType: 1,
        type: 1,
        examId: 1,
        parentId: null,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        title: 'Reading Section',
        instructions:
          'Read the passage and answer the comprehension questions.',
        content: null,
        sectionType: 2,
        type: 1,
        orderNum: 2,
        examId: 1,
        parentId: null,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      {
        title: 'Writing Section',
        instructions: 'Write an essay based on the given topic.',
        content: null,
        sectionType: 3,
        type: 1,
        orderNum: 3,
        examId: 1,
        parentId: null,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        title: 'Speaking Section',
        instructions: 'Fill in the blanks with the correct words.',
        content: null,
        sectionType: 4,
        type: 1,
        orderNum: 4,
        examId: 1,
        parentId: null,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ])
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('SectionParts', null, {})
  },
}
