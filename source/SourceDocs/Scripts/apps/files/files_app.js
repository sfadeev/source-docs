App.module("FilesApp", function(Files, App, Backbone, Marionette, $, _) {
    var API = {
        listFiles: function() {
            Files.List.Controller.listFiles();
        }
    };

    App.commands.setHandler("set:active:file", function (name) {
        alert(name);
        App.FilesApp.List.Controller.setActiveFile(name);
    });

    App.on("contacts:list", function () {
        App.navigate("contacts");
        API.listFiles();
    });

    Files.on("start", function() {
        API.listFiles();
    });
});