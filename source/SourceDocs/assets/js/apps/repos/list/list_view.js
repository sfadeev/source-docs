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


    Module.RepoIndexListView = Marionette.CompositeView.extend({
        getTemplate: function() {
            return this.model.isRoot ? "#repo-index-template" : "#repo-index-link";
        },
        tagName: "li",
        childView: Module.RepoIndexTreeView,
        childViewContainer: "ul",

        modelEvents: {
            "change": "render"
        },

        initialize: function() {
            var children = this.model.get("children");
            if (children) {
                this.collection = new App.Entities.RepoIndexItemCollection(children);
            }
        },

        /*appendHtml: function(cv, iv) {
            cv.$("ul:first").append(iv.el);
        },*/

        onRender: function() {
            if (_.isUndefined(this.collection)) {
                this.$("ul:first").remove();
            }
        }
    });

    Module.RepoIndexView = Module.RepoIndexListView.extend({
        // tagName: "div"
        /*events: {
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
        }*/
    });
});