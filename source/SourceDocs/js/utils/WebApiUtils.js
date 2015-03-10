// var $ = require('jquery');
var AppActions = require('../actions/AppActions');

module.exports = {

  getRepositories: function() {
  	$.ajax({
	  url: "/api/repositories"
	}).done(function(data) {
	  AppActions.receiveRepositories(data);
	});
  }

};