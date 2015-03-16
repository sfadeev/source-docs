var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');

var AppActions = {
	receiveRepositories: function(data){
		AppDispatcher.handleServerAction({
			actionType:AppConstants.LOAD_REPOSITORIES,
			data: data
		})
	},

	receiveRepositoryIndex: function(data){
		AppDispatcher.handleServerAction({
			actionType:AppConstants.LOAD_REPOSITORY_INDEX,
			data: data
		})
	},

	receiveRepositoryDocument: function(data){
		AppDispatcher.handleServerAction({
			actionType:AppConstants.LOAD_REPOSITORY_DOCUMENT,
			data: data
		})
	},

	selectRepository: function(id){
		AppDispatcher.handleViewAction({
			actionType:AppConstants.SELECT_REPOSITORY,
			id: id
		})
	},
	
	selectRepositoryBranch: function(name){
		AppDispatcher.handleViewAction({
			actionType:AppConstants.SELECT_REPOSITORY_BRANCH,
			name: name
		})
	},

	selectRepositoryIndexItem: function(path){
		AppDispatcher.handleViewAction({
			actionType:AppConstants.SELECT_REPOSITORY_INDEX_ITEM,
			path: path
		})
	},

	searchRepositoryIndexItem: function(term){
		AppDispatcher.handleViewAction({
			actionType:AppConstants.SEARCH_REPOSITORY_INDEX_ITEM,
			term: term
		})
	},

}

module.exports = AppActions
