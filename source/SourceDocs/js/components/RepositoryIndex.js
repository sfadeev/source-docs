/** @jsx React.DOM */
var React = require('react');

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
                <a href={this.props.data.path} onClick={this._onClick}>
                    {this.props.data.name}
                </a>
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

var RepositoryIndex = React.createClass({

    getState: function() {
      return {
        index: RepoStore.getSelectedRepositoryIndex()
      };
    },

    getInitialState: function() {
        return this.getState();
    },

    componentDidMount: function() {
        RepoStore.addChangeListener(this._onChange);
    },

    componentWillUnmount: function() {
        RepoStore.removeChangeListener(this._onChange);
    },

    _onChange: function() {
        this.setState(this.getState());
    },

    _onSelectItem: function(item) {
        AppActions.selectRepositoryIndexItem(item.path);
    },

    render: function() {
        // console.log("RepositoryIndex.render", this.state);

        if (this.state.index && this.state.index.children) {
            return (
              <RepositoryIndexList children={this.state.index.children} level={1} onSelect={this._onSelectItem} />
            );
        }

        return null;
    }
});

module.exports = RepositoryIndex;