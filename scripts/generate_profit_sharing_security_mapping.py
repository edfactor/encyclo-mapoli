from __future__ import annotations

import argparse
import os
import re
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


REPO_ROOT = Path(__file__).resolve().parents[1]


@dataclass(frozen=True)
class NavItem:
    friendly_name: str
    url: str
    navigation_id: int
    roles: tuple[str, ...]


def _read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="replace")


def _parse_ts_routes(constants_ts: str) -> dict[str, str]:
    # Parse `export const ROUTES = { KEY: "value", ... } as const;`
    # Keep it intentionally simple: string literals only.
    routes: dict[str, str] = {}

    m = re.search(r"export\s+const\s+ROUTES\s*=\s*\{([\s\S]*?)\}\s+as\s+const;", constants_ts)
    if not m:
        raise RuntimeError("Unable to locate ROUTES object in constants.ts")

    body = m.group(1)
    for key, value in re.findall(r"\s*([A-Z0-9_]+)\s*:\s*\"([^\"]+)\"\s*,?", body):
        routes[key] = value

    return routes


def _parse_router_mapping(router_tsx: str) -> dict[str, str]:
    # Build: componentVar -> importPath
    import_map: dict[str, str] = {}
    for comp, imp in re.findall(
        r"const\s+([A-Z][A-Za-z0-9_]*)\s*=\s*lazy\(\(\)\s*=>\s*import\(\"([^\"]+)\"\)\)\s*;",
        router_tsx,
    ):
        import_map[comp] = imp

    # Build: routeKey (ROUTES.<key>) -> componentVar
    route_to_component: dict[str, str] = {}

    # Heuristic: within a <Route ...> block, grab the first <Component /> we see.
    for match in re.finditer(r"<Route\s+[\s\S]*?path=\{ROUTES\.([A-Z0-9_]+)\}[\s\S]*?</Route>", router_tsx):
        route_key = match.group(1)
        block = match.group(0)
        m_comp = re.search(r"<([A-Z][A-Za-z0-9_]*)\s*/>", block)
        if not m_comp:
            continue
        route_to_component[route_key] = m_comp.group(1)

    # Convert: routeKey -> pages folder wildcard (when possible)
    route_key_to_page_glob: dict[str, str] = {}
    for route_key, component in route_to_component.items():
        imp = import_map.get(component)
        if not imp:
            continue

        # Normalize common patterns:
        #   "../../pages/MasterInquiry/MasterInquiry" => "src/ui/src/pages/MasterInquiry/*"
        #   "../../pages/DecemberActivities/UnForfeit" => "src/ui/src/pages/DecemberActivities/UnForfeit/*"
        #   "../pages/..." etc.
        imp_norm = imp.replace("\\", "/")
        idx = imp_norm.find("/pages/")
        if idx == -1:
            continue

        pages_rel = imp_norm[idx + 1 :]
        # Drop filename portion
        folder = pages_rel.rsplit("/", 1)[0]
        route_key_to_page_glob[route_key] = f"src/ui/src/{folder}/*"

    return route_key_to_page_glob


