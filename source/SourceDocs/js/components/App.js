/** @jsx React.DOM */
var React = require('react');
var AppActions = require('../actions/AppActions');
var AppStore = require('../stores/AppStore');
var RepositoryList = require('./RepositoryList');

var App = React.createClass({
    /*handleClick:function(){
      AppActions.addItem('this is the item');
    },*/
    render:function(){
      return (
        <RepositoryList />
        /*<div className="wrapper">
          <h3 onClick={this.handleClick}>Click this Title, then check console</h3>
        </div>*/
      )
    }
  });

module.exports = App;
