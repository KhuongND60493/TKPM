const passport = require('../configs/passport')

const authMiddleware = passport.authenticate('bearer', { session: false })

module.exports = authMiddleware
