const passport = require('passport')
const BearerStrategy = require('passport-http-bearer').Strategy
const jwt = require('jsonwebtoken')
const redisClient = require('./redisClient')

passport.use(
  new BearerStrategy(async (token, done) => {
    try {
      const decoded = jwt.verify(token, process.env.ACCESSTOKEN_SECRET)
      // Check if token exists in Redis cache
      const userData = await redisClient.get(`auth:${token}`)
      if (!userData) return done(null, false)

      const user = JSON.parse(userData)
      return done(null, user ? user : false)
    } catch (error) {
      return done(null, false)
    }
  }),
)

module.exports = passport
