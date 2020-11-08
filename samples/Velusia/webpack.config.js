const path = require('path');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const WriteFileWebpackPlugin = require('write-file-webpack-plugin');

module.exports = {
	entry: './styles.css',
	output: {
		filename: 'styles.css',
		path: __dirname + '/Velusia.Server/wwwroot/dist/'
	},
	mode: process.env.NODE_ENV,
	module: {
		rules: [
			{
				test: /\.css$/,
				use: ExtractTextPlugin.extract({
					fallback: 'style-loader',
					use: [
						{
							loader: 'css-loader',
							options: { importLoaders: 1 }
						},
						'postcss-loader'
					]
				})
			},
			{
				test: /\.(png|woff|woff2|eot|ttf|svg)$/,
				loader: ["url-loader?limit=10000&name=[name].[ext]"],
			}
		]
	},
	plugins: [
		new ExtractTextPlugin('styles.css', {
			disable: false
		}),
		new HtmlWebpackPlugin({
			filename: 'output.html',
			template: 'index.html'
		}),
		new WriteFileWebpackPlugin({
			// Write only files that have ".css" extension.
			test: /\.(css|png|woff|woff2|eot|ttf|svg)$/,
			useHashIndex: true
		}),
		
	]
};
