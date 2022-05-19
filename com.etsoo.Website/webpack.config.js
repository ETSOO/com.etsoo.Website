/// <binding BeforeBuild='Run - Production' ProjectOpened='Watch - Development' />
const path = require('path');
const fs = require('fs');
const TerserPlugin = require("terser-webpack-plugin");

const entries = Object.fromEntries(
    fs.readdirSync('./Scripts/').filter((file) => file.endsWith('.ts') || file.endsWith('.tsx')).map(file => [file.split('.')[0], `./Scripts/${file}`])
);

module.exports = {
    entry: entries,
    externals: ['bootstrap', 'wx'],
    module: {
        rules: [
            {
                test: /\.(js|jsx|tsx|ts)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: [
                            [
                                '@babel/preset-env',
                                { modules: false, targets: 'defaults' }
                            ],
                            '@babel/preset-typescript'
                        ],
                        plugins: [
                            '@babel/plugin-transform-runtime',
                            '@babel/proposal-class-properties',
                            '@babel/proposal-object-rest-spread'
                        ]
                    }
                }
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js', '.jsx']
    },
    optimization: {
        minimize: true,
        minimizer: [new TerserPlugin()],
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot/js/')
    }
};
