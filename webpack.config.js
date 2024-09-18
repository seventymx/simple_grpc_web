const fs = require("fs");
const path = require("path");

const HtmlWebpackPlugin = require("html-webpack-plugin");
const webpack = require("webpack");
const ReactRefreshWebpackPlugin = require("@pmmmwh/react-refresh-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");

const devServerPort = JSON.stringify(process.env.SHOP_INTERFACE_PORT);
const devServerHost = "localhost";

module.exports = (_env, argv) => {
    const isDevelopment = argv.mode !== "production";

    return {
        entry: "./frontend/src/index.jsx",
        output: {
            path: path.resolve(__dirname, "frontend/dist"),
            filename: `bundle-[contenthash].min.js`
        },
        mode: isDevelopment ? "development" : "production",
        devtool: isDevelopment ? "eval-source-map" : false, // Source maps in development
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                },
                {
                    test: /\.css$/,
                    exclude: /\.module\.css$/,
                    use: [
                        "style-loader",
                        {
                            loader: "css-loader"
                        },
                        {
                            loader: "postcss-loader",
                            options: {
                                postcssOptions: {
                                    plugins: [require("tailwindcss"), require("autoprefixer")]
                                }
                            }
                        }
                    ]
                },
                {
                    test: /\.module\.css$/,
                    use: [
                        "style-loader",
                        {
                            loader: "css-loader",
                            options: {
                                modules: true, // Enable CSS Modules
                                importLoaders: 1
                            }
                        },
                        {
                            loader: "postcss-loader",
                            options: {
                                postcssOptions: {
                                    plugins: [require("tailwindcss"), require("autoprefixer")]
                                }
                            }
                        }
                    ]
                },
                {
                    test: /\.(ts|tsx)$/,
                    include: [path.resolve(__dirname, "frontend/src/shadcn")],
                    use: "ts-loader",
                    exclude: /node_modules/
                }
            ]
        },
        resolve: {
            extensions: [".js", ".jsx", ".ts", ".tsx"],
            alias: {
                "@": path.resolve(__dirname, "frontend/src/shadcn")
            }
        },
        plugins: [
            new HtmlWebpackPlugin({
                template: "frontend/src/index.html"
            }),
            new webpack.DefinePlugin({
                "process.env.SERVICE_BASE_ADDRESS": JSON.stringify(process.env.SERVICE_BASE_ADDRESS)
            }),
            isDevelopment && new ReactRefreshWebpackPlugin(),
            new CopyWebpackPlugin({
                patterns: [
                    {
                        from: path.resolve(__dirname, "frontend/public"),
                        to: path.resolve(__dirname, "frontend/dist")
                    }
                    // {
                    //     from: path.resolve(__dirname, "frontend/resources"),
                    //     to: path.resolve(__dirname, "frontend/dist/resources")
                    // }
                ]
            })
        ].filter(Boolean),
        devServer: {
            host: devServerHost,
            port: devServerPort,
            server: {
                type: "https",
                options: {
                    key: fs.readFileSync(path.resolve(__dirname, "cert/localhost.key")),
                    cert: fs.readFileSync(path.resolve(__dirname, "cert/localhost.crt"))
                }
            },
            client: {
                webSocketURL: {
                    hostname: devServerHost,
                    port: devServerPort,
                    protocol: "wss" // Use 'wss' for secure WebSocket connections
                }
            },
            static: [
                {
                    directory: path.join(__dirname, "./frontend/public")
                }
            ],
            hot: true,
            open: false, // Disable auto-opening the browser
            watchFiles: {
                paths: [path.join(__dirname, "frontend/src/**/*"), path.join(__dirname, "frontend/public/**/*")] //, path.join(__dirname, "frontend/resources/**/*")]
            }
        }
    };
};
