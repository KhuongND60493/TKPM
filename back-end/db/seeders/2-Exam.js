'use strict'

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.bulkInsert('Exams', [
      {
        title: 'VSTEP B2 HK I/2024',
        description: 'Practice your 4 skills for VSTEP B2.',
        duration: 30,
        userId: 2, // Teacher ID
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ])
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('Exams', null, {})
  },
}
