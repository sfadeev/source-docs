App.module("Entities", function (Entities, App, Backbone, Marionette, $, _) {
    Entities.File = Backbone.Model.extend({
        initialize: function () {
            var selectable = new Backbone.Picky.Selectable(this);
            _.extend(this, selectable);
        }
    });

    Entities.FileCollection = Backbone.Collection.extend({
        url: "files",
        model: Entities.File,

        initialize: function () {
            var singleSelect = new Backbone.Picky.SingleSelect(this);
            _.extend(this, singleSelect);
        }
    });

    var API = {
        getFiles: function () {
            if (Entities.files === undefined) {
                Entities.files = new Entities.FileCollection();
            }

            var defer = $.Deferred();
            Entities.files.fetch({
                success: function(data) {
                    defer.resolve(data);
                }
            });
            return defer.promise();
        }
    };

    App.reqres.setHandler("header:entities", function () {
        return API.getFiles();
    });
});
