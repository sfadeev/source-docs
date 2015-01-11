App.module("Repos.List", function (Module, App, Backbone, Marionette, $, _) {

    Module.Controller = {
        listRepos: function() {
            App.request("Entities:loadRepos").done(function (repos) {

                Module.repos = repos;

                var view = new Module.RepoListView({ model: repos });

                view.on("childview:navigate", function(childView, model) {
                    App.commands.execute("set:active:repo", model.get("id"));
                });

                App.reposRegion.show(view);
                App.commands.execute("set:active:repo", Module.selectedRpoId);
            });
        },

        selectRepo: function(id) {

            Module.selectedRpoId = id;

            if (Module.repos) {

                var items = Module.repos.get("items");

                var repo = items.find(function(x) { return x.get("id") === id; });

                if (repo === undefined && items.length > 0) {
                    repo = items.at(0);
                }

                if (repo) {
                    repo.select();
                    Module.repos.set("title", repo.get("id"));
                }
            }
        },

        listNodes: function () {

            if (Module.repos) {

                var items = Module.repos.get("items");

                if (items.selected) {

                    var view = new Module.NodeListView({ model: items.selected });

                    view.on("childview:navigate", function(childView, model) {
                        App.commands.execute("set:active:node", model.get("name"));
                    });

                    App.nodesRegion.show(view);
                    App.commands.execute("set:active:node", Module.selectedNodeName);
                }
            }
        },

        selectNode: function(name) {

            Module.selectedNodeName = name;

            if (Module.repos) {
                var repo = Module.repos.get("items").selected;

                var nodes = repo.get("nodes");

                var node = nodes.find(function (x) { return x.get("name") === name; });

                if (node === undefined && nodes.length > 0) {
                    node = nodes.at(0);
                }

                if (node) {
                    node.select();
                    repo.set("title", node.get("name"));

                    App.commands.execute("Repos:showRepoIndex", repo.get("id"), node.get("name"));
                }
            }
        },

        showRepoIndex: function (repoId, nodeName) {
            App.request("Entities:loadRepoIndex", repoId, nodeName).done(function (index) {

                Module.index = index;
                Module.index.isRoot = true;

                var view = new Module.RepoIndexView({ model: index, tagName: "div" });

                /*view.on("childview:navigate", function (childView, model) {
                    App.commands.execute("set:active:repo", model.get("id"));
                });*/

                App.repoIndexRegion.show(view);

                // App.commands.execute("set:active:repo", Module.selectedRpoId);
            });
        }

    };
});