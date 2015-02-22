App.module("Repos.List", function(Module, App, Backbone, Marionette, $, _) {

    Module.Controller = {
        listRepos: function() {
            App.request("Entities:loadRepos").done(function(repos) {

                Module.repos = repos;

                console.log("rendering repos list", repos);

                var view = new Module.RepoListView({ model: repos });

                view.on("childview:navigate", function(childView, model) {
                    App.commands.execute("Repos:selectRepo", model.get("id"));
                });

                App.reposRegion.show(view);

                App.commands.execute("Repos:selectRepo", Module.selectedRepoId);
            });
        },

        selectRepo: function(id) {

            Module.Controller.emptyNodeRegions();

            Module.selectedRepoId = id;

            if (Module.repos) {

                var items = Module.repos.get("items");

                var repo = items.find(function(x) { return x.get("id") === id; });

                if (repo === undefined /*&& items.length > 0*/) {
                    repo = items.find(function (x) { return x.get("url") != undefined; });
                    /*repo = items.at(0);
                    Module.selectedRepoId = repo.get("id");*/
                }

                if (repo) {
                    Module.selectedRepoId = repo.get("id");
                    repo.select();
                    Module.repos.set("title", repo.get("id"));
                } else {
                    Module.selectedRepoId = null;
                }
            }
        },

        listNodes: function() {

            if (Module.repos) {

                var items = Module.repos.get("items");

                if (items.selected) {

                    console.log("rendering nodes of repo", Module.selectedRepoId);

                    var view = new Module.NodeListView({ model: items.selected });

                    view.on("childview:navigate", function(childView, model) {
                        App.commands.execute("Repos:selectNode", model.get("name"));
                    });

                    App.nodesRegion.show(view);

                    App.commands.execute("Repos:selectNode", Module.selectedNodeName);
                }
            }
        },

        // todo: do not empty region till sure it should be empty to prevent flickering
        emptyNodeRegions: function () {

            App.repoIndexRegion.empty();
            App.breadcrumbRegion.empty();
            App.mainRegion.empty();
            App.pagerRegion.empty();
        },

        selectNode: function(name) {

            Module.Controller.emptyNodeRegions();

            Module.selectedNodeName = name;

            if (Module.repos) {
                var repo = Module.repos.get("items").selected;

                var nodes = repo.get("nodes");

                var node = nodes.find(function(x) { return x.get("name") === name; });

                if (node === undefined && nodes.length > 0) {
                    node = nodes.at(0);
                    Module.selectedNodeName = node.get("name");
                }

                if (node) {
                    node.select();
                    repo.set("title", node.get("name"));

                    App.commands.execute("Repos:showRepoIndex", repo.get("id"), node.get("name"));
                }
            }
        },

        showRepoIndex: function(repoId, nodeName) {
            App.request("Entities:loadRepoIndex", repoId, nodeName).done(function(index) {

                Module.index = index;
                Module.index.set("level", 0);

                var prevChildModel;
                var childrenList = new App.Entities.RepoIndexItemCollection();
                var buildListRecursive = function(level, parentModel, children) {
                    if (children) {
                        for (var i = 0; i < children.length; i++) {

                            var child = children[i];

                            var childModel = childrenList.add(child); // convert object to model

                            children[i] = childModel;
                            childModel.set("level", level);
                            childModel.set("sibling:parent", parentModel);

                            if (prevChildModel) {
                                childModel.set("sibling:previous", prevChildModel);
                                prevChildModel.set("sibling:next", childModel);
                            }
                            prevChildModel = childModel;

                            buildListRecursive(level + 1, childModel, child.children);
                        }
                    }
                };

                buildListRecursive(1, index, index.get("children"));

                Module.index.set("childrenList", childrenList);

                var selectedModel;
                if (Module.selectedPath) {
                    selectedModel = childrenList.find(function (x) { return x.get("path") === Module.selectedPath; });
                    // Module.selectedPath = null;
                }
                if (selectedModel === undefined && childrenList.length > 0) {
                    selectedModel = childrenList.at(0);
                }
                if (selectedModel) {
                    Module.Controller.selectIndexItem(selectedModel);
                }

                Module.Controller.renderIndex();

            });
        },

        renderIndex: function (searchTerm) {

            Module.index.searchTerm = searchTerm;

            console.log("rendering index", { selectedRepoId: Module.selectedRepoId, selectedNodeName: Module.selectedNodeName, searchTerm: Module.index.searchTerm });

            App.repoIndexRegion.show(new Module.RepoIndexView({ model: Module.index }));

        },

        selectIndexItem: function(model) {

            if (Module.selectedIndexItem) {
                Module.selectedIndexItem.deselect();
            }

            Module.selectedIndexItem = model;
            Module.selectedPath = model.get("path");

            model.select();

            App.navigate(Module.selectedRepoId + "/" + Module.selectedNodeName + "/~/" + Module.selectedPath);

            // build and show breadcrumb
            var breadcrumbList = new App.Entities.RepoIndexItemCollection();
            var breadcrumbItem = model;
            do {
                breadcrumbList.add(breadcrumbItem, { at: 0 });
                breadcrumbItem = breadcrumbItem.get("sibling:parent");

            } while (breadcrumbItem)

            App.breadcrumbRegion.show(
                new Module.RepoBreadcrumbListView({ model: model, collection: breadcrumbList })
                .on("childview:navigate", function(e) {
                    Module.Controller.selectIndexItem(e.model);
                })
            );

            // build and show pager
            App.pagerRegion.show(
                new Module.RepoPagerView({ model: model })
                .on({
                    "navigate:previous": function() { Module.Controller.selectSiblingIndexItem("previous"); },
                    "navigate:next": function() { Module.Controller.selectSiblingIndexItem("next"); }
                })
            );

            // load and show doc
            App.request("Entities:loadRepoDoc", Module.selectedRepoId, Module.selectedNodeName, model.get("path")).done(function(doc) {
                console.log("rendering document", doc);

                App.mainRegion.show(new Module.RepoDocView({ model: doc }));

                // todo: create plugin and highlight on document show
                if (hljs) {
                    App.mainRegion.$el.find("pre code").each(function (i, block) {
                        hljs.highlightBlock(block);
                    });
                }

            });
        },

        selectSiblingIndexItem: function (direction) {
            if (Module.selectedIndexItem) {
                var sibling = Module.selectedIndexItem.get("sibling:" + direction);
                if (sibling) Module.Controller.selectIndexItem(sibling);
            }
        },

        selectPath: function(path) {
            Module.selectedPath = path;
        },

        search: function (searchTerm) {
            Module.Controller.renderIndex(searchTerm);
        }

    };

    // todo: should be in repos app?
    Module.on("Repos.List:selectIndexItem", Module.Controller.selectIndexItem);
    Module.on("Repos.List:selectSiblingIndexItem", Module.Controller.selectSiblingIndexItem);

});