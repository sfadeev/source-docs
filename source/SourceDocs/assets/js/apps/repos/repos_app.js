﻿App.module("Repos", function(Module, App, Backbone, Marionette, $, _) {
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
            // Module.List.Controller.listIndex();
        }
    };

    App.commands.setHandler("set:active:repo", function (id) {
        API.selectRepo(id);
    });

    App.commands.setHandler("set:active:node", function (name) {
        API.selectNode(name);
    });

    App.commands.setHandler("list:index", function (repoId, nodeName) {
        console.log("list:index : " + repoId + "/" + nodeName);
        App.navigate("repo/" + repoId + "/" + nodeName);
    });

    /*App.on("contacts:list", function () {
        App.navigate("contacts");
        API.listFiles();
    });*/

    Module.on("start", function() {
        API.listRepos();
    });
});