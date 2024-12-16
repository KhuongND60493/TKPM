'use strict'
require('dotenv').config()
const path = require('path')
const express = require('express')
const swaggerUi = require('swagger-ui-express')
const cors = require('cors')
const session = require('express-session')
const RedisStore = require('connect-redis').default
const passport = require('./configs/passport')
const redisClient = require('./configs/redisClient')
const swaggerDocs = require('./utils/swaggerConfig')
const { errorHandler } = require('./utils/responseFormatter')

const app = express()
const port = process.env.API_PORT || 4000
const basePath = path.resolve(__dirname, '../..')
app.use(cors())
app.use(express.static(basePath + '/dist'))
app.use(express.json())
app.use(express.urlencoded({ extended: true }))

app.use(
  session({
    store: new RedisStore({ client: redisClient }),
    secret: process.env.REDIS_URL,
    resave: false,
    saveUninitialized: false,
  }),
)

app.use(express.json())
app.use(passport.initialize())
app.use('/swagger', swaggerUi.serve, swaggerUi.setup(swaggerDocs))
app.use('/api/uploads', express.static(path.join(__dirname, 'uploads')))
app.use('/', require('./routes/rootRouter'))
app.use('/api/users', require('./routes/authRouter'))
app.use('/api/exams', require('./routes/examRouter'))
app.use('/api/section-parts', require('./routes/sectionPartRouter'))
app.use('/api/questions', require('./routes/questionRouter'))
app.use('/api/attempts', require('./routes/attemptRouter'))
app.use('/api/ai', require('./routes/openAIRouter'))
app.use(errorHandler)
app.listen(port, () => {
  console.log(`server start at port: ${port}`)
})
