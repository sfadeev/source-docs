/** @jsx React.DOM */
// var jQuery = require('jquery-browserify');
var bootstrap = require('bootstrap');
var React = require('react');

// Not ideal to use createFactory, but don't know how to use JSX to solve this
// Posted question at: https://gist.github.com/sebmarkbage/ae327f2eda03bf165261
var RepositorySelector = require('./components/RepositorySelector.js');
var RepositoryIndex = require('./components/RepositoryIndex.js');
var RepositoryDocument = require('./components/RepositoryDocument.js');

var WebApiUtils = require('./utils/WebApiUtils.js');

React.render(
	<RepositorySelector />,
	document.getElementById('main')
);

React.render(
	<RepositoryIndex />,
	document.getElementById('repo-index-region')
);

React.render(
	<RepositoryDocument />,
	document.getElementById('repo-content-region')
);

WebApiUtils.loadRepositories();
