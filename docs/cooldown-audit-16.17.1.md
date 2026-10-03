# Stage 10-E — 21 unavailable cooldown audit

Verified 2026-10-03 (KST). Scope: the original 21 `manifest.unavailableCooldowns` only, plus full-data regression after the one confirmed correction. This supersedes earlier statements that all 21 merely reflected source limitations.

## Source and method

Read `PROJECT_STATUS.md`, `docs/regression-test-report.md` and `docs/champion-skill-data.md`, then extracted the exact inventory from the pre-correction manifest. All 21 generated cooldowns were `null` before this audit.

Downloaded the official [16.17.1 ko_KR championFull.json](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) afresh. Its SHA-256 matches the generator's cached source:

`74FBB8EC6181B139A4EF556F791BF0E865C86AECD858D596FD3B6EDD0E1B0D49`

Compared `data[championId].spells[index]` with generated Q/W/E/R in order (indexes 0/1/2/3), names and local icon keys. Compared `maxrank`, `cooldown[]`, and `cooldownBurn`; a scalar burn is expanded across its ranks for comparison. Descriptions/tooltips from this same version supply the mechanism evidence below. Unresolved tooltip placeholders were never treated as numbers. No current-patch substitution or third-party gameplay values were used.

Reproduction evidence (ignored local artifacts): `artifacts/cooldown-audit-source-16.17.1.json`, `artifacts/cooldown-audit/before-manifest.json`, `before-hashes.json`, and `results.json`. The results include original spell IDs, slot indexes, ranks, arrays, burn and source descriptions/tooltips. Run `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/RegressionScratch/verify-cooldown-source.ps1` after obtaining those inputs. The script checks the original inventory and the full 173/692 dataset; mechanism classification is the reviewed judgment below, not an automatic inference from zero.

## Item-by-item findings

Legend: N = NORMAL_SPECIAL_CASE; D = DISPLAY_IMPROVEMENT_NEEDED; B = DATA_BUG; U = UNVERIFIED. `0 × n` means an actual n-element all-zero array. For every all-zero row, `cooldownBurn` is `"0"`, and generated cooldown remains `null` after the audit. Only Urgot W changes.

| Champion / key / spell | maxrank | Source cooldown[] | Class | Same-version source evidence and limit |
|---|---:|---|---|---|
| Aphelios E — 무기 교체 시스템 | 6 | 0 × 6 | N | Description identifies a next-weapon indicator instead of a third usable spell. Six source ranks preserved in audit; not mapped to a different ability. |
| Ashe Q — 궁사의 집중 | 5 | 0 × 5 | N | Attack-generated concentration gates activation and is consumed. No ordinary rank timer can be recovered from the cooldown field. |
| Bard W — 수호자의 성소 | 5 | 0 × 5 | D | Tooltip explicitly mentions stored charges (`ammo_limit`); recharge seconds unresolved. Shrine healing growth time is not a recharge cooldown. |
| Gangplank E — 화약통 | 5 | 0 × 5 | U | Barrel duration/decay/explosion text does not resolve the zero cooldown field or a recharge timer. Do not import a charge timer from memory. |
| Kalista E — 뽑아 찢기 | 5 | 0 × 5 | D | Tooltip says a kill resets cooldown. This confirms a reset condition, not the missing base timer or literal zero seconds. |
| Karthus Q — 황폐화 | 5 | 0 × 5 | U | Damage/explosion text does not explain the all-zero cooldown or supply a verified replacement. |
| Rakan E — 전쟁무도 | 5 | 0 × 5 | D | A second free cast within `recastwindow` is documented. Recast window and primary cooldown must not be conflated. |
| Shyvana R — 용의 강림 | 3 | 0 × 3 | N | Tooltip describes fury generation, dragon transformation, continuous fury expenditure and ending at zero. No inferred time-to-transform value. |
| Singed Q — 맹독의 자취 | 5 | 0 × 5 | N | Tooltip explicitly marks activation/deactivation as a toggle. No fabricated toggle lockout. |
| TahmKench R — 집어삼키기 | 3 | 0 × 3 | D | Ally/enemy branches and a recast to spit out are documented. Neither branch gives a resolved base cooldown here. |
| Talon E — 암살자의 길 | 5 | 0 × 5 | D | Tooltip distinguishes short skill cooldown from the same-terrain cooldown (`wallcd`). Neither may be replaced with literal zero. |
| Teemo E — 맹독 다트 | 5 | 0 × 5 | N | Tooltip marks passive on-hit poison. |
| TwistedFate E — 속임수 덱 | 5 | 0 × 5 | N | Passive attack effect, every fourth attack; not a conventional activation timer. |
| Urgot W — 심판의 원 | 5 | 12 / 9 / 6 / 3 / 0 | B | Burn is `"12/9/6/3/0"`, matching the array. Tooltip explicitly changes max rank to a toggle. Dropping ranks 1–4 was a parser/validation bug. |
| Vayne W — 은화살 | 5 | 0 × 5 | N | Passive third-hit effect. |
| Veigar W — 암흑 물질 | 5 | 0 × 5 | D | Tooltip describes stack-dependent cooldown reduction, without resolving its base timer. |
| Vi W — 찌그러뜨리기 | 5 | 0 × 5 | N | Passive third-hit effect. The shield cooldown reduction mentioned elsewhere in this tooltip is not this spell's own cooldown. |
| Yunara Q — 영혼 단련 | 5 | 0 × 5 | N | Attack-generated resource is consumed for activation; transformed state auto-activates it. No resource-to-seconds conversion inferred. |
| Yuumi W — 너랑 유미랑! | 5 | 0 × 5 | D | Immobilization imposes `ccattachlockout`; the placeholder is not a resolved number. |
| Zeri Q — 집중 사격 | 5 | 0 × 5 | U | Described as a basic attack, but this source does not provide a cooldown scaling formula that explains the zero. |
| Zyra W — 맹렬한 성장 | 5 | 0 × 5 | D | Two charges and `ammorechargetime`, plus kill-based recharge reduction, are documented. Recharge seconds remain unresolved. |

