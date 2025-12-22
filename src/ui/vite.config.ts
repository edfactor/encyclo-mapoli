import react from "@vitejs/plugin-react";
import fs from "fs";
import path from "path";
import type { PreRenderedAsset } from "rollup";
import { defineConfig, loadEnv } from "vite";
import compress from "vite-plugin-compression";

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export default defineConfig(async ({ command, mode }) => {
  // Dynamic import for React Compiler to work with ES modules
  const ReactCompilerConfig = (await import("babel-plugin-react-compiler")).default;
  const env = loadEnv(mode, process.cwd(), "");
  const isProd = mode === "production";

  // Load build version from .buildinfo.json if available
  let buildVersion = "1.0.1"; // fallback to package.json version
  try {
    const buildInfoPath = path.resolve(__dirname, "../../.buildinfo.json");
    if (fs.existsSync(buildInfoPath)) {
      const buildInfo = JSON.parse(fs.readFileSync(buildInfoPath, "utf-8"));
      if (buildInfo.buildNumber && buildInfo.buildId) {
        buildVersion = `${buildInfo.buildNumber}.${buildInfo.buildId}`;
      }
    }
  } catch (_e) {
    // Silently ignore errors reading buildinfo.json
  }
  return {
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
        reduxstore: path.resolve(__dirname, "./src/reduxstore"),
        utils: path.resolve(__dirname, "./src/utils"),
        Okta: path.resolve(__dirname, "./src/Okta"),
        components: path.resolve(__dirname, "./src/components"),
        pages: path.resolve(__dirname, "./src/pages"),
        schemas: path.resolve(__dirname, "./src/schemas"),
        hooks: path.resolve(__dirname, "./src/hooks"),
        styles: path.resolve(__dirname, "./src/styles")
      }
    },
    server: {
      port: 3100,
      strictPort: true // Prevents Vite from trying other ports if 3100 is unavailable
    },
    define: {
      //'process.env.YOUR_STRING_VARIABLE': JSON.stringify(env.YOUR_STRING_VARIABLE),
      //'process.env.YOUR_BOOLEAN_VARIABLE': env.YOUR_BOOLEAN_VARIABLE,
      // If you want to exposes all env variables, which is not recommended
      "process.env": env,
      __APP_VERSION__: JSON.stringify(buildVersion)
    },
    optimizeDeps: {
      include: ["@emotion/react", "@emotion/styled", "@mui/material/Tooltip"]
    },
    test: {
      globals: true,
      setupFiles: ["./src/tests/utils/setup.ts"],
      environment: "jsdom",
      include: ["**/*.test.tsx"],
      coverage: {
        reporter: ["text", "html"]
      },
      reporters: ["default", "junit"],
      outputFile: {
        junit: "./FE_Tests/unit-test.xml"
      }
    },
    plugins: [
      react({
        babel: {
          plugins: [["babel-plugin-react-compiler", ReactCompilerConfig]]
        }
      }),
      compress({
        algorithm: "brotliCompress"
      })
    ],
    build: {
      outDir: "build",
      exmptyOutDir: true,
      minify: isProd,
      rollupOptions: {
        output: {
          assetFileNames: (assetInfo: PreRenderedAsset) => {
            let extType = assetInfo.name!.split(".").at(1)!;
            if (/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)) {
              extType = "img";
            }
            if (extType === "css") {
              return `static/${extType}/[name]-[hash][extname]`;
            }
            return `static/media/[name]-[hash][extname]`;
          },
          chunkFileNames: "static/js/[name]-[hash].js",
          entryFileNames: "static/js/[name]-[hash].js"
        }
      }
    }
  };
});
