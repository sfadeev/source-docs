var App = new Backbone.Marionette.Application();

App.config = {
    api: {
        url: "api/"
    }
};

App.getApiUrl = function(path) {
    return Backbone.history.root + App.config.api.url + path;
};

App.addRegions({
    reposRegion: "#repos-region",
    nodesRegion: "#nodes-region",
    repoIndexRegion: "#repo-index-region",
    breadcrumbRegion: "#breadcrumb-region",
    mainRegion: "#main-region",
    pagerRegion: "#pager-region"
    /*dialogRegion: Marionette.Region.Dialog.extend({
        el: "#dialog-region"
    })*/
});

App.navigate = function (route, options) {
    options || (options = {});
    Backbone.history.navigate(route, options);
};

App.getCurrentRoute = function () {
    return Backbone.history.fragment;
};

App.Router = Backbone.Router.extend({
    routes: {
        "home": "home",
        "repo/:repoId(/@/*nodeName)(/~/*path)": "repo"
    },

    home: function () {
        console.log("#home");
    },

    repo: function(repoId, nodeName, path) {
        console.log("routing", { history: Backbone.history, repoId: repoId, nodeName: nodeName, path: path });

        if (repoId) App.commands.execute("Repos:selectRepo", repoId);
        if (nodeName) App.commands.execute("Repos:selectNode", nodeName);
        if (path) App.commands.execute("Repos:selectPath", path);
    }

});

App.router = new App.Router();

/*(function() {
    var trigger = Backbone.Events.trigger;
    var wrapper = function() {
        // console.log("Event Triggered: " + arguments[0]);
        trigger.apply(this, arguments);
    };
    Backbone.Model.prototype.trigger = wrapper;
    Backbone.Collection.prototype.trigger = wrapper;
    Backbone.View.prototype.trigger = wrapper;
})();*/

App.on("start", function(options) {
    if (Backbone.history) {
        Backbone.history.start({ pushState: true, root: options.root });

        App.commands.execute("Repos:listRepos");

        /*if (this.getCurrentRoute() === "") {
            App.trigger("contacts:list");
        }*/
    }
});
