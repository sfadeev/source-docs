/** @jsx React.DOM */
var React = require('react');

var AppActions = require('../actions/AppActions');
var RepositorySelector = require('./RepositorySelector');

var App = React.createClass({
    render:function(){
      return (
        <RepositorySelector />
      )
    }
  });

module.exports = App;