def _parse_sql_nav_items(sql: str) -> dict[str, NavItem]:
    # Parse numeric constants first so we can resolve insert_navigation_item(MASTER_INQUIRY_PAGE, ...) to 100.
    constants: dict[str, int] = {}
    for name, value in re.findall(r"\b([A-Z0-9_]+)\s+CONSTANT\s+NUMBER\s*:=\s*(\d+)\s*;", sql):
        constants[name] = int(value)

    # Roles table gives canonical names.
    role_id_to_name: dict[int, str] = {}
    for role_id, role_name in re.findall(
        r"INSERT\s+INTO\s+NAVIGATION_ROLE\s*\(ID,\s*NAME,\s*IS_READ_ONLY\)\s*VALUES\s*\((\d+),\s*'([^']+)'\s*,\s*\d+\);",
        sql,
        flags=re.IGNORECASE,
    ):
        role_id_to_name[int(role_id)] = role_name

    def resolve_number(token: str) -> int | None:
        token = token.strip()
        if token.isdigit():
            return int(token)
        return constants.get(token)

    # Capture nav items.
    url_to_nav: dict[str, NavItem] = {}

    # insert_navigation_item(ID, ParentId, 'TITLE', 'SUB', 'URL', ...)
    for m in re.finditer(r"insert_navigation_item\(([^;]+)\);", sql, flags=re.IGNORECASE):
        args = m.group(1)
        # Split args at top-level commas (no parens in these calls, so a naive split works).
        parts = [p.strip() for p in args.split(",")]
        if len(parts) < 5:
            continue

        nav_id_token = parts[0]
        title_token = parts[2]
        url_token = parts[4]

        nav_id = resolve_number(nav_id_token)
        if nav_id is None:
            continue

        title_m = re.match(r"'([^']*)'", title_token)
        url_m = re.match(r"'([^']*)'", url_token)
        if not title_m or not url_m:
            continue

        title = title_m.group(1).strip()
        url = url_m.group(1).strip()
        if not url:
            continue

        url_to_nav[url] = NavItem(
            friendly_name=title.title() if title.isupper() else title,
            url=url,
            navigation_id=nav_id,
            roles=(),
        )

    # Capture role assignments and attach to nav items.
    nav_id_to_role_ids: dict[int, set[int]] = {}
    for m in re.finditer(r"assign_navigation_role\(([^;]+)\);", sql, flags=re.IGNORECASE):
        args = m.group(1)
        parts = [p.strip() for p in args.split(",")]
        if len(parts) != 2:
            continue
        nav_id = resolve_number(parts[0])
        role_id = resolve_number(parts[1])
        if nav_id is None or role_id is None:
            continue
        nav_id_to_role_ids.setdefault(nav_id, set()).add(role_id)

    url_to_nav_with_roles: dict[str, NavItem] = {}
    for url, nav in url_to_nav.items():
        role_ids = sorted(nav_id_to_role_ids.get(nav.navigation_id, set()))
        role_names = tuple(role_id_to_name.get(rid, f"RoleId-{rid}") for rid in role_ids)
        url_to_nav_with_roles[url] = NavItem(
            friendly_name=nav.friendly_name,
            url=nav.url,
            navigation_id=nav.navigation_id,
            roles=role_names,
        )

    return url_to_nav_with_roles


def _scan_hooks_under(path_glob: str) -> list[str]:
    # path_glob is workspace-relative like src/ui/src/pages/MasterInquiry/*
    abs_glob = (REPO_ROOT / path_glob).as_posix()
    root = Path(abs_glob[:-2]) if abs_glob.endswith("/*") else Path(abs_glob)

    if not root.exists():
        return []

    hook_names: set[str] = set()
    for file_path in root.rglob("*.ts*"):
        try:
            text = _read_text(file_path)
        except OSError:
            continue

        for hook in re.findall(r"\b(use[A-Za-z0-9_]+(?:Query|Mutation))\b", text):
            hook_names.add(hook)

    return sorted(hook_names)


def _parse_backend_group_policies() -> tuple[dict[str, list[str]], set[str]]:
    groups_dir = REPO_ROOT / "src/services/src/Demoulas.ProfitSharing.Endpoints/Groups"
    if not groups_dir.exists():
        return {}, set()

    group_route_to_policies: dict[str, list[str]] = {}
    known_routes: set[str] = set()

    for cs in groups_dir.glob("*.cs"):
        text = _read_text(cs)
        m_route = re.search(r"protected\s+override\s+string\s+Route\s*=>\s*\"([^\"]+)\"\s*;", text)
        if not m_route:
            continue
        route = m_route.group(1).strip().lstrip("/")
        if route:
            known_routes.add(route)

        # Capture `ep.Policies(Policy.X, Policy.Y);` and normalize to constant names.
        policies: list[str] = []
        for m_pol in re.finditer(r"ep\.Policies\(([^\)]*)\)\s*;", text):
            args = m_pol.group(1)
            for p in re.findall(r"Policy\.([A-Za-z0-9_]+)", args):
                policies.append(p)

        if policies:
            group_route_to_policies[route] = sorted(set(policies))

    return group_route_to_policies, known_routes


