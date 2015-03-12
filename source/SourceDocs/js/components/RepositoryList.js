/** @jsx React.DOM */
var React = require('react');

var RepositoryItem = React.createClass({
    
    _onClick: function(e) {
        e.preventDefault();

        this.props.onSelect(this.props.data);
    },

    render: function() {

        if (this.props.data.url) {
            var className = this.props.data.selected ? "active" : "";

            return (
                <li className={className}>
                    <a href={this.props.data.id} onClick={this._onClick}>
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

    render: function() {

        var items = this.props.data.map(function(item, index) {
            return (
              <RepositoryItem data={item} onSelect={this.props.onSelect} />
            );
        }, this);
        
        return (
            <li className="dropdown">
                <a href="#" className="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
                    {this.props.data.title} 
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