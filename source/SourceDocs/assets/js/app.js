// http://stackoverflow.com/questions/11084021/how-to-use-backbone-marionette-itemview-with-mustache
Marionette.TemplateCache.prototype.compileTemplate = function (rawTemplate) {

    // Mustache.parse will not return anything useful (returns an array)
    // The render function from Marionette.Renderer.render expects a function
    // so instead pass a partial of Mustache.render 
    // with rawTemplate as the initial parameter.

    // Additionally Mustache.compile no longer exists so we must use parse.
    Mustache.parse(rawTemplate);
    return _.partial(Mustache.render, rawTemplate);
};

var App = new Backbone.Marionette.Application();

App.config = {
    api: {
        url: "api/"
    }
};

App.addRegions({
    reposRegion: "#repos-region",
    nodesRegion: "#nodes-region",
    repoIndexRegion: "#repo-index-region",
    mainRegion: "#main-region"
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
        "repo/:repoId(/:nodeName)": "repo"
    },

    home: function () {
        console.log("#home");
    },

    repo: function(repoId, nodeName) {
        console.log("#repo/" + repoId + "/" + nodeName);

        if (repoId) App.commands.execute("set:active:repo", repoId);
        if (nodeName) App.commands.execute("set:active:node", nodeName);
    }

});

App.router = new App.Router();

/*(function() {
    var trigger = Backbone.Events.trigger;
    var wrapper = function() {
        console.log("Event Triggered: " + arguments[0]);
        trigger.apply(this, arguments);
    };
    Backbone.Model.prototype.trigger = wrapper;
    Backbone.Collection.prototype.trigger = wrapper;
    Backbone.View.prototype.trigger = wrapper;
})();*/

App.on("start", function() {
    if (Backbone.history) {
        Backbone.history.start(/*{ pushState: true }*/);

        /*if (this.getCurrentRoute() === "") {
            App.trigger("contacts:list");
        }*/
    }
});
