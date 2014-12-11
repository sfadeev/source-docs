﻿Dokka.module("FilesApp.List", function(List, Dokka, Backbone, Marionette, $, _) {
    List.Controller = {
        listFiles: function() {
            var links = Dokka.request("header:entities");
            var headers = new List.Headers({ collection: links });

            headers.on("brand:clicked", function() {
                Dokka.trigger("contacts:list");
            });

            headers.on("childview:navigate", function(childView, model) {
                var trigger = model.get("navigationTrigger");
                Dokka.trigger(trigger);
            });

            Dokka.headerRegion.show(headers);
        },

        setActiveFile: function(headerUrl) {
            var links = Dokka.request("header:entities");
            var headerToSelect = links.find(function(header) { return header.get("url") === headerUrl; });
            headerToSelect.select();
            links.trigger("reset");
        }
    };
});