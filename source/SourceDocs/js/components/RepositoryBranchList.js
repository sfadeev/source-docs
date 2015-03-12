/** @jsx React.DOM */
var React = require('react');

var RepositoryBranchItem = React.createClass({
    
    _onClick: function(e) {
        e.preventDefault();

        this.props.onSelect(this.props.data);
    },

    render: function() {

        var className = this.props.data.selected ? "active" : "";

        return (
            <li className={className}>
                <a href={"#" + this.props.data.name} onClick={this._onClick}>
                    {this.props.data.name}
                </a>
            </li>
        );
    }
});

var RepositoryBranchList = React.createClass({

    render: function() {

        if (this.props.data) {
            var items = this.props.data.nodes.map(function(item, index) {
                return (
                  <RepositoryBranchItem data={item} onSelect={this.props.onSelect} />
                );
            }, this);
            
            return (
                <li className="dropdown">
                    <a href="#" className="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
                        {this.props.data.nodes.title} 
                        <span className="caret"></span>
                    </a>
                    <ul className="dropdown-menu" role="menu">
                        {items}
                    </ul>
                </li>
            );
        }

        return null;
    },

});

module.exports = RepositoryBranchList;