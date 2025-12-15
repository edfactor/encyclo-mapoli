import fs from "node:fs";
import path from "node:path";
import process from "node:process";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const REPO_ROOT = path.resolve(__dirname, "..");

function readText(filePath) {
  return fs.readFileSync(filePath, { encoding: "utf8" });
}

function parseTsRoutes(constantsTs) {
  const match = constantsTs.match(/export\s+const\s+ROUTES\s*=\s*\{([\s\S]*?)\}\s+as\s+const;/);
  if (!match) {
    throw new Error("Unable to locate ROUTES object in src/ui/src/constants.ts");
  }

  const body = match[1];
  /** @type {Record<string, string>} */
  const routes = {};

  for (const m of body.matchAll(/\s*([A-Z0-9_]+)\s*:\s*"([^"]+)"\s*,?/g)) {
    routes[m[1]] = m[2];
  }

  return routes;
}

function parseRouterMapping(routerTsx) {
  /** @type {Record<string, string>} */
  const importMap = {};

  for (const m of routerTsx.matchAll(
    /const\s+([A-Z][A-Za-z0-9_]*)\s*=\s*lazy\(\s*\(\)\s*=>\s*import\(\s*"([^"]+)"\s*\)\s*\)\s*;/g
  )) {
    importMap[m[1]] = m[2];
  }

  /** @type {Record<string, string>} */
  const routeToComponent = {};

  function tryGetRouteKey(block) {
    // Prefer extracting from the path= attribute, but fall back to any ROUTES.X occurrence.
    const pathIdx = block.indexOf("path=");
    if (pathIdx >= 0) {
      const window = block.slice(pathIdx, pathIdx + 600);
      const m = window.match(/ROUTES\.([A-Z0-9_]+)/);
      if (m) {
        return m[1];
      }
    }

    const mAny = block.match(/ROUTES\.([A-Z0-9_]+)/);
    return mAny ? mAny[1] : null;
  }

  function tryGetComponent(block) {
    // The block often contains other JSX like <PageLoadingFallback /> inside props.
    // Prefer a component that has a corresponding lazy import entry.
    const candidates = Array.from(block.matchAll(/<([A-Z][A-Za-z0-9_]*)\s*\/>/g)).map((x) => x[1]);
    return candidates.find((c) => importMap[c]) ?? null;
  }

  // 1) Standard <Route ...>...</Route> blocks.
  for (const m of routerTsx.matchAll(/(^[\t ]*)<Route\b[\s\S]*?<\/Route>/gm)) {
    const block = m[0];
    const routeKey = tryGetRouteKey(block);
    if (!routeKey) {
      continue;
    }

    const chosen = tryGetComponent(block);
    if (!chosen) {
      continue;
    }

    routeToComponent[routeKey] = chosen;
  }

  // 2) Self-closing <Route ... /> blocks. Use indentation to avoid stopping at nested "/>".
  for (const m of routerTsx.matchAll(/(^[\t ]*)<Route\b[\s\S]*?\n\1\/>\s*/gm)) {
    const block = m[0];
    const routeKey = tryGetRouteKey(block);
    if (!routeKey) {
      continue;
    }

    const chosen = tryGetComponent(block);
    if (!chosen) {
      continue;
    }

    routeToComponent[routeKey] = chosen;
  }

  /** @type {Record<string, string>} */
  const routeKeyToPageGlob = {};
  for (const [routeKey, component] of Object.entries(routeToComponent)) {
    const imp = importMap[component];
    if (!imp) {
      continue;
    }

    const impNorm = imp.replaceAll("\\\\", "/").replaceAll("//", "/");
    const idx = impNorm.indexOf("/pages/");
    if (idx === -1) {
      continue;
    }

    const pagesRel = impNorm.substring(idx + 1);
    const folder = pagesRel.split("/").slice(0, -1).join("/");
    routeKeyToPageGlob[routeKey] = `src/ui/src/${folder}/*`;
  }

  return routeKeyToPageGlob;
}

