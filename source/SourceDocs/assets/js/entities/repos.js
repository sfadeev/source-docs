App.module("Entities", function (Entities, App, Backbone, Marionette, $, _) {
    Entities.Repo = Backbone.Model.extend({
        initialize: function () {
            /*var selectable = new Backbone.Picky.Selectable(this);
            _.extend(this, selectable);*/
        }
    });

    Entities.RepoCollection = Backbone.Collection.extend({
        url: App.config.api.url + "repos",
        model: Entities.Repo,
        initialize: function () {
            /*var singleSelect = new Backbone.Picky.SingleSelect(this);
            _.extend(this, singleSelect);*/
        }
    });

    var API = {
        getRepos: function () {
            if (Entities.repos === undefined) {
                Entities.repos = new Entities.RepoCollection();
            }

            var dfd = $.Deferred();
            Entities.repos.fetch({
                success: function(data) {
                    dfd.resolve(data);
                }
            });
            return dfd.promise();
        }
    };

    App.reqres.setHandler("header:repos", function () {
        return API.getRepos();
    });
});
