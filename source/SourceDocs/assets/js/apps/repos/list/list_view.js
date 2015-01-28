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
        template: "#repo-index-link",
        tagName: "li",
        // childView: Module.RepoIndexListView,
        childViewContainer: "ul",

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

    Module.RepoIndexView = Marionette.CompositeView.extend({
        template: "#repo-index-template",
        tagName: "div",
        childViewContainer: "ul",
        childView: Module.RepoIndexListView,

        initialize: function () {
            var children = this.model.get("children");
            if (children) {
                this.collection = new App.Entities.RepoIndexItemCollection(children);
            }
        },

        onAttach: function () {
            console.log("binding hotkeys for index view");

            $(document).bind("keydown", "left", this.navigatePrevious);
            $(document).bind("keydown", "right", this.navigateNext);
        },

        onDestroy: function () {
            console.log("unbinding hotkeys for index view");

            $(document).unbind("keydown", this.navigatePrevious);
            $(document).unbind("keydown", this.navigateNext);
        },

        navigatePrevious: function (e) {
            e.preventDefault();
            Module.trigger("Repos.List:selectSiblingIndexItem", "previous");
        },

        navigateNext: function (e) {
            e.preventDefault();
            Module.trigger("Repos.List:selectSiblingIndexItem", "next");
        }
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

        ui: {
            "previous": "li.previous a",
            "next": "li.next a"
        },

        triggers: {
            "click @ui.previous": "navigate:previous",
            "click @ui.next": "navigate:next"
        }
    });
});