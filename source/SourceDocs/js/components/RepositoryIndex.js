/** @jsx React.DOM */
var React = require('react');
var Router = require('react-router');

var AppActions = require('../actions/AppActions');
var RepoStore = require('../stores/RepoStore');

var RepositoryIndexItem = React.createClass({

    _onClick: function(e) {
        e.preventDefault();

        this.props.onSelect(this.props.data);
    },

    render: function() {
        // console.log("RepositoryIndexItem.render", this.props);
        
        var children;

        if (this.props.data.children) {
            children = (
                <RepositoryIndexList children={this.props.data.children} level={this.props.level + 1} onSelect={this.props.onSelect} />
            );
        }

        var className = "nav-level-" + this.props.level;
        if (this.props.data.selected) className += " active";
        if (this.props.data.visible == false) className += " hidden";

        return (
            <li className={className}>
                <a href={this.props.data.path} onClick={this._onClick}>{this.props.data.name}</a>
                {children}
            </li>
        );
    }
});

var RepositoryIndexList = React.createClass({

    render: function() {
        // console.log("RepositoryIndexList.render", this.props);

        var items = this.props.children.map(function(item, index) {
            return (
              <RepositoryIndexItem data={item} level={this.props.level} onSelect={this.props.onSelect} />
            );
        }, this);

        return (
            <ul className="nav">
                {items}
            </ul>
        );
    }
});

var RepositoryIndexSearch = React.createClass({

    _onChange: function(e) {
        this.props.onSearch(e.target.value);
    },

    render: function() {
        // console.log("RepositoryIndexSearch.render", this.props);

        return (
            <div className="navbar-form sidebar-search">
                <form className="form-inline" role="search">
                    <div className="input-group">
                        <input type="text" onChange={this._onChange} className="form-control" value={this.props.value} placeholder="Search for..." />
                        {/*<span className="input-group-btn">
                            <button className="btn btn-default" type="button">Go!</button>
                        </span>*/}
                    </div>
                </form>
            </div>
        );
    }
});

var RepositoryIndex = React.createClass({
    // mixins: [Navigation],

    contextTypes: {
        router: React.PropTypes.func.isRequired
    },

    getState: function() {
      return {
        selectedRepositoryId: RepoStore.getSelectedRepositoryId(),
        selectedRepositoryBranchName: RepoStore.getSelectedRepositoryBranchName(),
        index: RepoStore.getSelectedRepositoryIndex()
      };
    },

    getInitialState: function() {
        return this.getState();
    },

    componentDidMount: function() {
        RepoStore.addChangeListener(this._onChange);

        // console.log("binding hotkeys for index view");
        $(document).bind("keydown", "left", this.navigatePrevious);
        $(document).bind("keydown", "right", this.navigateNext);
    },

    componentWillUnmount: function() {
        RepoStore.removeChangeListener(this._onChange);

        // console.log("unbinding hotkeys for index view");
        $(document).unbind("keydown", this.navigatePrevious);
        $(document).unbind("keydown", this.navigateNext);
    },

    _onChange: function() {
        this.setState(this.getState());
    },

    _onSelectItem: function(item) {
        if (item.path) {

            this.context.router.transitionTo('repo', {
                repoId: this.state.selectedRepositoryId, 
                branchName: this.state.selectedRepositoryBranchName, 
                splat: item.path
            });

            AppActions.selectRepositoryIndexItem(item.path);
        }
    },

    _onSearchItem: function(term) {
        AppActions.searchRepositoryIndexItem(term);
    },

    navigatePrevious: function (e) {
        e.preventDefault();
        AppActions.selectSiblingRepositoryIndexItem("previous");
    },

    navigateNext: function (e) {
        e.preventDefault();
        AppActions.selectSiblingRepositoryIndexItem("next");
    },

    render: function() {
        // console.log("RepositoryIndex.render", this.state, this.props);

        if (this.state.index && this.state.index.children) {
            return (
                <div className="col-md-2 sidebar navbar-default" role="navigation">
                    <RepositoryIndexSearch value={this.state.index.searchTerm} onSearch={this._onSearchItem} />
                    <RepositoryIndexList children={this.state.index.children} level={1} onSelect={this._onSelectItem} />
                </div>
            );
        }

        return null;
    }
});

module.exports = RepositoryIndex;