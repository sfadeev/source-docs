var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');

var AppActions = {
  receiveRepositories: function(data){
    AppDispatcher.handleServerAction({
      actionType:AppConstants.RECEIVE_REPOSITORIES,
      data: data
    })
  },
  addItem: function(item){
    AppDispatcher.handleViewAction({
      actionType:AppConstants.ADD_ITEM,
      item: item
    })
  }
}

module.exports = AppActions
