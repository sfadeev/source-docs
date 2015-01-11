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
                this.$el.addClass("active");
            };
        }
    });

    Module.RepoListView = Marionette.CompositeView.extend({
        template: "#repos-template",
        tagName: "li",
        className: "dropdown",
        childView: Module.RepoView,
        childViewContainer: "ul",

        modelEvents: {
            "change": "render"
        },

        initialize: function() {
            this.collection = this.model.get("items");
        }
    });

    Module.NodeView = Marionette.ItemView.extend({
        template: "#node-link",
        tagName: "li",

        events: {
            "click a": "navigate"
        },

        navigate: function (e) {
            e.preventDefault();
            this.trigger("navigate", this.model);
        },

        onRender: function () {
            if (this.model.selected) {
                this.$el.addClass("active");
            };
        }
    });

    Module.NodeListView = Marionette.CompositeView.extend({
        template: "#nodes-template",
        tagName: "li",
        className: "dropdown",
        childView: Module.NodeView,
        childViewContainer: "ul",

        modelEvents: {
            "change": "render"
        },

        initialize: function() {
            this.collection = this.model.get("nodes");
        }
    });

    Module.RepoIndexView = Marionette.ItemView.extend({
        template: "#repo-index-link",
        tagName: "li",

        events: {
            "click a": "navigate"
        },

        navigate: function (e) {
            e.preventDefault();
            this.trigger("navigate", this.model);
        },

        onRender: function () {
            if (this.model.selected) {
                this.$el.addClass("active");
            };
        }
    });

    Module.RepoIndexListView = Marionette.CompositeView.extend({
        template: "#repo-index-template",
        tagName: "li",
        childView: Module.RepoIndexListView,
        childViewContainer: "ul",

        modelEvents: {
            "change": "render"
        },

        initialize: function () {
            this.collection = new App.Entities.RepoIndexItemCollection(this.model.get("children"));
        }
    });
});