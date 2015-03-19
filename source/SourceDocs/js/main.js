/** @jsx React.DOM */
var React = require('react');

var Router = require('react-router');
var DefaultRoute = Router.DefaultRoute;
var Link = Router.Link;
var Route = Router.Route;

var bootstrap = require('bootstrap');

var Repository = require('./components/Repository.js');
var WebApiUtils = require('./utils/WebApiUtils.js');

// React.render(<Repository />, );

var App = React.createClass({
  render: function () {
    return (
      <div>
        <Router.RouteHandler {...this.props} />
        <Link to="app" className="navbar-brand"><span className="glyphicon glyphicon-home" aria-hidden="true"></span></Link>
        <Link to="settings">Settings</Link>
      </div>
    );
  }
});

var Settings = React.createClass({
  render: function () {
  	console.log("RepositorySelector.Settings", this.state, this.props);

    return (
      <h1>Settings</h1>
    );
  }
});

var routes = (
  <Route name="app" path="/" handler={App}>
    <Route name="settings" handler={Settings} />
    <Route name="repo" path="?:repoId/?:branchName/*" handler={Repository} />
    <DefaultRoute handler={Repository} />
  </Route>
);

Router.run(routes, Router.HistoryLocation, function (Handler, state) {
	console.log("Router.Handler", state);
	React.render(<Handler {...state.params} />, document.getElementById('app'));
});

WebApiUtils.loadRepositories();