function parseSqlNavItems(sql) {
  /** @type {Record<string, number>} */
  const constants = {};
  for (const m of sql.matchAll(/\b([A-Z0-9_]+)\s+CONSTANT\s+NUMBER\s*:=\s*(\d+)\s*;/g)) {
    constants[m[1]] = Number.parseInt(m[2], 10);
  }

  /** @type {Record<number, string>} */
  const roleIdToName = {};
  for (const m of sql.matchAll(
    /INSERT\s+INTO\s+NAVIGATION_ROLE\s*\(ID,\s*NAME,\s*IS_READ_ONLY\)\s*VALUES\s*\((\d+),\s*'([^']+)'\s*,\s*\d+\);/gi
  )) {
    roleIdToName[Number.parseInt(m[1], 10)] = m[2];
  }

  function resolveNumber(token) {
    const t = token.trim();
    if (/^\d+$/.test(t)) {
      return Number.parseInt(t, 10);
    }
    return constants[t] ?? null;
  }

  /** @type {Record<string, { friendlyName: string, url: string, navigationId: number, roles: string[] }>} */
  const urlToNav = {};

  for (const m of sql.matchAll(/insert_navigation_item\(([^;]+)\);/gi)) {
    const args = m[1];
    const parts = args.split(",").map((p) => p.trim());
    if (parts.length < 5) {
      continue;
    }

    const navId = resolveNumber(parts[0]);
    if (navId == null) {
      continue;
    }

    const titleMatch = parts[2].match(/^'([^']*)'/);
    const urlMatch = parts[4].match(/^'([^']*)'/);
    if (!titleMatch || !urlMatch) {
      continue;
    }

    const title = titleMatch[1].trim();
    const url = urlMatch[1].trim();
    if (!url) {
      continue;
    }

    urlToNav[url] = {
      friendlyName: title.toUpperCase() === title ? title.toLowerCase().replace(/\b\w/g, (c) => c.toUpperCase()) : title,
      url,
      navigationId: navId,
      roles: []
    };
  }

  /** @type {Record<number, Set<number>>} */
  const navIdToRoleIds = {};
  for (const m of sql.matchAll(/assign_navigation_role\(([^;]+)\);/gi)) {
    const args = m[1];
    const parts = args.split(",").map((p) => p.trim());
    if (parts.length !== 2) {
      continue;
    }
    const navId = resolveNumber(parts[0]);
    const roleId = resolveNumber(parts[1]);
    if (navId == null || roleId == null) {
      continue;
    }
    navIdToRoleIds[navId] ??= new Set();
    navIdToRoleIds[navId].add(roleId);
  }

  for (const nav of Object.values(urlToNav)) {
    const roleIds = Array.from(navIdToRoleIds[nav.navigationId] ?? []).sort((a, b) => a - b);
    nav.roles = roleIds.map((rid) => roleIdToName[rid] ?? `RoleId-${rid}`);
  }

  return urlToNav;
}

function scanHooksUnder(repoRelativeGlob) {
  // repoRelativeGlob is something like src/ui/src/pages/MasterInquiry/*
  const folder = repoRelativeGlob.endsWith("/*")
    ? repoRelativeGlob.slice(0, -2)
    : repoRelativeGlob;

  const absFolder = path.resolve(REPO_ROOT, folder);
  if (!fs.existsSync(absFolder)) {
    return [];
  }

  /** @type {Set<string>} */
  const hookNames = new Set();

  /** @param {string} dir */
  function walk(dir) {
    for (const ent of fs.readdirSync(dir, { withFileTypes: true })) {
      const p = path.join(dir, ent.name);
      if (ent.isDirectory()) {
        walk(p);
        continue;
      }
      if (!/\.tsx?$/.test(ent.name)) {
        continue;
      }
      const text = readText(p);
      for (const m of text.matchAll(/\b(use[A-Za-z0-9_]+(?:Query|Mutation))\b/g)) {
        hookNames.add(m[1]);
      }
    }
  }

  walk(absFolder);
  return Array.from(hookNames).sort();
}

