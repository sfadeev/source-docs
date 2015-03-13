// var $ = require('jquery');
var AppActions = require('../actions/AppActions');

module.exports = {

  loadRepositories: function() {
  	$.ajax({
	  url: "/api/repositories"
	}).done(function(data) {
	  AppActions.receiveRepositories(data);
	});
  },

  loadRepositoryIndex: function(repositoryId, branchName) {
  	$.ajax({
	  url: "/api/repositories/" + repositoryId + "/" + branchName + "/index"
	}).done(function(data) {
	  AppActions.receiveRepositoryIndex(data);
	});
  }

};