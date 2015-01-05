App.module("FilesApp.List", function(Module, App, Backbone, Marionette, $, _) {
    Module.HeaderView = Marionette.ItemView.extend({
        template: "#files-link",
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

    Module.HeaderListView = Marionette.CompositeView.extend({
        template: "#files-template",
        tagName: "li",
        className: "dropdown",
        childView: Module.HeaderView,
        childViewContainer: "ul.files",

        events: {
            "click a.brand": "brandClicked"
        },

        brandClicked: function(e) {
            e.preventDefault();
            this.trigger("brand:clicked");
        }
    });
});