let mix = require('laravel-mix');

mix.setPublicPath('wwwroot/')

mix.js('./assets/js/app.js', 'js/site.js')
    .sass('./assets/scss/app.scss', 'css/site.css');