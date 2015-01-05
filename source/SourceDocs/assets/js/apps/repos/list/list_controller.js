App.module("Repos.List", function (Module, App, Backbone, Marionette, $, _) {
    Module.Controller = {
        listRepos: function () {
            App.request("header:repos").done(function (repos) {

                Module.repos = repos;

                var view = new Module.RepoListView({ collection: repos });

                /*view.on("brand:clicked", function() {
                    App.trigger("contacts:list");
                });*/

                view.on("childview:navigate", function (childView, model) {
                    // App.navigate(model.get("id"));

                    App.commands.execute("set:active:repo", model.get("id"));

                    /*var trigger = model.get("navigationTrigger");
                    App.trigger(trigger);*/
                });

                /*var repoToSelect;
                if (this.selectedRepoId) {
                    repoToSelect = repos.find(function (repo) { return repo.get("id") === this.selectedRepoId; });
                }
                if (repoToSelect === undefined && repos.length > 0) {
                    repoToSelect = repos.at(0);
                }

                if (repoToSelect) {
                    repoToSelect.selected = true;
                    App.commands.execute("set:active:repo", repoToSelect.get("id"));
                }*/

                App.reposRegion.show(view);
                App.commands.execute("set:active:repo");
            });
        },

        selectRepo: function (id) {
            if (Module.repos) {
                var repoToSelect = Module.repos.find(function (repo) { return repo.get("id") === id; });

                if (repoToSelect === undefined && Module.repos.length > 0) {
                    repoToSelect = Module.repos.at(0);
                }

                if (repoToSelect) {
                    // todo: check why picky not extend model
                    // Module.repos.select(repoToSelect);
                    // repoToSelect.selected(); 
                    repoToSelect.selected = true;
                    Module.selectedRepoId = repoToSelect.get("id");
                    App.navigate(Module.selectedRepoId);
                    Module.repos.trigger("reset");
                }
            }
        }
    };
});