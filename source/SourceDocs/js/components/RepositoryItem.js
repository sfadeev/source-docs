/** @jsx React.DOM */
var React = require('react');
var AppActions = require('../actions/AppActions');

var RepositoryItem = React.createClass({
    _onClick: function(e) {
        e.preventDefault();

        AppActions.selectRepository(this.props.data.id);
        // App.commands.execute("Repos:selectRepo", this.props.data.attributes.id);
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

module.exports = RepositoryItem;