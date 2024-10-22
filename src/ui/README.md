# Project Setup

0. Make sure you're authenticated with the Demoulas JFrog instance. (Follow the Set Me Up guide to auth your local npm client in the Artifactory website.)
1. create a new `.npmrc` file in the src/ui directory with the following contents:
```
registry=https://registry.npmjs.org/
smart-ui-library:registry=https://demoulas.jfrog.io/artifactory/api/npm/npm-smart-registry-local/
```
This will ensure that when you run npm install you're only using the Jfrog registry for smart-ui-library, the standard npm registry otherwise.
2. run `npm i --legacy-peer-deps`
3. run `npm run dev`