'use strict'
const express = require('express')
const router = express.Router()

router.get('/', (req, res) => {
  let someHTML =
    '<div><p><h2>Wellcome to my server</h2></p><p><a href="/create-tables">Migration Tables</a><br/><a href="/swagger">Swagger</a></p></div>'
  res.send(someHTML)
})
router.get('/create-tables', (req, res) => {
  let models = require('../../db/models')
  models.sequelize.sync().then(() => {
    res.send(
      'Created tables successfully!. Go to <a href="/swagger">swagger</a>',
    )
  })
})

module.exports = router
