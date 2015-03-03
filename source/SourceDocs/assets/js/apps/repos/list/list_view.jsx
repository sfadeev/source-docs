var RepositoryList = React.createClass({
    render: function() {
        // console.log("RepositoryList.render", this.props);

        var items = this.props.data.attributes.items.map(function(item, index) {
            return (
              <RepositoryItem data={item} />
            );
        });
        
        return (
            <li className="dropdown">
                <a href="#" className="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
                    {this.props.data.attributes.title} 
                    <span className="caret"></span>
                </a>
                <ul className="dropdown-menu" role="menu">
                    {items}
                </ul>
            </li>
        );
    }
});

var RepositoryItem = React.createClass({
    _onClick: function(e) {
        e.preventDefault();

        App.commands.execute("Repos:selectRepo", this.props.data.attributes.id);
    },
    render: function() {
        // console.log("RepositoryItem.render", this.props);

        if (this.props.data.attributes.url) {
            var className = this.props.data.selected ? "active" : "";

            return (
                <li className={className}>
                    <a href={this.props.data.attributes.url} onClick={this._onClick}>
                        {this.props.data.attributes.id}
                    </a>
                </li>
            );
        }
        else {
            return (
                <li className="dropdown-header">
                    {this.props.data.attributes.id}
                </li>
            );
        }
    }
});

var RepositoryIndexItem = React.createClass({
    _onClick: function(e) {
        e.preventDefault();

        App.Repos.List.trigger("Repos.List:selectIndexItem", this.props.data);
    },
    render: function() {
        // console.log("RepositoryIndexItem.render", this.props);
        
        var children;

        if (this.props.data.attributes.children) {
            children = (
                <RepositoryIndexList children={this.props.data.attributes.children} />
            );
        }

        var className = "nav-level-" + this.props.data.attributes.level;
        if (this.props.data.selected) className += " active";
        if (this.props.data.attributes.visible == false) className += " hidden";

        return (
            <li className={className}>
                <a href={this.props.data.attributes.path} onClick={this._onClick}>
                    {this.props.data.attributes.name}
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
              <RepositoryIndexItem data={item} />
            );
        });

        return (
            <ul className="nav">
                {items}
            </ul>
        );
    }
});

App.renderRepository = function(model, elementId) {
    React.render(
      <RepositoryList data={model} />, document.getElementById(elementId)
    );
}

App.renderRepositoryIndex = function(model, elementId) {
    React.render(
      <RepositoryIndexList children={model.get("children")} />, document.getElementById(elementId)
    );
}
