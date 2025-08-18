import react from "@vitejs/plugin-react";
import path from "path";
import { defineConfig, loadEnv } from "vite";
import compress from "vite-plugin-compression";
export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  const isProd = mode === "production";
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
      "process.env": env
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
      react(),
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
          assetFileNames: (assetInfo) => {
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
