'use strict'

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.bulkInsert('Questions', [
      {
        questionText: 'What is the purpose of the conversation?',
        point: 1,
        orderNum: 1,
        passageId: 5,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'What does the speaker mean by "efficiency"?',
        point: 1,
        orderNum: 2,
        passageId: 5,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'What is the main topic of the lecture?',
        point: 1,
        orderNum: 1,
        passageId: 6, // Listening Part 2
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'What example does the lecturer give about teamwork?',
        point: 1,
        orderNum: 2,
        passageId: 6,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      // Questions for Reading Section Parts in Exam 1
      {
        questionText: 'What is the central idea of the passage?',
        point: 1,
        orderNum: 1,
        passageId: 7,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText:
          'Which of the following best describes the tone of the passage?',
        point: 5,
        orderNum: 2,
        passageId: 7,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'What does the author imply by "sustainable growth"?',
        point: 5,
        orderNum: 1,
        passageId: 8,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'Which statement is supported by the passage?',
        point: 5,
        orderNum: 2,
        passageId: 8,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText:
          'Write an essay on the importance of technology in education.',
        point: 10,
        orderNum: 1,
        passageId: 9,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText:
          'Describe the impact of social media on communication skills.',
        point: 10,
        orderNum: 2,
        passageId: 9,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText: 'Discuss your favorite book and why you like it.',
        point: 10,
        orderNum: 1,
        passageId: 10,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        questionText:
          'Explain the importance of teamwork in achieving success.',
        point: 10,
        orderNum: 2,
        passageId: 10,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ])
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('Questions', null, {})
  },
}
