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
        initialize: function() {
            Backbone.Cycle.SelectableModel.applyTo(this);
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

    Entities.RepoIndexItemModel = Backbone.Model.extend({
        initialize: function () {
            Backbone.Cycle.SelectableModel.applyTo(this);
        }
    });

    Entities.RepoIndexItemCollection = Backbone.Collection.extend({
        model: Entities.RepoIndexItemModel,

        initialize: function (models, options) {

            /*var children = this.get("children");
            if (children) {
                this.children = new RepoIndexItemCollection(children);
                this.unset("children");
            }*/

            Backbone.Cycle.SelectableCollection.applyTo(this, models, options);
        }
    });

    Entities.RepoIndexModel = Backbone.Model.extend({
        initialize: function (options) {
            this.options = options;
            Backbone.Cycle.SelectableModel.applyTo(this);
        },
        url: function () {
            return App.config.api.url + "repos/" + this.options.repoId + "/" + this.options.nodeName + "/index";
        }
    });

    var API = {

        loadRepos: function () {
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
        },

        loadRepoIndex: function (repoId, nodeName) {

            var result = new Entities.RepoIndexModel({ repoId: repoId, nodeName: nodeName });

            var dfd = $.Deferred();
            result.fetch({
                // type: "POST",
                cache: false,
                success: function(data) {
                    dfd.resolve(data);
                }
            });
            return dfd.promise();
        }

    };

    App.reqres.setHandler("Entities:loadRepos", function () {
        return API.loadRepos();
    });

    App.reqres.setHandler("Entities:loadRepoIndex", function (repoId, nodeName) {
        return API.loadRepoIndex(repoId, nodeName);
    });
});
