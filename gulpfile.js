var gulp = require('gulp');
var browserify = require('gulp-browserify');
var concat = require('gulp-concat');

gulp.task('browserify', function() {
    gulp.src('source/SourceDocs/js/main.js')
      .pipe(browserify({transform:'reactify'}))
      .pipe(concat('bundle.js'))
      .pipe(gulp.dest('source/SourceDocs/assets/js'));
});

gulp.task('default',['browserify']);

gulp.task('watch', function() {
    gulp.watch('source/SourceDocs/js/**/*.*', ['default']);
});
