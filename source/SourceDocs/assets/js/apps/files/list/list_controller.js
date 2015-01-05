App.module("FilesApp.List", function(Module, App, Backbone, Marionette, $, _) {
    Module.Controller = {
        listFiles: function() {
            App.request("header:entities").done(function(files) {

                var headers = new Module.HeaderListView({ collection: files });

                headers.on("brand:clicked", function() {
                    App.trigger("contacts:list");
                });

                headers.on("childview:navigate", function(childView, model) {
                    var trigger = model.get("navigationTrigger");
                    App.trigger(trigger);
                });

                App.filesRegion.show(headers);
            });
        },

        setActiveFile: function(headerUrl) {
            var links = App.request("header:entities");
            var headerToSelect = links.find(function(header) { return header.get("url") === headerUrl; });
            headerToSelect.select();
            links.trigger("reset");
        }
    };
});