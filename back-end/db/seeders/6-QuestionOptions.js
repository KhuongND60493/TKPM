'use strict'

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.bulkInsert('QuestionOptions', [
      // Options for Listening Questions
      {
        content: 'To discuss the project timeline.',
        isCorrect: true,
        orderNum: 1,
        questionId: 1, // "What is the purpose of the conversation?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'To criticize the team members.',
        isCorrect: false,
        orderNum: 2,
        questionId: 1,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'To improve the process of task management.',
        isCorrect: true,
        orderNum: 1,
        questionId: 2, // "What does the speaker mean by 'efficiency'?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'To increase the workload for the team.',
        isCorrect: false,
        orderNum: 2,
        questionId: 2,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The importance of effective communication.',
        isCorrect: true,
        orderNum: 1,
        questionId: 3, // "What is the main topic of the lecture?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The challenges of leadership roles.',
        isCorrect: false,
        orderNum: 2,
        questionId: 3,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'Collaboration in team projects.',
        isCorrect: true,
        orderNum: 1,
        questionId: 4, // "What example does the lecturer give about teamwork?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The inefficiency of teamwork.',
        isCorrect: false,
        orderNum: 2,
        questionId: 4,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      // Options for Reading Questions
      {
        content: 'The main idea is about environmental sustainability.',
        isCorrect: true,
        orderNum: 1,
        questionId: 5, // "What is the central idea of the passage?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The main idea is about economic instability.',
        isCorrect: false,
        orderNum: 2,
        questionId: 5,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'Optimistic and hopeful.',
        isCorrect: true,
        orderNum: 1,
        questionId: 6, // "Which of the following best describes the tone?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'Pessimistic and doubtful.',
        isCorrect: false,
        orderNum: 2,
        questionId: 6,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: '"Sustainable growth" refers to balancing resources.',
        isCorrect: true,
        orderNum: 1,
        questionId: 7, // "What does the author imply by 'sustainable growth'?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: '"Sustainable growth" refers to rapid industrialization.',
        isCorrect: false,
        orderNum: 2,
        questionId: 7,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The company implemented eco-friendly policies.',
        isCorrect: true,
        orderNum: 1,
        questionId: 8, // "Which statement is supported by the passage?"
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        content: 'The company expanded without limitations.',
        isCorrect: false,
        orderNum: 2,
        questionId: 8,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ])
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('QuestionOptions', null, {})
  },
}
