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