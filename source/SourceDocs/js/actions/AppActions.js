var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');

var AppActions = {
  receiveRepositories: function(data){
    AppDispatcher.handleServerAction({
      actionType:AppConstants.RECEIVE_REPOSITORIES,
      data: data
    })
  },
  selectRepository: function(id){
    AppDispatcher.handleViewAction({
      actionType:AppConstants.SELECT_REPOSITORY,
      id: id
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