function parseBackendGroupPolicies() {
  const groupsDir = path.resolve(
    REPO_ROOT,
    "src/services/src/Demoulas.ProfitSharing.Endpoints/Groups"
  );

  /** @type {Record<string, string[]>} */
  const routeToPolicies = {};

  /** @type {Set<string>} */
  const knownRoutes = new Set();

  if (!fs.existsSync(groupsDir)) {
    return routeToPolicies;
  }

  for (const fileName of fs.readdirSync(groupsDir)) {
    if (!fileName.endsWith(".cs")) {
      continue;
    }
    const csPath = path.join(groupsDir, fileName);
    const text = readText(csPath);

    const mRoute = text.match(/protected\s+override\s+string\s+Route\s*=>\s*"([^"]+)"\s*;/);
    if (!mRoute) {
      continue;
    }
    const route = mRoute[1].trim().replace(/^\//, "");
    if (route) {
      knownRoutes.add(route);
    }

    /** @type {Set<string>} */
    const policies = new Set();
    for (const m of text.matchAll(/ep\.Policies\(([^\)]*)\)\s*;/g)) {
      const args = m[1];
      for (const pm of args.matchAll(/Policy\.([A-Za-z0-9_]+)/g)) {
        policies.add(pm[1]);
      }
    }

    if (policies.size > 0) {
      routeToPolicies[route] = Array.from(policies).sort();
    }
  }

  return { routeToPolicies, knownRoutes };
}

function hookToEndpointKey(hookName) {
  const m = hookName.match(/^use(?:Lazy)?([A-Z][A-Za-z0-9_]*)?(Query|Mutation)$/);
  if (!m || !m[1]) {
    return null;
  }
  const core = m[1];
  return core[0].toLowerCase() + core.slice(1);
}

