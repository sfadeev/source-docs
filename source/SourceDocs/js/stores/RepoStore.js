var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');

var AppDispatcher = require('../dispatcher/AppDispatcher');
var AppConstants = require('../constants/AppConstants');

var CHANGE_EVENT = 'change';

var _repos = [],
	_selected;

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

	getRepositories: function() {
		return _repos;
	},

	selectRepository: function(id) {
		var selected;

		if (id) {
		  	for (var i = 0; i < _repos.length; i++) {
		  		var item = _repos[i];
		  		item.selected = (item.id == id);
		  		if (item.selected) {
		  			selected = item;
		  		}
		  	}
		}

	  	if (!selected) {
	  		selected = this._find(_repos, function (item) {
	  			return item.url != undefined;
	  		});
	  	}

	  	if (_selected) _selected.selected = false;

	  	if (selected) {
	  		_selected = selected;
	  		_selected.selected = true;
	  		_repos.title = _selected.id;
	  	}
	  	else {
			_repos.title = null;
	  	}
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
				_repos = action.data;
				RepoStore.selectRepository();
				RepoStore.emitChange();
				break;

			case AppConstants.SELECT_REPOSITORY:
				RepoStore.selectRepository(action.id);
				RepoStore.emitChange();
				break;
		}

		return true; // No errors. Needed by promise in Dispatcher.
	})
});

module.exports = RepoStore;
