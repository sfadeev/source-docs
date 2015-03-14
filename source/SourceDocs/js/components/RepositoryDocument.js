/** @jsx React.DOM */
var React = require('react');

var AppActions = require('../actions/AppActions');
var RepoStore = require('../stores/RepoStore');

var RepositoryDocument = React.createClass({

    getState: function() {
      return {
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
        // console.log("RepositoryDocument.render", this.state);

        return (
            <div className="content"
                dangerouslySetInnerHTML={{__html: this.state.document ? this.state.document.content : null}} />
        );
    }
});

module.exports = RepositoryDocument;