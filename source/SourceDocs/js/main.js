/** @jsx React.DOM */
var React = require('react');
var bootstrap = require('bootstrap');
var App = require('./components/App.js');
var WebApiUtils = require('./utils/WebApiUtils.js');

React.render(<App />, document.getElementById('app'));

WebApiUtils.loadRepositories();
