<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **my-first-game** (1033 symbols, 2424 relationships, 85 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/my-first-game/context` | Codebase overview, check index freshness |
| `gitnexus://repo/my-first-game/clusters` | All functional areas |
| `gitnexus://repo/my-first-game/processes` | All execution flows |
| `gitnexus://repo/my-first-game/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->

## Đồ họa HD-2D (đang triển khai)

Dự án đang được nâng cấp đồ họa lên phong cách **HD-2D qua URP** (Mức 3). Trước khi làm bất kỳ
việc gì liên quan tới đồ họa / rendering / URP / ánh sáng / camera:

- **BẮT BUỘC đọc `plan/02-ai-rules.md`** — rule an toàn cho task này.
- Kế hoạch chi tiết theo phase: `plan/01-hd2d-urp-migration-plan.md`.
- Cập nhật tiến độ vào: `plan/03-progress.md`.

Tóm tắt rule cứng: làm trên branch `feature/hd2d-urp` (không commit thẳng `main`); procedural-only
(không import asset ngoài); không trộn "đổi graphics" với "đổi nội dung" trong một commit; giữ
pixel-perfect; nghiệm thu đủ 5 map; HỎI trước khi cài package / đổi render pipeline / bắt đầu Phase D.