function parseRtkUrls() {
  const apiRoot = path.resolve(REPO_ROOT, "src/ui/src/reduxstore");

  /** @type {Record<string, Set<string>>} */
  const endpointToUrls = {};

  if (!fs.existsSync(apiRoot)) {
    return {};
  }

  /**
   * Best-effort extraction of a stable path prefix from a template literal.
   * Examples:
   *  `yearend/vesting/${id}` => "yearend/vesting/"
   *  `${baseUrl}?archive=true` => null (not stable)
   *
   * @param {string} templateBody
   */
  function normalizeTemplateLiteralPath(templateBody) {
    const raw = templateBody.trim().replace(/^\//, "");
    if (!raw) {
      return null;
    }

    // If the template starts with interpolation, we can't infer a stable prefix.
    if (raw.startsWith("${")) {
      return null;
    }

    // Keep only the static prefix before any interpolation.
    const beforeInterp = raw.split("${", 1)[0];
    const withoutQuery = beforeInterp.split("?", 1)[0];
    const cleaned = withoutQuery.trim();
    if (!cleaned) {
      return null;
    }
    if (!cleaned.includes("/")) {
      // We only care about path-ish strings.
      return null;
    }
    return cleaned;
  }

  /** @param {string} dir */
  function walk(dir) {
    for (const ent of fs.readdirSync(dir, { withFileTypes: true })) {
      const p = path.join(dir, ent.name);
      if (ent.isDirectory()) {
        walk(p);
        continue;
      }
      if (!ent.isFile() || !ent.name.endsWith(".ts")) {
        continue;
      }

      const text = readText(p);
      if (!text.includes("builder.query") && !text.includes("builder.mutation")) {
        continue;
      }

      for (const m of text.matchAll(
        /\b([a-zA-Z0-9_]+)\s*:\s*builder\.(query|mutation)(?:\s*<[\s\S]*?>\s*)?\s*\(/g
      )) {
        const endpointKey = m[1];
        const start = m.index ?? 0;
        const window = text.slice(start, start + 3000);

        // Prefer url: "..." or url: `...`
        for (const um of window.matchAll(/\burl\s*:\s*(?:"([^"]+)"|`([^`]+)`)/g)) {
          const raw = (um[1] ?? um[2] ?? "").trim();
          if (!raw) {
            continue;
          }
          if (raw.startsWith("http")) {
            continue;
          }

          const normalized = um[2]
            ? normalizeTemplateLiteralPath(raw)
            : raw.replace(/^\//, "");

          if (!normalized) {
            continue;
          }

          endpointToUrls[endpointKey] ??= new Set();
          endpointToUrls[endpointKey].add(normalized);
        }

        // Capture common patterns like: const baseUrl = `yearend/foo`;
        // (Very common in this codebase; helps when url is built from baseUrl.)
        for (const bm of window.matchAll(/\bconst\s+baseUrl\s*=\s*`([^`]+)`\s*;/g)) {
          const normalized = normalizeTemplateLiteralPath(bm[1]);
          if (!normalized) {
            continue;
          }
          endpointToUrls[endpointKey] ??= new Set();
          endpointToUrls[endpointKey].add(normalized);
        }

        // Fallback: any "segment/segment" literal
        if (!endpointToUrls[endpointKey] || endpointToUrls[endpointKey].size === 0) {
          for (const sm of window.matchAll(/"([a-zA-Z0-9_\-/]+)"/g)) {
            const url = sm[1];
            if (!url.includes("/")) {
              continue;
            }
            if (url.startsWith("http") || url.startsWith("src/")) {
              continue;
            }
            endpointToUrls[endpointKey] ??= new Set();
            endpointToUrls[endpointKey].add(url.replace(/^\//, ""));
          }

          // Also consider template literals without interpolation.
          for (const tm of window.matchAll(/`([^`]+)`/g)) {
            const normalized = normalizeTemplateLiteralPath(tm[1]);
            if (!normalized) {
              continue;
            }
            if (normalized.startsWith("http") || normalized.startsWith("src/")) {
              continue;
            }
            endpointToUrls[endpointKey] ??= new Set();
            endpointToUrls[endpointKey].add(normalized);
          }
        }
      }
    }
  }

  walk(apiRoot);

  /** @type {Record<string, string[]>} */
  const out = {};
  for (const [k, v] of Object.entries(endpointToUrls)) {
    out[k] = Array.from(v).sort();
  }
  return out;
}

function backendPrefixesFromUrls(urls) {
  /** @type {Set<string>} */
  const prefixes = new Set();
  for (const url of urls) {
    const seg = url.trim().replace(/^\//, "").split("/", 1)[0].trim();
    if (seg) {
      prefixes.add(seg);
    }
  }
  return prefixes;
}

function generateMappingTable(baseUrl) {
  const constantsTsPath = path.resolve(REPO_ROOT, "src/ui/src/constants.ts");
  const routerTsxPath = path.resolve(REPO_ROOT, "src/ui/src/components/router/RouterSubAssembly.tsx");
  const sqlPath = path.resolve(REPO_ROOT, "src/database/ready_import/Navigations/add-navigation-data.sql");

  const routes = parseTsRoutes(readText(constantsTsPath));
  const routeKeyToPageGlob = parseRouterMapping(readText(routerTsxPath));
  const urlToNav = parseSqlNavItems(readText(sqlPath));

  const { routeToPolicies: groupPolicies, knownRoutes: knownGroupRoutes } = parseBackendGroupPolicies();
  const endpointToUrls = parseRtkUrls();

  /** @type {Array<Record<string, string>>} */
  const rows = [];

  for (const routeKey of Object.keys(routeKeyToPageGlob).sort()) {
    const pageGlob = routeKeyToPageGlob[routeKey];
    const routeUrl = routes[routeKey];
    if (!routeUrl) {
      continue;
    }

    const nav = urlToNav[routeUrl];
    const friendly = nav ? nav.friendlyName : routeKey.replaceAll("_", " ").toLowerCase().replace(/\b\w/g, (c) => c.toUpperCase());
    const navId = nav ? String(nav.navigationId) : "(no nav ID)";

    const hooks = scanHooksUnder(pageGlob);
    const endpointKeys = hooks.map(hookToEndpointKey).filter(Boolean);

    /** @type {Set<string>} */
    const backendPrefixes = new Set();

    for (const endpointKey of endpointKeys) {
      const urls = endpointToUrls[endpointKey] ?? [];
      for (const prefix of backendPrefixesFromUrls(urls)) {
        backendPrefixes.add(prefix);
      }
    }

    const backendPrefixNote2 = backendPrefixes.size > 0
      ? `  → backend: ${Array.from(backendPrefixes).sort().map((p) => `${p}/*`).join(", ")}`
      : "";

    const apisCalled = hooks.length > 0
      ? `${hooks.join(", ")}${backendPrefixNote2}`
      : "(unknown)";

    const policyLabels = Array.from(backendPrefixes).sort().map((prefix) => {
      const pol = groupPolicies[prefix];
      if (pol && pol.length > 0) {
        return `${prefix}/* → ${pol.join(", ")}`;
      }
      if (knownGroupRoutes.has(prefix)) {
        return `${prefix}/* → (no group policy)`;
      }
      return `${prefix}/* → (unknown group)`;
    });

    const mappedPolicy = policyLabels.length > 0 ? policyLabels.join("; ") : "(unknown)";

    let roleLinks;
    if (nav && nav.roles && nav.roles.length > 0) {
      const base = baseUrl.replace(/\/$/, "");
      roleLinks = nav.roles
        .map((role) => `[${role}](${base}/${routeUrl}?impersonationRole=${role})`)
        .join(", ");
    } else {
      roleLinks = navId === "(no nav ID)" ? "(not a nav entry)" : "(none)";
    }

    rows.push({
      "Friendly Name": friendly,
      "Route URL": `\`${routeUrl}\``,
      "Page component (file / folder)": `\`${pageGlob}\``,
      "Navigation ID": navId,
      "APIs called (RTK Query hooks / backend prefix)": apisCalled,
      "Mapped Policy for each API": mappedPolicy,
      "Navigation Roles (click to open with impersonationRole)": roleLinks
    });
  }

  const headers = [
    "Friendly Name",
    "Route URL",
    "Page component (file / folder)",
    "Navigation ID",
    "APIs called (RTK Query hooks / backend prefix)",
    "Mapped Policy for each API",
    "Navigation Roles (click to open with impersonationRole)"
  ];

  const lines = [
    `| ${headers.join(" | ")} |`,
    `| ${headers.map(() => "---").join(" | ")} |`
  ];

  for (const r of rows) {
    lines.push(`| ${headers.map((h) => r[h]).join(" | ")} |`);
  }

  return lines.join("\n") + "\n";
}

function parseArgs(argv) {
  /** @type {Record<string, string>} */
  const args = {};
  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (a.startsWith("--")) {
      const key = a.slice(2);
      const val = argv[i + 1] && !argv[i + 1].startsWith("--") ? argv[++i] : "true";
      args[key] = val;
    }
  }
  return args;
}

const args = parseArgs(process.argv);
const baseUrl = args["base-url"] ?? "https://ps.qa.demoulas.net";
const output = args["output"] ?? path.resolve(REPO_ROOT, "docs/generated/PROFIT_SHARING_SECURITY_MAPPING.md");

fs.mkdirSync(path.dirname(output), { recursive: true });
fs.writeFileSync(output, generateMappingTable(baseUrl), { encoding: "utf8" });
console.log(`Wrote: ${output}`);
