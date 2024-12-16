'use strict'

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.bulkInsert('SectionParts', [
      // Listening Section Parts for Exam 1
      {
        title: 'Listening Part 1',
        instructions:
          'Listen to the conversation and choose the correct answer.',
        content: 'uploads/audio/audio_1733742104597.mp3',
        sectionType: 1,
        type: 2,
        orderNum: 1,
        examId: 1,
        parentId: 1,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        title: 'Listening Part 2',
        instructions:
          'Listen to the lecture and answer the comprehension questions.',
        content: 'uploads/audio/audio_1733742104597.mp3',
        sectionType: 1,
        type: 2,
        orderNum: 2,
        examId: 1,
        parentId: 1,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      // Reading Section Parts for Exam 1
      {
        title: 'Reading Part 1',
        instructions: 'Read the passage and answer the main idea questions.',
        content:
          'FALL WEATHER\n' +
          '\n' +
          'One of the first things we look for in fall is the first frost and freeze of the season, killing or sending into dormancy the beautiful vegetation you admired all summer long. For some locations along the Canadian border, and in the higher terrain of the West, the first freeze typically arrives by the middle part of September. Cities in the South may not see the first freeze until November, though a frost is very possible before then. A few cities in the Lower 48, including International Falls, Minnesota and Grand Forks, North Dakota, have recorded a freeze in every month of the year. ',
        sectionType: 2,
        type: 2,
        orderNum: 3,
        examId: 1,
        parentId: 2,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
      {
        title: 'Reading Part 2',
        instructions: 'Read the passage and answer the detailed questions.',
        content:
          'FALL WEATHER\n' +
          '\n' +
          'One of the first things we look for in fall is the first frost and freeze of the season, killing or sending into dormancy the beautiful vegetation you admired all summer long. For some locations along the Canadian border, and in the higher terrain of the West, the first freeze typically arrives by the middle part of September. Cities in the South may not see the first freeze until November, though a frost is very possible before then. A few cities in the Lower 48, including International Falls, Minnesota and Grand Forks, North Dakota, have recorded a freeze in every month of the year. ',
        sectionType: 2,
        type: 2,
        orderNum: 4,
        examId: 1,
        parentId: 2,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      // Writing Section Parts for Exam 1
      {
        title: 'Essay Writing',
        instructions: 'Write an essay about the given topic.',
        content:
          'You should spend about 20 minutes on this task.\n' +
          '\n' +
          'You received an email from your English friend, Jane. She asked you for some information about one of your friends. Read part of her email below. \n' +
          '\n' +
          'I’ve just got an email from your friend, An. She said she’s going to take a course in London this summer. She asked if she could stay with my family until she could find an apartment. Can you tell me a bit about her (things like her personality, hobbies and interests, and her current work or study if possible)? I want to see if she will fit in with my family. \n' +
          '\n' +
          ' Write an email responding to Jane.  \n' +
          '\n' +
          'You should write at least 120 words. You do not need to include your name or addresses. Your response will be evaluated in terms of Task Fulfillment, Organization, Vocabulary and Grammar. ',
        sectionType: 3,
        type: 2,
        orderNum: 5,
        examId: 1,
        parentId: 3,
        createdAt: new Date(),
        updatedAt: new Date(),
      },

      {
        title: 'Speaking Part 1',
        instructions: 'Fill in the blanks with the correct form of the words.',
        content:
          'Let’s talk about your free time activities.\n' +
          '\n' +
          '- What do you often do in your free time?\n' +
          '\n' +
          '- Do you watch TV? If no, why not? If yes, which TV channel do you like best? Why?\n' +
          '\n' +
          '- Do you read books? If no, why not? If yes, what kinds of books do you like best? Why? \n' +
          '\n' +
          'Let’s talk about your neighborhood.\n' +
          '\n' +
          '- Can you tell me something about your neighborhood? \n' +
          '\n' +
          '- What do you like most about it? \n' +
          '\n' +
          '- Do you plan to live there for a long time? Why/why not?',
        orderNum: 6,
        sectionType: 4,
        type: 2,
        examId: 1,
        parentId: 4,
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ])
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.bulkDelete('SectionParts', null, {})
  },
}
