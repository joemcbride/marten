var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');

var output = './public';

module.exports = {

  devtool: 'source-map',

  entry: {
    'index': './Content/app/index/index.js',
    'create': './Content/app/create/index.js',
    'graphiql': './Content/app/graphiql/index.js'
  },

  output: {
    path: output,
    filename: '[name].js'
  },

  resolve: {
    extensions: ['', '.js', '.json']
  },

  module: {
    loaders: [
      { test: /\.js/, loader: 'babel', exclude: /node_modules/ },
      { test: /\.css$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader!postcss-loader') }
    ]
  },

  plugins: [
    new ExtractTextPlugin('[name].css', { allChunks: true })
  ]
};
