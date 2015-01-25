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
        tagName: "li",
        childView: Module.RepoIndexTreeView,
        childViewContainer: "ul",
        getTemplate: function() {
            return (this.model.get("level") === 0) ? "#repo-index-template" : "#repo-index-link";
        },

        events: {
            "click a": "navigate"
        },

        navigate: function (e) {
            e.preventDefault();
            Module.trigger("Repos.List:selectIndexItem", this.model);
        },

        modelEvents: {
            "change selected deselected": "render"
        },

        initialize: function() {
            var children = this.model.get("children");
            if (children) {
                this.collection = new App.Entities.RepoIndexItemCollection(children);
            }
        },

        // http://jsfiddle.net/hoffmanc/NH9J6/
        /*appendHtml: function(cv, iv) {
            cv.$("ul:first").append(iv.el);
        },*/

        onRender: function () {
            this.$el.addClass("nav-level-" + this.model.get("level"));

            if (this.model.selected) {
                this.$el.addClass("active");
            } else {
                this.$el.removeClass("active");
            };

            if (_.isUndefined(this.collection)) {
                this.$("ul:first").remove();
            }
        }
    });

    Module.RepoIndexView = Module.RepoIndexListView.extend({
    });

    Module.RepoDocView = Marionette.ItemView.extend({
        template: "#repo-doc-template"
    });

    Module.RepoBreadcrumbView = Marionette.ItemView.extend({
        template: "#repo-doc-breadcrumb-template",
        tagName: "nav"
    });

    Module.RepoPagerView = Marionette.ItemView.extend({
        template: "#repo-doc-pager-template",
        tagName: "nav",

        events: {
            "click li.previous a": "navigatePrev",
            "click li.next a": "navigateNext"
        },

        navigatePrev: function (e) {
            e.preventDefault();
            Module.trigger("Repos.List:selectIndexItem", this.model.get("prev"));
        },

        navigateNext: function (e) {
            e.preventDefault();
            Module.trigger("Repos.List:selectIndexItem", this.model.get("next"));
        }
    });
});