const path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
  mode: 'production',
  entry: './UI/main.js',
  output: {
    path: path.resolve(__dirname, 'dist/ui'),
    filename: 'bundle.js',
  },
  module: {
    rules: [
      { test: /\.(png|svg|jpg|gif)$/, use: [{ loader: 'file-loader', options: { esModule: false } }] },
      { test: /\.vue$/, loader: 'vue-loader' },
      { test: /\.css$/, use: ['vue-style-loader', 'css-loader'] },
    ],
  },
  plugins: [
    new VueLoaderPlugin(),
    new HtmlWebpackPlugin({ template: './UI/index.html' })],
  resolve: {
    alias: {
      vue$: 'vue/dist/vue.esm.js',
    },
    extensions: ['*', '.js', '.vue', '.json'],
  },
};
