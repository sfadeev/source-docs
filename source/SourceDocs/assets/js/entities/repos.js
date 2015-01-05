App.module("Entities", function (Entities, App, Backbone, Marionette, $, _) {
    Entities.Repo = Backbone.Model.extend({
        initialize: function () {
            Backbone.Cycle.SelectableModel.applyTo(this);
        }
    });

    Entities.RepoCollection = Backbone.Collection.extend({
        url: App.config.api.url + "repos",
        model: Entities.Repo,
        initialize: function ( models, options ) {
            Backbone.Cycle.SelectableCollection.applyTo( this, models, options );
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
