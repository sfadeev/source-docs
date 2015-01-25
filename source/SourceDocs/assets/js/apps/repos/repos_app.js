App.module("Repos", function(Module, App, Backbone, Marionette, $, _) {
    var API = {
        listRepos: function() {
            Module.List.Controller.listRepos();
        },
        selectRepo: function(id) {
            Module.List.Controller.selectRepo(id);
            Module.List.Controller.listNodes();
        },
        selectNode: function(name) {
            Module.List.Controller.selectNode(name);
        },
        showRepoIndex: function(repoId, nodeName) {
            // App.navigate("repo/" + repoId + "/" + nodeName);
            Module.List.Controller.showRepoIndex(repoId, nodeName);
        },
        selectPath: function(path) {
            Module.List.Controller.selectPath(path);
        }
    };

    App.commands.setHandler("Repos:listRepos", function() {
        API.listRepos();
    });

    App.commands.setHandler("Repos:selectRepo", function(id) {
        API.selectRepo(id);
    });

    App.commands.setHandler("Repos:selectNode", function(name) {
        API.selectNode(name);
    });

    App.commands.setHandler("Repos:showRepoIndex", function(repoId, nodeName) {
        API.showRepoIndex(repoId, nodeName);
    });

    App.commands.setHandler("Repos:selectPath", function(path) {
        API.selectPath(path);
    });

    /*App.on("contacts:list", function () {
        App.navigate("contacts");
        API.listFiles();
    });*/

    Module.on("start", function() {
        // API.listRepos();
    });
});