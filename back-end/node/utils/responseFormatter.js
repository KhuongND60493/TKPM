'use strict';
const format = {};
const baseResponse = (success, message, data = null) => {
    return {
        success: success,
        message: message,
        data: data
    };
}
format.formatResponse = baseResponse;

format.errorHandler = (err, req, res, next) => {
    console.error(err.stack);
    res.status(500).json(baseResponse(false, 'An error occurred', null));
}

module.exports = format;
