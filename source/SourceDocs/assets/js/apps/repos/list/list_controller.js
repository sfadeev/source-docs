App.module("Repos.List", function (Module, App, Backbone, Marionette, $, _) {

    Module.Controller = {
        listRepos: function () {
            App.request("header:repos").done(function (repos) {

                Module.repos = repos;

                var view = new Module.RepoListView({ model: repos });

                /*view.on("brand:clicked", function() {
                    App.trigger("contacts:list");
                });*/

                view.on("childview:navigate", function (childView, model) {
                    App.commands.execute("set:active:repo", model.get("id"));

                    /*var trigger = model.get("navigationTrigger");
                    App.trigger(trigger);*/
                });

                App.reposRegion.show(view);
                App.commands.execute("set:active:repo");
            });
        },

        selectRepo: function (id) {
            if (Module.repos) {

                var items = Module.repos.get("items");

                var repo = items.find(function (x) { return x.get("id") === id; });

                if (repo === undefined && items.length > 0) {
                    repo = items.at(0);
                }

                if (repo) {
                    repo.select();
                    App.navigate(repo.get("id"));

                    Module.repos.set("title", repo.get("id"));

                    // Module.model.trigger("change");
                    // Module.repos.trigger("reset");
                }
            }
        }
    };
});