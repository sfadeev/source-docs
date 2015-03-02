var RepositoryIndexItem = React.createClass({
    handleClick: function(e) {
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
                <a href={this.props.data.attributes.path} onClick={this.handleClick}>
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
        var indexItems = this.props.children.map(function(item, index) {
            return (
              <RepositoryIndexItem data={item} />
            );
        });
        return (
            <ul className="nav">
                {indexItems}
            </ul>
        );
    }
});

App.renderRepositoryIndex = function(model, elementId) {
    React.render(
      <RepositoryIndexList children={model.get("children")} />, document.getElementById(elementId)
    );
}
