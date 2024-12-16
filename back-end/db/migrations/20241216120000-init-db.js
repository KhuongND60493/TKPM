'use strict'

module.exports = {
  up: async (queryInterface, Sequelize) => {
    // Tạo bảng Users
    await queryInterface.createTable('Users', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      email: {
        type: Sequelize.STRING,
        allowNull: false,
        unique: true,
      },
      password: {
        type: Sequelize.STRING,
        allowNull: false,
      },
      firstName: {
        type: Sequelize.STRING,
        allowNull: true,
      },
      lastName: {
        type: Sequelize.STRING,
        allowNull: true,
      },
      role: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng Exams
    await queryInterface.createTable('Exams', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      title: {
        type: Sequelize.STRING,
        allowNull: false,
      },
      description: {
        type: Sequelize.STRING,
        allowNull: true,
      },
      duration: {
        type: Sequelize.INTEGER,
        allowNull: false,
        defaultValue: 30,
      },
      userId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Users',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng SectionParts
    await queryInterface.createTable('SectionParts', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      title: {
        type: Sequelize.STRING,
        allowNull: true,
      },
      instructions: {
        type: Sequelize.TEXT,
        allowNull: true,
      },
      content: {
        type: Sequelize.TEXT,
        allowNull: true,
      },
      orderNum: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      sectionType: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      type: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      examId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Exams',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      parentId: {
        type: Sequelize.INTEGER,
        allowNull: true,
        references: {
          model: 'SectionParts',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng Questions
    await queryInterface.createTable('Questions', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      questionText: {
        type: Sequelize.TEXT,
        allowNull: false,
      },
      point: {
        type: Sequelize.INTEGER,
        allowNull: false,
      },
      orderNum: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      passageId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'SectionParts',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng QuestionOptions
    await queryInterface.createTable('QuestionOptions', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      content: {
        type: Sequelize.TEXT,
        allowNull: false,
      },
      isCorrect: {
        type: Sequelize.BOOLEAN,
        allowNull: false,
        defaultValue: false,
      },
      orderNum: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      questionId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Questions',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng StudentAttempts
    await queryInterface.createTable('StudentAttempts', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      startTime: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      endTime: {
        type: Sequelize.DATE,
        allowNull: true,
      },
      duration: {
        type: Sequelize.INTEGER,
        allowNull: false,
        defaultValue: 30,
      },
      finalScore: {
        type: Sequelize.DECIMAL(4, 2),
        allowNull: false,
        defaultValue: 0,
      },
      status: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      userId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Users',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      examId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Exams',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng Answers
    await queryInterface.createTable('Answers', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      essayAnswer: {
        type: Sequelize.TEXT,
        allowNull: true,
      },
      aiFeedback: {
        type: Sequelize.TEXT,
        allowNull: true,
      },
      score: {
        type: Sequelize.INTEGER,
        allowNull: true,
      },
      attemptId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'StudentAttempts',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      questionId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Questions',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      questionOptionId: {
        type: Sequelize.INTEGER,
        allowNull: true,
        references: {
          model: 'QuestionOptions',
          key: 'id',
        },
        onDelete: 'SET NULL',
      },
      createdAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
      updatedAt: {
        type: Sequelize.DATE,
        allowNull: false,
      },
    })

    // Tạo bảng WritingAssessments
    await queryInterface.createTable('WritingAssessments', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      taskAchievement: Sequelize.INTEGER,
      coherenceCohesion: Sequelize.INTEGER,
      lexicalResource: Sequelize.INTEGER,
      grammarAccuracy: Sequelize.INTEGER,
      detailedFeedback: Sequelize.TEXT,
      assessedAt: Sequelize.DATE,
      answerId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Answers',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: Sequelize.DATE,
      updatedAt: Sequelize.DATE,
    })

    // Tạo bảng SpeakingAssessments
    await queryInterface.createTable('SpeakingAssessments', {
      id: {
        type: Sequelize.INTEGER,
        autoIncrement: true,
        primaryKey: true,
      },
      pronunciation: Sequelize.INTEGER,
      fluency: Sequelize.INTEGER,
      vocabulary: Sequelize.INTEGER,
      grammar: Sequelize.INTEGER,
      wordDetails: Sequelize.JSONB,
      detailedFeedback: Sequelize.TEXT,
      accuracy: Sequelize.DECIMAL(4, 2),
      topicScore: Sequelize.DECIMAL(4, 2),
      prosody: Sequelize.DECIMAL(4, 2),
      transcribedFeedback: Sequelize.TEXT,
      transcribedText: Sequelize.TEXT,
      audioUrl: Sequelize.STRING,
      assessedAt: Sequelize.DATE,
      answerId: {
        type: Sequelize.INTEGER,
        allowNull: false,
        references: {
          model: 'Answers',
          key: 'id',
        },
        onDelete: 'CASCADE',
      },
      createdAt: Sequelize.DATE,
      updatedAt: Sequelize.DATE,
    })
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.dropTable('SpeakingAssessments')
    await queryInterface.dropTable('WritingAssessments')
    await queryInterface.dropTable('Answers')
    await queryInterface.dropTable('StudentAttempts')
    await queryInterface.dropTable('QuestionOptions')
    await queryInterface.dropTable('Questions')
    await queryInterface.dropTable('SectionParts')
    await queryInterface.dropTable('Exams')
    await queryInterface.dropTable('Users')
  },
}
