'use strict'
const swaggerJsDoc = require('swagger-jsdoc')

const port = process.env.API_PORT || 4000
const swaggerOptions = {
  swaggerDefinition: {
    openapi: '3.0.0',
    info: {
      title: 'TKPM API I/2024 Documentation',
      version: '1.0.0',
      description: 'API Information',
      contact: {
        name: 'Developer',
      },
    },
    servers: [
      {
        url: `http://localhost:${port}/api`,
      },
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: 'http',
          scheme: 'bearer',
          bearerFormat: 'JWT',
        },
      },
    },
  },
  apis: [__dirname + '/../routes/*.js'],
}

const swaggerDocs = swaggerJsDoc(swaggerOptions)
module.exports = swaggerDocs