def _hook_to_endpoint_key(hook_name: str) -> str | None:
    # Examples:
    #  useLazyGetFooQuery -> getFoo
    #  useGetFooQuery -> getFoo
    #  useUpdateFooMutation -> updateFoo
    m = re.match(r"^use(?:Lazy)?([A-Z][A-Za-z0-9_]*)?(Query|Mutation)$", hook_name)
    if not m:
        return None
    core = m.group(1)
    if not core:
        return None
    # lower-case first char
    return core[0].lower() + core[1:]


def _parse_rtk_urls() -> dict[str, list[str]]:
    # endpointKey -> list of url strings found in its definition
    api_root = REPO_ROOT / "src/ui/src/reduxstore"
    if not api_root.exists():
        return {}

    endpoint_to_urls: dict[str, set[str]] = {}

    def normalize_template_literal_path(template_body: str) -> str | None:
        raw = template_body.strip().lstrip("/")
        if not raw:
            return None

        # If the template starts with interpolation, it's not stable.
        if raw.startswith("${"):
            return None

        # Keep only the static prefix (before interpolation), and remove querystring.
        before_interp = raw.split("${", 1)[0]
        before_query = before_interp.split("?", 1)[0]
        cleaned = before_query.strip()
        if not cleaned:
            return None
        if "/" not in cleaned:
            return None
        return cleaned

    # This is heuristic: scan api files where RTK Query endpoints live.
    for file_path in api_root.rglob("*.ts"):
        text = _read_text(file_path)
        if "builder.query" not in text and "builder.mutation" not in text:
            continue

        # Find endpoint blocks by `name: builder.query<...>(` or `name: builder.mutation<...>(`
        # Note: generic type params can span lines and include nested generics; the pattern
        # expands until it finds the '>' that is actually followed by '('.
        for m in re.finditer(
            r"\b([a-zA-Z0-9_]+)\s*:\s*builder\.(query|mutation)(?:\s*<[\s\S]*?>\s*)?\s*\(",
            text,
        ):
            endpoint_key = m.group(1)
            start = m.start()
            window = text[start : start + 3000]

            # Prefer explicit url: "..."
            for url in re.findall(r"\burl\s*:\s*\"([^\"]+)\"", window):
                if url.startswith("http"):
                    continue
                endpoint_to_urls.setdefault(endpoint_key, set()).add(url.lstrip("/"))

            # Also support url: `...` template literals.
            for tpl in re.findall(r"\burl\s*:\s*`([^`]+)`", window):
                normalized = normalize_template_literal_path(tpl)
                if not normalized:
                    continue
                if normalized.startswith("http"):
                    continue
                endpoint_to_urls.setdefault(endpoint_key, set()).add(normalized)

            # Capture common pattern: const baseUrl = `prefix/path`;
            for tpl in re.findall(r"\bconst\s+baseUrl\s*=\s*`([^`]+)`\s*;", window):
                normalized = normalize_template_literal_path(tpl)
                if not normalized:
                    continue
                endpoint_to_urls.setdefault(endpoint_key, set()).add(normalized)

            # Fallback: any string literal that looks like a path segment
            if endpoint_key not in endpoint_to_urls:
                for url in re.findall(r"\"([a-zA-Z0-9_\-/]+)\"", window):
                    if "/" not in url:
                        continue
                    if url.startswith("http"):
                        continue
                    if url.startswith("src/"):
                        continue
                    endpoint_to_urls.setdefault(endpoint_key, set()).add(url.lstrip("/"))

                # And template literals without interpolation.
                for tpl in re.findall(r"`([^`]+)`", window):
                    normalized = normalize_template_literal_path(tpl)
                    if not normalized:
                        continue
                    if normalized.startswith("http"):
                        continue
                    if normalized.startswith("src/"):
                        continue
                    endpoint_to_urls.setdefault(endpoint_key, set()).add(normalized)

    return {k: sorted(v) for k, v in endpoint_to_urls.items()}


def _backend_prefix_from_urls(urls: Iterable[str]) -> set[str]:
    prefixes: set[str] = set()
    for url in urls:
        seg = url.strip().lstrip("/").split("/", 1)[0].strip()
        if seg:
            prefixes.add(seg)
    return prefixes


