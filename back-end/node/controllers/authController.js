const bcrypt = require('bcryptjs')
const models = require('../../db/models')

const { formatResponse } = require('../utils/responseFormatter')
const {
  generateAccessToken,
  generateRefreshToken,
  invalidateTokens,
} = require('../configs/jwt')
const redisClient = require('../configs/redisClient')

const register = async (req, res) => {
  try {
    const { email, password, lastName, firstName, role } = req.body
    const hashedPassword = await bcrypt.hash(password, 10)
    const user = new models.User({
      email,
      password: hashedPassword,
      lastName,
      firstName,
      role,
    })
    await user.save()
    res.status(201).json({ message: 'User registered successfully' })
  } catch (error) {
    res.status(500).json({ error: 'Registration failed' })
  }
}

const login = async (req, res) => {
  try {
    const { email, password } = req.body
    const user = await models.User.findOne({ where: { email } })
    if (!user || !(await bcrypt.compare(password, user.password))) {
      return res.status(401).json(formatResponse(false, 'Invalid credentials'))
    }
    const accessToken = generateAccessToken(user)
    const refreshToken = generateRefreshToken(user)
    redisClient.setEx(`auth:${accessToken}`, 3600, JSON.stringify(user))
    res.json(
      formatResponse(true, 'Login successfully', {
        accessToken,
        refreshToken,
        user: {
          id: user.id,
          role: user.role,
          lastName: user.lastName,
          firstName: user.firstName,
        },
      }),
    )
  } catch (error) {
    res.status(500).json(formatResponse(false, 'Login failed'))
  }
}

const refreshAccessToken = async (req, res) => {
  const { refreshToken } = req.body
  if (!refreshToken)
    return res.status(400).json({ error: 'Refresh token required' })

  try {
    const decoded = jwt.verify(refreshToken, process.env.ACCESSTOKEN_SECRET)
    const userData = await redisClient.get(`refresh:${refreshToken}`)
    if (!userData)
      return res.status(403).json({ error: 'Invalid or expired refresh token' })

    const user = JSON.parse(userData)
    const newAccessToken = generateAccessToken(user)
    const newRefreshToken = generateRefreshToken(user)

    redisClient.setEx(`auth:${newAccessToken}`, 3600, JSON.stringify(user))
    redisClient.del(`refresh:${refreshToken}`)
    res.json({ accessToken: newAccessToken, refreshToken: newRefreshToken })
  } catch (error) {
    res.status(403).json({ error: 'Invalid or expired refresh token' })
  }
}

const profile = (req, res) => {
  res.status(200).json(
    formatResponse(true, 'User Profile', {
      user: {
        id: req?.user.id,
        role: req?.user.role,
        lastName: req?.user.lastName,
        firstName: req?.user.firstName,
      },
    }),
  )
}

module.exports = { register, login, refreshAccessToken, profile }
