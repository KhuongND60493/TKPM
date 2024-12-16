const Redis = require('redis')
const redisClient = Redis.createClient()

redisClient.on('error', (err) => {
  console.error('Redis connection error:', err)
})

redisClient.connect()

module.exports = redisClient
