var AppDispatcher = require('../dispatcher/AppDispatcher');
var EventEmitter = require('events').EventEmitter;
var AppConstants = require('../constants/AppConstants');
var assign = require('object-assign');

var CHANGE_EVENT = 'change';

var _repos = [],
	_selected;

var RepoStore = assign({}, EventEmitter.prototype, {

  getAll: function() {
    return _repos;
  },

  find: function(predicate) {
  	for (var i = 0; i < _repos.length; i++) {
  		if (predicate(_repos[i])) {
  			return _repos[i];
  		}
  	}

  	return null;
  },

  select: function(id) {
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
  		selected = this.find(function (item) {
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
      case AppConstants.RECEIVE_REPOSITORIES:
        _repos = action.data;
        RepoStore.select();
        RepoStore.emitChange();
        break;

      case AppConstants.SELECT_REPOSITORY:
        RepoStore.select(action.id);
        RepoStore.emitChange();
        break;

      /*case TodoConstants.TODO_DESTROY:
        destroy(action.id);
        TodoStore.emitChange();
        break;*/

      // add more cases for other actionTypes, like TODO_UPDATE, etc.
    }

    return true; // No errors. Needed by promise in Dispatcher.
  })
});

module.exports = RepoStore;
