App.module("Repos.List", function (Module, App, Backbone, Marionette, $, _) {
    Module.Controller = {
        listRepos: function () {
            App.request("header:repos").done(function (repos) {

                var headers = new Module.RepoListView({ collection: repos });

                /*headers.on("brand:clicked", function() {
                    App.trigger("contacts:list");
                });*/

                headers.on("childview:navigate", function(childView, model) {
                    var trigger = model.get("navigationTrigger");
                    App.trigger(trigger);
                });

                App.reposRegion.show(headers);
            });
        },

        /*setActiveFile: function(headerUrl) {
            var links = App.request("header:entities");
            var headerToSelect = links.find(function(header) { return header.get("url") === headerUrl; });
            headerToSelect.select();
            links.trigger("reset");
        }*/
    };
});