Totals: **21 = N 9 + D 8 + B 1 + U 3**. Classification B records the defect found, even though corrected. D means the conservative missing-number display is valid but lacks explanatory context; it does not certify a zero-second gameplay value. N confirms a nonstandard mechanism, not comprehensive subspell/lockout coverage. U remains unresolved despite verifying what the source field contains.

## Minimal correction and regeneration

- Fixed only Urgot W: preserve source `[12,9,6,3,0]`, append `5랭크: 토글 전환` in its cooldown presentation. Generator exception is limited to Urgot/W/16.17.1/five ranks/positive ranks 1–4/zero rank 5 plus a toggle marker in the source tooltip. Loader has the same champion/slot/version/shape boundary and supplies the audited note rather than trusting arbitrary note text.
- No other zero, negative or absent value is converted to a numerical guess. No new gameplay functionality, dependency or statistical metric.
- Ran the full collector using the hash-verified cache. Hash comparison against the pre-audit directory changed **only `Urgot.json` and `manifest.json`**; other 172 champion JSONs, 692 icons and orders are unchanged. Unavailable count: 21 → 20.
- Added a regression assertion for all five Urgot ranks and the explanatory note.

## Full-data verification

| Check | Result |
|---|---|
| Champions / spells / missing champion | **173 / 692 / 0** |
| QWER mapping / source-rank count / retained numeric value errors | **0 / 0 / 0** |
| Structurally malformed generated cooldowns | **0** |
| Numeric generated arrays / conservative null arrays | **672 / 20** |
| Source all-zero / mixed-zero arrays | **20 / 1** (mixed-zero is corrected Urgot W) |
| Negative arrays, including -1 / empty arrays | **0 / 0** |
| Identical value repeated across all ranks | **197** (includes 20 zero arrays); repetition is valid and not collapsed |
| Original 21 burn-vs-array comparisons | **21 match**, including rank counts |
| Debug / Release app build | **PASS**, each 0 warnings / 0 errors |
| `--skills-only` regression | **PASS**, coverage, Urgot fix, fallback, safety, caching and role/patch separation |

Structural malformed = 0 does **not** mean the 20 null cooldowns are complete verified timers: 3 remain UNVERIFIED, and 8 need explanatory display context. Transformation audit concerns the source's four base slots only; no transformed/subspell completeness claim.

## Actual UI verification

Opened the freshly built Debug desktop app and used its existing Developer Test Mode. Opened each enemy portrait into the production skill detail page and visually inspected the rendered rows:

| Case | Observed | Result |
|---|---|---|
| Urgot W, TOP | `12 / 9 / 6 / 3 / 0초` followed by `5랭크: 토글 전환`; all five ranks and note visible | PASS |
| Singed Q, JUNGLE | `정보 없음`; no false zero-second value | PASS |
| Vi W, JUNGLE | `정보 없음`; no false zero-second value | PASS |

Three actual UI cases inspected at the current desktop DPI and window size. Urgot and Singed used custom test opponents; Vi used normal 5v5. This is native app UI validation with fixtures, not a real League match or exhaustive DPI test. Picker automation timeouts were re-observed; attempted inputs were not counted as successful verification.

## Stage 11 RC decision

**Conditional only; full cooldown accuracy acceptance is not closed.** The one proven application data bug is fixed, full structural regression and three actual UI checks pass. Keep 3 UNVERIFIED items and the conservative fallback documented; resolve or explicitly accept those limitations before an unconditional data-accuracy sign-off. Existing real-game/DPI/Skill Order source and release acceptance gates remain.

This audit rebuilt Debug/Release application outputs and regenerated source data. It did **not** rebuild the prior portable ZIP/installer/publish artifacts or install them. Earlier release hashes and package results describe the pre-correction package; regenerate through the release pipeline before distributing this fix.
