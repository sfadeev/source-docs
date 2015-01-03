Dokka.module("FilesApp", function(Files, Dokka, Backbone, Marionette, $, _) {
    var API = {
        listFiles: function() {
            Files.List.Controller.listFiles();
        }
    };

    Dokka.commands.setHandler("set:active:file", function (name) {
        alert(name);
        Dokka.FilesApp.List.Controller.setActiveFile(name);
    });

    Dokka.on("contacts:list", function () {
        Dokka.navigate("contacts");
        API.listFiles();
    });

    Files.on("start", function() {
        API.listFiles();
    });
});