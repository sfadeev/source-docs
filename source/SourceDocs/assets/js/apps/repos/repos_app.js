App.module("Repos", function(Module, App, Backbone, Marionette, $, _) {
    var API = {
        listRepos: function () {
            Module.List.Controller.listRepos();
        },
        selectRepo: function (id) {
            Module.List.Controller.selectRepo(id);
        }
    };

    App.commands.setHandler("set:active:repo", function (id) {
        API.selectRepo(id);
    });

    /*App.on("contacts:list", function () {
        App.navigate("contacts");
        API.listFiles();
    });*/

    Module.on("start", function() {
        API.listRepos();
    });
});