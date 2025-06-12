import CompressionPlugin from "compression-webpack-plugin";
import HtmlWebpackPlugin from "html-webpack-plugin";
import { resolve } from "path";
import webpack from "webpack";

/** @type {(env: any, argv: any) => import("webpack").Configuration} */
export default (_env, argv) => {
  const mode = argv?.mode ?? "production";
  return {
    mode,
    target: "web",
    entry: "./src/main.ts",
    output: { path: resolve("./wwwroot") },
    resolve: {
      extensions: [".ts", ".js"],
      modules: ["./src", "./node_modules"],
    },
    module: {
      rules: [
        { test: /\.ts$/, loader: "ts-loader", exclude: /node_modules/ },
        { test: /\.html$/, loader: "html-loader", exclude: /node_modules/ },
        {
          test: /\.css$/,
          use: [{ loader: "style-loader" }, { loader: "css-loader" }],
        },
      ],
    },
    plugins: [
      new webpack.DefinePlugin({
        __MODE__: JSON.stringify(mode),
      }),
      new HtmlWebpackPlugin({
        template: "index.ejs",
      }),
      ...(mode === "production"
        ? [
            new CompressionPlugin({
              test: /\.(js|css|html|svg)$/,
            }),
          ]
        : []),
    ],
    experiments: {
      topLevelAwait: true,
    },
  };
};
