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
    filesRegion: "#files-region",
    mainRegion: "#main-region",
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

App.on("start", function() {
    if (Backbone.history) {
        Backbone.history.start();

        if (this.getCurrentRoute() === "") {
            App.trigger("contacts:list");
        }
    }
});

// temp. from other apps
App.on("about:show", function () {
    App.navigate("about");
    // API.listContacts();
});