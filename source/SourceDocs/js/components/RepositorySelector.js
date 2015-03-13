/** @jsx React.DOM */
var React = require('react');

var AppActions = require('../actions/AppActions');
var RepoStore = require('../stores/RepoStore');

var RepositoryList = require('./RepositoryList');
var RepositoryBranchList = require('./RepositoryBranchList');

function getState() {
  return {
    repositories: RepoStore.getRepositories(),
    selectedRepository: RepoStore.getSelectedRepository()
  };
}

var RepositorySelector = React.createClass({


    getInitialState: function() {
        return getState();
    },

    componentDidMount: function() {
        RepoStore.addChangeListener(this._onChange);
    },

    componentWillUnmount: function() {
        RepoStore.removeChangeListener(this._onChange);
    },

    _onChange: function() {
        this.setState(getState());
    },

    _onSelectRepository: function(repository) {
        AppActions.selectRepository(repository.id);
    },

    _onSelectRepositoryBranch: function(branch) {
        AppActions.selectRepositoryBranch(branch.name);
    },

    /**
    * @return {object}
    */
    render: function() {
        // console.log("RepositorySelector.render", this.state);

        return (
            <ul className="nav navbar-nav">
                <RepositoryList data={this.state.repositories} onSelect={this._onSelectRepository} />
                <RepositoryBranchList data={this.state.selectedRepository} onSelect={this._onSelectRepositoryBranch} />
            </ul>
        );
    },

});

module.exports = RepositorySelector;