App.module("Entities", function(Entities, App, Backbone, Marionette, $, _) {

    Entities.Node = Backbone.Model.extend({
        initialize: function() {
            Backbone.Cycle.SelectableModel.applyTo(this);
        }
    });

    Entities.NodeCollection = Backbone.Collection.extend({
        model: Entities.Node,
        initialize: function(models, options) {
            Backbone.Cycle.SelectableCollection.applyTo(this, models, options);
        }
    });

    Entities.Repo = Backbone.Model.extend({
        /*defaults: {
            nodes: new Entities.NodeCollection()
        },*/
        initialize: function() {
            Backbone.Cycle.SelectableModel.applyTo(this);
        },
        parse: function(response, options) {
            this.get("nodes").reset(response.nodes);
        }
    });

    Entities.RepoCollection = Backbone.Collection.extend({
        model: Entities.Repo,
        initialize: function(models, options) {
            Backbone.Cycle.SelectableCollection.applyTo(this, models, options);
        }
    });

    Entities.Repos = Backbone.Model.extend({
        url: App.config.api.url + "repos",
        defaults: {
            items: new Entities.RepoCollection()
        },
        parse: function(response, options) {
            this.get("items").reset(response.items);
            this.get("items").each(function(repo) {
                repo.set("nodes", new Entities.NodeCollection(repo.get("nodes")));
            });
        }
    });

    var API = {
        getRepos: function() {
            if (Entities.repos === undefined) {
                Entities.repos = new Entities.Repos();
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

    App.reqres.setHandler("header:repos", function() {
        return API.getRepos();
    });
});
