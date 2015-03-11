/** @jsx React.DOM */
var React = require('react');
var RepoStore = require('../stores/RepoStore');
var RepositoryItem = require('./RepositoryItem');

function getState() {
  return {
    repositories: RepoStore.getAll()
  };
}

var RepositoryList = React.createClass({

    getInitialState: function() {
        return getState();
    },

    componentDidMount: function() {
        RepoStore.addChangeListener(this._onChange);
    },

    componentWillUnmount: function() {
        RepoStore.removeChangeListener(this._onChange);
    },

    /**
    * @return {object}
    */
    render: function() {
        console.log("RepositoryList.render", this.state);

        // return null;

        var items = this.state.repositories.map(function(item, index) {
            return (
              <RepositoryItem data={item} />
            );
        });
        
        return (
            <li className="dropdown">
                <a href="#" className="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
                    {this.state.repositories.title} 
                    <span className="caret"></span>
                </a>
                <ul className="dropdown-menu" role="menu">
                    {items}
                </ul>
            </li>
        );
    },

    _onChange: function() {
        this.setState(getState());
    }
});

module.exports = RepositoryList;