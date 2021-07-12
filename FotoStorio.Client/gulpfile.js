const gulp = require('gulp');
const postcss = require('gulp-postcss');
const cleanCSS = require('gulp-clean-css');
const purgecss = require('gulp-purgecss');

gulp.task('css:dev', () => {
  return gulp.src('./Styles/tailwind.css')
    .pipe(postcss([
      require('precss'),
      require('tailwindcss'),
      require('autoprefixer')
    ]))
    .pipe(gulp.dest('./wwwroot/css/'));
});

gulp.task('css:prod', () => {
  return gulp.src('./Styles/tailwind.css')
    .pipe(postcss([
      require('precss'),
      require('tailwindcss'),
      require('autoprefixer')
    ]))
    .pipe(purgecss({ content: ['**/*.html', '**/*.razor'], defaultExtractor: content => content.match(/[^<>"'`\s]*[^<>"'`\s:]/g) || [] }))
    .pipe(cleanCSS({ level: 2 }))
    .pipe(gulp.dest('./wwwroot/css/'));
});
