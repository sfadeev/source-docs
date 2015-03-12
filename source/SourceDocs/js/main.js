/** @jsx React.DOM */
// var jQuery = require('jquery-browserify');
var bootstrap = require('bootstrap');
var React = require('react');

// Not ideal to use createFactory, but don't know how to use JSX to solve this
// Posted question at: https://gist.github.com/sebmarkbage/ae327f2eda03bf165261
var App = require('./components/app.js');
var WebApiUtils = require('./utils/WebApiUtils.js');

React.render(
  <App />,
  document.getElementById('main')
);

WebApiUtils.getRepositories();
