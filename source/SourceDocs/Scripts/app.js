var Dokka = new Backbone.Marionette.Application();

Dokka.addRegions({
    filesRegion: "#files-region",
    mainRegion: "#main-region",
    /*dialogRegion: Marionette.Region.Dialog.extend({
        el: "#dialog-region"
    })*/
});

Dokka.navigate = function (route, options) {
    options || (options = {});
    Backbone.history.navigate(route, options);
};

Dokka.getCurrentRoute = function () {
    return Backbone.history.fragment;
};

Dokka.on("start", function() {
    if (Backbone.history) {
        Backbone.history.start();

        if (this.getCurrentRoute() === "") {
            Dokka.trigger("contacts:list");
        }
    }
});

// temp. from other apps
Dokka.on("about:show", function () {
    Dokka.navigate("about");
    // API.listContacts();
});