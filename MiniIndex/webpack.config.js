const glob = require("glob");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: {
        site: ["./wwwroot/js/site.js", "./wwwroot/css/site.css"],
        styles: "./wwwroot/css/styles.css",
        validation: "./wwwroot/js/validation.js",
        tags: "./wwwroot/js/tags-tagify.js",
        mini: ["./wwwroot/js/mini.js", "./wwwroot/css/mini.css"],
        latest: "./wwwroot/js/latest.js",
        admin: "./wwwroot/js/admin.js",
        edit: "./wwwroot/js/edit.js"
    },
    output: {
        filename: "[name].entry.js",
        path: __dirname + "/wwwroot/dist",
    },
    devtool: "source-map",
    mode: "development",
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [{ loader: MiniCssExtractPlugin.loader }, "css-loader"],
            },
            { test: /\.eot(\?v=\d+\.\d+\.\d+)?$/, loader: "file-loader" },
            {
                test: /\.(woff|woff2)$/,
                loader: "url-loader?prefix=font/&limit=5000",
            },
            {
                test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/,
                loader: "url-loader?limit=10000&mimetype=application/octet-stream",
            },
            {
                test: /\.otf(\?v=\d+\.\d+\.\d+)?$/,
                loader: "url-loader?limit=10000&mimetype=image/svg+xml",
            },
            {
                test: /\.svg(\?v=\d+\.\d+\.\d+)?$/,
                loader: "url-loader?limit=10000&mimetype=image/svg+xml",
            },
            { test: /\.png$/, loader: "file-loader" },
            { test: /\.gif$/, loader: "file-loader" },
            {
                test: /\.(scss)$/,
                use: [
                    {
                        loader: "style-loader", // inject CSS to page
                    },
                    {
                        loader: "css-loader", // translates CSS into CommonJS modules
                    },
                    {
                        loader: "postcss-loader", // Run postcss actions
                        options: {
                            plugins: function () {
                                // postcss plugins, can be exported to postcss.config.js
                                return [require("autoprefixer")];
                            },
                        },
                    },
                    {
                        loader: "sass-loader", // compiles Sass to CSS
                    },
                ],
            },
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css",
        }),
    ],
};
