'use strict'
const bcrypt = require('bcrypt')

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    const items = [
      {
        email: 'admin@gmail.com',
        password: bcrypt.hashSync('Demo@123', 8),
        firstName: 'Nguyện Thị',
        lastName: 'Admin',
        role: 0,
      },
      {
        email: 'teacher@gmail.com',
        password: bcrypt.hashSync('Demo@123', 8),
        firstName: 'Trần Văn',
        lastName: 'Giáo Viên',
        role: 1,
      },
      {
        email: 'student@gmail.com',
        password: bcrypt.hashSync('Demo@123', 8),
        firstName: 'Phan Văn',
        lastName: 'Sinh Viên',
        role: 2,
      },
    ]
    items.forEach((item) => {
      item.createdAt = Sequelize.literal('NOW()')
      item.updatedAt = Sequelize.literal('NOW()')
    })
    await queryInterface.bulkInsert('Users', items, {})
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('Users', null, {})
  },
}
