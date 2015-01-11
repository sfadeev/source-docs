App.module("Repos", function(Module, App, Backbone, Marionette, $, _) {
    var API = {
        listRepos: function () {
            Module.List.Controller.listRepos();
        },
        selectRepo: function (id) {
            Module.List.Controller.selectRepo(id);
            Module.List.Controller.listNodes();
        },
        selectNode: function (name) {
            Module.List.Controller.selectNode(name);
        },
        showRepoIndex: function (repoId, nodeName) {
            App.navigate("repo/" + repoId + "/" + nodeName);
            Module.List.Controller.showRepoIndex(repoId, nodeName);
        }
    };

    App.commands.setHandler("set:active:repo", function (id) {
        API.selectRepo(id);
    });

    App.commands.setHandler("set:active:node", function (name) {
        API.selectNode(name);
    });

    App.commands.setHandler("Repos:showRepoIndex", function (repoId, nodeName) {
        API.showRepoIndex(repoId, nodeName);

    });

    /*App.on("contacts:list", function () {
        App.navigate("contacts");
        API.listFiles();
    });*/

    Module.on("start", function() {
        API.listRepos();
    });
});