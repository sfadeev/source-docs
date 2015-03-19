/** @jsx React.DOM */
var React = require('react');

var RepositorySelector = require('./RepositorySelector');
var RepositoryIndex = require('./RepositoryIndex.js');
var RepositoryDocument = require('./RepositoryDocument.js');

var Repository = React.createClass({

    render: function() {
        console.log("App.render", this.props);

        return (
            <div>
                <div className="navbar navbar-default navbar-fixed-top">
                    <div className="container-fluid">

                        <div className="navbar-header">

                            <button type="button" className="navbar-toggle collapsed" data-toggle="collapse" data-target=".navbar-collapse">
                                <span className="sr-only">Toggle navigation</span>
                                <span className="icon-bar"></span>
                                <span className="icon-bar"></span>
                                <span className="icon-bar"></span>
                            </button>

                            <a className="navbar-brand" href="/"><span className="glyphicon glyphicon-home" aria-hidden="true"></span></a>
                        </div>

                        <div className="collapse navbar-collapse">
                            <RepositorySelector />
                        </div>

                    </div>
                </div>

                <div className="container-fluid">
                    <div className="row">
                        <RepositoryIndex {...this.props} />
                        <RepositoryDocument />
                    </div>
                </div>
            </div>
        );
    },

});

module.exports = Repository;