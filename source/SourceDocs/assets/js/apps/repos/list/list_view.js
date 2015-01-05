App.module("Repos.List", function(Module, App, Backbone, Marionette, $, _) {
    Module.RepoView = Marionette.ItemView.extend({
        template: "#repos-link",
        tagName: "li",

        events: {
            "click a": "navigate"
        },

        navigate: function(e) {
            e.preventDefault();
            this.trigger("navigate", this.model);
        },

        onRender: function() {
            if (this.model.selected) {
                // add class so Bootstrap will highlight the active entry in the navbar
                this.$el.addClass("active");
            };
        }
    });

    Module.RepoListView = Marionette.CompositeView.extend({
        template: "#repos-template",
        tagName: "li",
        className: "dropdown",
        childView: Module.RepoView,
        childViewContainer: "ul.repos",

        /*events: {
            "click a.brand": "brandClicked"
        },

        brandClicked: function(e) {
            e.preventDefault();
            this.trigger("brand:clicked");
        }*/
    });
});