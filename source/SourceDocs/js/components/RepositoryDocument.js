/** @jsx React.DOM */
var React = require('react');

var AppActions = require('../actions/AppActions');
var RepoStore = require('../stores/RepoStore');

var RepositoryBreadcrumbItem = React.createClass({

    _onClick: function(e) {
        e.preventDefault();

        this.props.onSelect(this.props.data);
    },

    render: function() {
        // console.log("RepositoryBreadcrumbItem.render", this.props);

        var item;

        if (this.props.active) {
            return (
                <li className="active">{this.props.data.name}</li>
            );
        }
        
        if (this.props.index != 0 && this.props.data.path) {
            return (
                <li><a href={this.props.data.path} onClick={this._onClick}>{this.props.data.name}</a></li>
            );
        }

        return (
            <li>{this.props.data.name}</li>
        );
    }
});

var RepositoryBreadcrumb = React.createClass({

    render: function() {
        // console.log("RepositoryBreadcrumb.render", this.props);

        var lastIndex = this.props.data.length - 1;

        var items = this.props.data.map(function(item, index) {
            return (
                <RepositoryBreadcrumbItem data={item} onSelect={this.props.onSelect} index={index} active={(index == lastIndex)} />
            );
        }, this);

        return (
            <ol className="breadcrumb">
                {items}
            </ol>
        );
    }
});

var RepositoryDocumentContent = React.createClass({

    /*componentDidMount: function () {
        this.highlight();
    },*/

    componentDidUpdate: function () {
        this.highlight();
    },

    highlight: function () {
        var nodes = this.getDOMNode().querySelectorAll("pre code");
        for (var i = 0; i < nodes.length; i = i + 1) {
            hljs.highlightBlock(nodes[i]);
        }
    },
    
    render: function() {
        // console.log("RepositoryDocumentContent.render", this.props);

        return (
            <div className="content" dangerouslySetInnerHTML={{__html: this.props.data ? this.props.data.content : null}} />
        );
    }
});

var RepositoryDocument = React.createClass({

    getState: function() {
        return {
            breadcrumb: RepoStore.getSelectedRepositoryBreadcrumb(),
            document: RepoStore.getSelectedRepositoryDocument()
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
        // console.log("RepositoryDocument.render", this.state);

        return (
            <div className="col-md-9">
                <br />
                <RepositoryBreadcrumb data={this.state.breadcrumb} onSelect={this._onSelectItem} />
                <RepositoryDocumentContent data={this.state.document} />
            </div>
        );
    }
});

module.exports = RepositoryDocument;