def generate_mapping_table(base_url: str) -> str:
    constants_ts = _read_text(REPO_ROOT / "src/ui/src/constants.ts")
    routes = _parse_ts_routes(constants_ts)

    router_tsx = _read_text(REPO_ROOT / "src/ui/src/components/router/RouterSubAssembly.tsx")
    route_key_to_page_glob = _parse_router_mapping(router_tsx)

    sql = _read_text(REPO_ROOT / "src/database/ready_import/Navigations/add-navigation-data.sql")
    url_to_nav = _parse_sql_nav_items(sql)

    group_policies, known_group_routes = _parse_backend_group_policies()
    endpoint_to_urls = _parse_rtk_urls()

    # Build rows for every ROUTES key we can map to a page folder.
    rows: list[dict[str, str]] = []

    for route_key, page_glob in sorted(route_key_to_page_glob.items()):
        route_url = routes.get(route_key)
        if not route_url:
            continue

        nav = url_to_nav.get(route_url)
        friendly = nav.friendly_name if nav else route_key.replace("_", " ").title()
        nav_id = str(nav.navigation_id) if nav else "(no nav ID)"

        hooks = _scan_hooks_under(page_glob)
        endpoint_keys = [k for h in hooks if (k := _hook_to_endpoint_key(h))]

        backend_urls: set[str] = set()
        backend_prefixes: set[str] = set()
        for endpoint_key in endpoint_keys:
            urls = endpoint_to_urls.get(endpoint_key, [])
            backend_urls.update(urls)
            backend_prefixes.update(_backend_prefix_from_urls(urls))

        backend_prefix_note = ""
        if backend_prefixes:
            backend_prefix_note = " → backend: " + ", ".join(f"{p}/*" for p in sorted(backend_prefixes))

        apis_called = ", ".join(hooks) + backend_prefix_note if hooks else "(unknown)"

        policy_labels: list[str] = []
        for prefix in sorted(backend_prefixes):
            policies = group_policies.get(prefix)
            if policies:
                policy_labels.append(f"{prefix}/* → " + ", ".join(policies))
            else:
                if prefix in known_group_routes:
                    policy_labels.append(f"{prefix}/* → (no group policy)")
                else:
                    policy_labels.append(f"{prefix}/* → (unknown group)")

        mapped_policy = "; ".join(policy_labels) if policy_labels else "(unknown)"

        role_links = ""
        if nav and nav.roles:
            links = []
            for role in nav.roles:
                # Only the clickable links are changed to the base URL.
                links.append(f"[{role}]({base_url.rstrip('/')}/{route_url}?impersonationRole={role})")
            role_links = ", ".join(links)
        else:
            role_links = "(not a nav entry)" if nav_id == "(no nav ID)" else "(none)"

        rows.append(
            {
                "Friendly Name": friendly,
                "Route URL": f"`{route_url}`",
                "Page component (file / folder)": f"`{page_glob}`",
                "Navigation ID": nav_id,
                "APIs called (RTK Query hooks / backend prefix)": apis_called,
                "Mapped Policy for each API": mapped_policy,
                "Navigation Roles (click to open with impersonationRole)": role_links,
            }
        )

    headers = [
        "Friendly Name",
        "Route URL",
        "Page component (file / folder)",
        "Navigation ID",
        "APIs called (RTK Query hooks / backend prefix)",
        "Mapped Policy for each API",
        "Navigation Roles (click to open with impersonationRole)",
    ]

    lines = [
        "| " + " | ".join(headers) + " |",
        "| " + " | ".join(["---"] * len(headers)) + " |",
    ]

    for r in rows:
        lines.append("| " + " | ".join(r[h] for h in headers) + " |")

    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Generate the 'Navigation Permissions and API Mapping' table for Confluence."
    )
    parser.add_argument(
        "--base-url",
        default="https://ps.qa.demoulas.net",
        help="Base UI URL to use for impersonation links (only clickable links are affected).",
    )
    parser.add_argument(
        "--output",
        default=str(REPO_ROOT / "docs/generated/PROFIT_SHARING_SECURITY_MAPPING.md"),
        help="Output markdown file path.",
    )

    args = parser.parse_args()

    out_path = Path(args.output)
    out_path.parent.mkdir(parents=True, exist_ok=True)

    table = generate_mapping_table(args.base_url)

    out_path.write_text(table, encoding="utf-8")
    print(f"Wrote: {out_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
