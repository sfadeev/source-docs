var gulp = require('gulp'),
    concat = require('gulp-concat');

gulp.task('lib_scripts', function() {
    gulp.src([
            './bower_components/jquery/dist/jquery.js',
            './bower_components/bootstrap/dist/js/bootstrap.js',
            './bower_components/json2/json2.js',
            './bower_components/underscore/underscore.js',
            './bower_components/backbone/backbone.js',
            './bower_components/backbone.cycle/dist/backbone.cycle.js',
            './bower_components/marionette/lib/backbone.marionette.js'
        ])
        .pipe(concat('libs.js'))
        .pipe(gulp.dest('./SourceDocs/assets/js/'));
});
