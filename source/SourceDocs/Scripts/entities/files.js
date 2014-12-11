Dokka.module("Entities", function (Entities, Dokka, Backbone, Marionette, $, _) {
    Entities.File = Backbone.Model.extend({
        initialize: function () {
            var selectable = new Backbone.Picky.Selectable(this);
            _.extend(this, selectable);
        }
    });

    Entities.FileCollection = Backbone.Collection.extend({
        model: Entities.File,

        initialize: function () {
            var singleSelect = new Backbone.Picky.SingleSelect(this);
            _.extend(this, singleSelect);
        }
    });

    var initializeFiles = function () {
        Entities.files = new Entities.FileCollection([
          { name: "Contacts", url: "contacts", navigationTrigger: "contacts:list" },
          { name: "About", url: "about", navigationTrigger: "about:show" }
        ]);
    };

    var API = {
        getFiles: function () {
            if (Entities.files === undefined) {
                initializeFiles();
            }
            return Entities.files;
        }
    };

    Dokka.reqres.setHandler("header:entities", function () {
        return API.getFiles();
    });
});