﻿App.module("Repos.List", function(Module, App, Backbone, Marionette, $, _) {

    Module.Controller = {
        listRepos: function() {
            App.request("Entities:loadRepos").done(function(repos) {

                Module.repos = repos;

                console.log("rendering", repos);

                var view = new Module.RepoListView({ model: repos });

                view.on("childview:navigate", function(childView, model) {
                    App.commands.execute("Repos:selectRepo", model.get("id"));
                });

                App.reposRegion.show(view);

                App.commands.execute("Repos:selectRepo", Module.selectedRepoId);
            });
        },

        selectRepo: function(id) {

            App.repoIndexRegion.empty();
            App.breadcrumbRegion.empty();
            App.mainRegion.empty();
            App.pagerRegion.empty();

            Module.selectedRepoId = id;

            if (Module.repos) {

                var items = Module.repos.get("items");

                var repo = items.find(function(x) { return x.get("id") === id; });

                if (repo === undefined && items.length > 0) {
                    repo = items.at(0);
                    Module.selectedRepoId = repo.get("id");
                }

                if (repo) {
                    repo.select();
                    Module.repos.set("title", repo.get("id"));
                }
            }
        },

        listNodes: function() {

            if (Module.repos) {

                var items = Module.repos.get("items");

                if (items.selected) {

                    console.log("rendering nodes of " + Module.selectedRepoId);

                    var view = new Module.NodeListView({ model: items.selected });

                    view.on("childview:navigate", function(childView, model) {
                        App.commands.execute("Repos:selectNode", model.get("name"));
                    });

                    App.nodesRegion.show(view);

                    App.commands.execute("Repos:selectNode", Module.selectedNodeName);
                }
            }
        },

        selectNode: function(name) {

            App.repoIndexRegion.empty();
            App.breadcrumbRegion.empty();
            App.mainRegion.empty();
            App.pagerRegion.empty();

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

                var childrenList = new App.Entities.RepoIndexItemCollection();
                var buildListRecursive = function(level, children) {
                    if (children) {
                        for (var i = 0; i < children.length; i++) {
                            var child = children[i];
                            children[i] = childrenList.add(child);
                            children[i].set("level", level);
                            buildListRecursive(level + 1, child.children);
                        }
                    }
                };

                buildListRecursive(1, index.get("children"));

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

                console.log("rendering index of " + repoId + "/" + nodeName);

                var view = new Module.RepoIndexView({ model: index, tagName: "div" });

                App.repoIndexRegion.show(view);
            });
        },

        selectIndexItem: function(model) {

            if (Module.selectedIndexItem) {
                Module.selectedIndexItem.deselect();
            }

            Module.selectedIndexItem = model;
            Module.selectedPath = model.get("path");

            model.select();

            App.navigate("repo/" + Module.selectedRepoId + "/" + Module.selectedNodeName + "/" + Module.selectedPath);

            App.breadcrumbRegion.show(new Marionette.ItemView({
                template: "#repo-doc-breadcrumb-template",
                tagName: "nav",
                model: model
            }));

            App.pagerRegion.show(new Marionette.ItemView({
                template: "#repo-doc-pager-template",
                tagName: "nav",
                model: model
            }));

            App.request("Entities:loadRepoDoc", Module.selectedRepoId, Module.selectedNodeName, model.get("path")).done(function(doc) {
                console.log("rendering", doc);

                var view = new Module.RepoDocView({ model: doc });
                App.mainRegion.show(view);

            });
        },

        selectPath: function(path) {
            Module.selectedPath = path;
        }

    };

    Module.on("Repos.List:selectIndexItem", Module.Controller.selectIndexItem);
});