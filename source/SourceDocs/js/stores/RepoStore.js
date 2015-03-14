var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');

var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');
var WebApiUtils = require('../utils/WebApiUtils.js');

var CHANGE_EVENT = 'change';

var _repositories = [],
	_repositoryIndex = {},
	_repositoryDocument,
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

	setRepositoryIndex: function(data) {
		_repositoryIndex = data;
		this.selectRepositoryIndexItem();
	},

	setRepositoryDocument: function(data) {
		_repositoryDocument = data;
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

	selectRepositoryBranch: function(name) {

		var selected = this.getSelectedRepositoryBranch(),
			selectedRepository = this.getSelectedRepository();
		
		if (selectedRepository) {

			if (selected) {
		  		selected.selected = false;
		  		selected = null;
	  		}

			if (name) {
				selected = this._find(selectedRepository.nodes, function (item) {
		  			return item.name == name;
		  		});
	  		}

		  	if (!selected && selectedRepository.nodes.length > 0) {
		  		selected = this._find(selectedRepository.nodes, function (item) {
		  			return item.name == "master";
		  		}) || selectedRepository.nodes[0];
		  	}

	  		if (selected) {
	  			selected.selected = true;

	  			_selectedRepositoryBranchId = selected.name;
	  			selectedRepository.nodes.title = selected.name;

	  			WebApiUtils.loadRepositoryIndex(_selectedRepositoryId, _selectedRepositoryBranchId);
	  		}
	  		else {
	  			_selectedRepositoryBranchId = null;
	  			selectedRepository.nodes.title = null;
	  		}
		}
	},

	selectRepositoryIndexItem: function(path) {

	  	WebApiUtils.loadRepositoryDocument(_selectedRepositoryId, _selectedRepositoryBranchId, path || "README.md");

		this.emitChange();
	},

	getSelectedRepository: function() {
		if (_selectedRepositoryId) {
			return this._find(_repositories, function (item) {
	  			return item.id == _selectedRepositoryId;
	  		});
  		}
  		return null;
	},

	getSelectedRepositoryBranch: function() {	
		var selectedRepository = this.getSelectedRepository();
		if (selectedRepository && _selectedRepositoryBranchId) {
			return this._find(selectedRepository.nodes, function (item) {
	  			return item.name == _selectedRepositoryBranchId;
	  		});
  		}
  		return null;
	},

	getSelectedRepositoryIndex: function() {
		return _repositoryIndex;
	},

	getSelectedRepositoryDocument: function() {	
  		return _repositoryDocument;
	},

	emitChange: function() {
		this.emit(CHANGE_EVENT);
	},

	addChangeListener: function(callback) {
		this.on(CHANGE_EVENT, callback);
	},

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

			case AppConstants.LOAD_REPOSITORY_INDEX:
				RepoStore.setRepositoryIndex(action.data);
				RepoStore.emitChange();
				break;

			case AppConstants.LOAD_REPOSITORY_DOCUMENT:
				RepoStore.setRepositoryDocument(action.data);
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY:
				RepoStore.selectRepository(action.id);
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY_BRANCH:
				RepoStore.selectRepositoryBranch(action.name);
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY_INDEX_ITEM:
				RepoStore.selectRepositoryIndexItem(action.path);
				break;
		}

		return true; // No errors. Needed by promise in Dispatcher.
	})
});

module.exports = RepoStore;
