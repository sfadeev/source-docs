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
        var contentNode = React.findDOMNode(this.refs.content);
        if (contentNode) {
            var nodes = contentNode.querySelectorAll("pre code");
            for (var i = 0; i < nodes.length; i = i + 1) {
                hljs.highlightBlock(nodes[i]);
            }
        }
    },

    render: function() {
        // console.log("RepositoryDocumentContent.render", this.props);

        if (this.props.data && this.props.data.content) {

            var content = this.props.data.content;

            function createMarkup() { return { __html: content }; };

            return (
                <div ref="content" className="content" dangerouslySetInnerHTML={ createMarkup() } />
            );
        }

        return null;
    }
});

var RepositoryDocumentPager = React.createClass({

    _onClickPrevious: function(e) {
        e.preventDefault();
        this.props.onSelect(this.props.data.previous);
    },
    _onClickNext: function(e) {
        e.preventDefault();
        this.props.onSelect(this.props.data.next);
    },

    render: function() {
        // console.log("RepositoryDocumentPager.render", this.props);

        if (this.props.data) {
            var previous, next;

            if (this.props.data.previous) {
                previous = (
                    <li className="previous">
                        <a href="#" onClick={this._onClickPrevious}><span aria-hidden="true">&larr;</span> {this.props.data.previous.name}</a>
                    </li>
                );
            }
            if (this.props.data.next) {
                next = (
                    <li className="next">
                        <a href="#" onClick={this._onClickNext}>{this.props.data.next.name} <span aria-hidden="true">&rarr;</span></a>
                    </li>
                );
            }

            return (
                <nav>
                  <ul className="pager">
                    {previous}{next}
                  </ul>
                </nav>
            );
        }

        return null;
    }
});

var RepositoryDocument = React.createClass({

    getState: function() {
        return {
            breadcrumb: RepoStore.getSelectedRepositoryBreadcrumb(),
            document: RepoStore.getSelectedRepositoryDocument(),
            pager: RepoStore.getSelectedRepositoryPager()
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
        console.log("RepositoryDocument.render", this.state);

        return (
            <div className="col-md-10">
                <br />
                <RepositoryBreadcrumb data={this.state.breadcrumb} onSelect={this._onSelectItem} />
                <RepositoryDocumentPager data={this.state.pager} onSelect={this._onSelectItem} />
                <RepositoryDocumentContent data={this.state.document} />
                <RepositoryDocumentPager data={this.state.pager} onSelect={this._onSelectItem} />
            </div>
        );
    }
});

module.exports = RepositoryDocument;