/** @jsx React.DOM */
var React = require('react');

var AppActions = require('../actions/AppActions');
var RepoStore = require('../stores/RepoStore');
// var RepositoryItem = require('./RepositoryItem');

function getState() {
  return {
    repositories: RepoStore.getRepositories()
  };
}

var RepositoryItem = React.createClass({
    
    _onClick: function(e) {
        e.preventDefault();

        this.props.onSelect(this.props.data);
    },

    render: function() {
        // console.log("RepositoryItem.render", this.props);

        if (this.props.data.url) {
            var className = this.props.data.selected ? "active" : "";

            return (
                <li className={className}>
                    <a href={this.props.data.url} onClick={this._onClick}>
                        {this.props.data.id}
                    </a>
                </li>
            );
        }
        else {
            return (
                <li className="dropdown-header">
                    {this.props.data.id}
                </li>
            );
        }
    }
});

var RepositoryList = React.createClass({

    _onChange: function() {
        this.setState(getState());
    },

    _onSelect: function(item) {
        AppActions.selectRepository(item.id);
    },

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
              <RepositoryItem data={item} onSelect={this._onSelect} />
            );
        }, this);
        
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

});

module.exports = RepositoryList;