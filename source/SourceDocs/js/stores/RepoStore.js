var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');

var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');

var CHANGE_EVENT = 'change';

var _repositories = [],
	_selectedRepositoryId,
	_selectedRepositoryBranchId;

var RepoStore = assign({}, EventEmitter.prototype, {

	// todo: use array.find
	_find: function(array, predicate) {
		for (var i = 0; i < array.length; i++) {
			if (predicate(array[i])) {
				return array[i];
			}
		}

		return null;
	},

	setRepositories: function(data) {

		_repositories = data;

		this.selectRepository();
	},

	getRepositories: function() {
		return _repositories;
	},

	selectRepository: function(id) {
		var selected = this.getSelectedRepository();

	  	if (selected) {
	  		selected.selected = false;
	  		selected = null;
	  	}

		if (id) {
			selected = this._find(_repositories, function (item) {
	  			return item.id == id;
	  		});
		}

	  	if (!selected) {
	  		selected = this._find(_repositories, function (item) {
	  			return item.url != undefined;
	  		});
	  	}

	  	if (selected) {
	  		selected.selected = true;

	  		_selectedRepositoryId = selected.id;
	  		_repositories.title = selected.id;
	  	}
	  	else {
	  		_selectedRepositoryId = null;
			_repositories.title = null;
	  	}

	  	this.selectRepositoryBranch();
	},

	selectRepositoryBranch: function(id) {

		var selected,
			selectedRepository = this.getSelectedRepository();
		
		if (selectedRepository) {
			if (id) {
				selected = this._find(selectedRepository.nodes, function (item) {
		  			return item.name == id;
		  		});
	  		}

		  	if (!selected && selectedRepository.nodes.length > 0) {
		  		selected = this._find(selectedRepository.nodes, function (item) {
		  			return item.name == "master";
		  		}) || selectedRepository.nodes[0];
		  	}

	  		if (selected) {
	  			selected.selected = true;
	  			selectedRepository.nodes.title = selected.name;
	  		}
		}
	},

	getSelectedRepository: function() {
		if (_selectedRepositoryId) {
			return this._find(_repositories, function (item) {
	  			return item.id == _selectedRepositoryId;
	  		});
  		}
  		return null;
	},

	emitChange: function() {
		this.emit(CHANGE_EVENT);
	},

	/**
	* @param {function} callback
	*/
	addChangeListener: function(callback) {
		this.on(CHANGE_EVENT, callback);
	},

	/**
	* @param {function} callback
	*/
	removeChangeListener: function(callback) {
		this.removeListener(CHANGE_EVENT, callback);
	},

	dispatcherIndex: AppDispatcher.register(function(payload) {
		var action = payload.action;

		switch(action.actionType) {
			case AppConstants.LOAD_REPOSITORIES:
				RepoStore.setRepositories(action.data);
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY:
				RepoStore.selectRepository(action.id);
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY_BRANCH:
				RepoStore.selectRepositoryBranch(action.id);
				RepoStore.emitChange();
				break;
		}

		return true; // No errors. Needed by promise in Dispatcher.
	})
});

module.exports = RepoStore;
