var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');

var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');
var WebApiUtils = require('../utils/WebApiUtils.js');

var CHANGE_EVENT = 'change';

var _repositories = [],
	_repositoryIndex = {},
	_repositoryIndexList = [],
	_repositoryDocument,
	_selectedRepositoryId,
	_selectedRepositoryBranchId,
	_selectedRepositoryDocumentPath;

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

		this.selectRepository(_selectedRepositoryId);
	},

	setRepositoryIndex: function(data) {

		_repositoryIndex = data;
		_repositoryIndexList = [];

		var prevChild;
		var buildRepositoryIndexListRecursive = function(parent, children) {
            if (parent.children) {
                for (var i = 0; i < parent.children.length; i++) {

                    var child = parent.children[i];

                    _repositoryIndexList.push(child);

                    child["sibling:parent"] = parent;

                    if (prevChild) {
                        child["sibling:previous"] = prevChild;
                        prevChild["sibling:next"] = child;
                    }

                    prevChild = child;

                    buildRepositoryIndexListRecursive(child);
                }
            }
        };

		buildRepositoryIndexListRecursive(_repositoryIndex);

		this.selectRepositoryIndexItem(_selectedRepositoryDocumentPath);
		this.searchRepositoryIndexItem("");
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

	selectSiblingRepositoryIndexItem: function(direction) {
		var item, selected;
		item = selected = this.getSelectedRepositoryIndexItem();

		if (item) {
			do {
				item = item["sibling:" + direction];
				if (item && item.visible && item.path) {

					// todo: merge with selectRepositoryIndexItem
					selected.selected = false;
					
					item.selected = true;
					_selectedRepositoryDocumentPath = item.path;
					WebApiUtils.loadRepositoryDocument(_selectedRepositoryId, _selectedRepositoryBranchId, _selectedRepositoryDocumentPath);

					return true;
				}
			} while (item)
		}

		return false;
	},

	selectRepositoryIndexItem: function(path) {

		var selected = this.getSelectedRepositoryIndexItem();

		if (selected) {
	  		selected.selected = false;
	  		selected = null;
  		}

		if (path) {
			selected = this._find(_repositoryIndexList, function (item) {
	  			return item.path == path;
	  		});
  		}

	  	if (!selected && _repositoryIndexList.length > 0) {
	  		selected = this._find(_repositoryIndexList, function (item) {
	  			return item.path == "README.md";
	  		}) || this._find(_repositoryIndexList, function (item) {
	  			return item.path != null;
	  		});
	  	}

	  	if (selected) {
	  		selected.selected = true;

  			_selectedRepositoryDocumentPath = selected.path;

	  		WebApiUtils.loadRepositoryDocument(_selectedRepositoryId, _selectedRepositoryBranchId, _selectedRepositoryDocumentPath);
  		}
  		else {
  			_selectedRepositoryDocumentPath = null;
  		}
	},

	searchRepositoryIndexItem: function(term) {
		_repositoryIndex.searchTerm = term;

		var searchRegExp = term ? new RegExp(term, "i") : null;

		for (var i = 0; i < _repositoryIndexList.length; i++) {
			var item = _repositoryIndexList[i];
			var match = !searchRegExp || !item.name || item.name.search(searchRegExp) > -1;

            item.visible = match;

            if (match) {
                var parent = item["sibling:parent"];
                while (parent) {
                    parent.visible = true;
                    parent = parent["sibling:parent"];
                }
            }
		};
	},

	getSelectedRepositoryId: function() {
		return _selectedRepositoryId;
	},

	getSelectedRepository: function() {
		if (_selectedRepositoryId) {
			return this._find(_repositories, function (item) {
	  			return item.id == _selectedRepositoryId;
	  		});
  		}
  		return null;
	},

	getSelectedRepositoryBranchName: function() {
		return _selectedRepositoryBranchId;
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

	getSelectedRepositoryIndexItem: function() {	
		if (_repositoryIndexList && _selectedRepositoryDocumentPath) {
			return this._find(_repositoryIndexList, function (item) {
	  			return item.path == _selectedRepositoryDocumentPath;
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

	getSelectedRepositoryBreadcrumb: function() {	
  		var breadcrumb = [],
  			item = this.getSelectedRepositoryIndexItem();

		while (item) {
            breadcrumb.unshift(item);
            item = item["sibling:parent"];

        }

  		return breadcrumb;
	},

	getSelectedRepositoryPager: function() {	
  		var item = this.getSelectedRepositoryIndexItem();

		if (item) {
            return {
            	previous: item["sibling:previous"],
            	next: item["sibling:next"]
            };
        }

  		return null;
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
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_SIBLING_REPOSITORY_INDEX_ITEM:
				if (RepoStore.selectSiblingRepositoryIndexItem(action.direction)) {
					RepoStore.emitChange();					
				}
				break;

			case AppConstants.SEARCH_REPOSITORY_INDEX_ITEM:
				RepoStore.searchRepositoryIndexItem(action.term);
				RepoStore.emitChange();
				break;
		}

		return true; // No errors. Needed by promise in Dispatcher.
	})
});

module.exports = RepoStore;
