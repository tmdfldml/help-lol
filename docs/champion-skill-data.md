# Champion Skill Reference & Skill Order — Stage 10-E

Reviewed 2026-10-03 (KST). This feature is optional local reference content, independent of matchup statistics. No player identity is stored or logged. Existing statistics collectors, updater, aggregates and classifiers are unchanged.

## Official source and generation

- [Riot Data Dragon documentation](https://developer.riotgames.com/docs/lol#data-dragon): official static champion data and assets.
- Pinned source: [16.17.1 Korean championFull.json](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json), matching the existing champion metadata. The versions endpoint currently lists 16.19.1; this release deliberately retains 16.17.1 and labels it rather than implying current-game accuracy.
- Actual fields verified: `data[id].spells` (four slots, Q/W/E/R), `name`, `description`, `cooldown[]`, `maxrank`, and `image.{sprite,x,y,w,h}`. Passive and templated damage/cost/range tooltips are not imported.
- Riot's short Korean `description` is HTML-decoded and tags/extra whitespace removed. No generated gameplay summary, damage calculation or inferred facts. Some complex champions have longer descriptions; they wrap and scroll.
- Developer-only .NET 10 Windows console project: `Tools/ChampionSkillDataCollector/`. Uses built-in HttpClient, JSON and WPF PNG cropping; no NuGet dependencies or API key.
- One bulk JSON plus 19 sprite files generated all 173 metadata champions and 692 local PNGs. Cache is under ignored `artifacts/skill-source-cache/<version>/`; requests are sequential, at least 400 ms apart, cached, with no automatic retry. A failed request stops generation; rerun resumes cache. Raw downloads never ship.
- Final content is under `Data/ChampionSkills/`. Only that final JSON/assets directory is copied by the app project. Tools are neither compiled into the app nor published.

```powershell
dotnet run --project Tools/ChampionSkillDataCollector -- C:\Projects\LoLMatchup 16.17.1
dotnet build LoLMatchup.csproj
dotnet run --project Tools/RegressionScratch -- --skills-only
dotnet run --project Tools/RegressionScratch -- --skill-ui-only
```

For a new patch, select an explicitly available Data Dragon version, review supported champion metadata separately, run the generator, inspect its manifest/unknown cooldown list, run validation and UI checks, then use the existing release pipeline. Generation writes champion JSON after every supported champion has been checked; review the entire generated directory before packaging. It is a developer operation, not an app updater.

## Data contract and failure behavior

One file named `<stable English ChampionId>.json` contains `schemaVersion`, `championId`, localized `championName`, full `patch`, `source`, and `spells[]`. Schema 2 adds a string-serialized `SpellCooldownType` enum, nullable `rechargeTimes[]`, nullable `maxCharges`, `mechanicNote`, `sourceEvidence`, and optional `baseCooldowns[]` for conditional charge abilities. `cooldowns[]` means the cast-to-cast timer; it must never be copied into `rechargeTimes[]`. Four spell objects retain `key`, `name`, untouched short Riot `description`, and local icon filename. The loader also accepts legacy schema 1. Examples of join keys: `LeeSin`, `MonkeyKing`; names remain Korean.

The generator rejects duplicate metadata IDs and missing champions. The loader requires the file's ID to match the requested ID, a supported schema, a valid patch and official source. Each Q/W/E/R slot must be unique; invalid, duplicated or missing spells become per-slot fallbacks. Cooldowns must contain 1–6 finite positive values; the generator additionally checks the source `maxrank`. Numeric values are displayed in source rank order with invariant decimal formatting, including actual three-rank ultimates. Haste and current remaining cooldown are never calculated.

The initial 21 unavailable arrays were audited against a fresh official 16.17.1 source download; see [the complete cooldown audit](cooldown-audit-16.17.1.md). One actual data bug was fixed: Urgot W's `[12,9,6,3,0]` had been discarded because rank 5 is zero. The generator and loader now permit this exact champion/slot/version and five-rank positive/zero shape, with the source tooltip confirming the max-rank toggle. The UI shows all five source numbers and “5랭크: 토글 전환”. Other zero/negative arrays remain rejected by the loader. No blanket zero acceptance was introduced.

The previous audit had 20 null timers, including 3 UNVERIFIED spells; that count is historical. The 2026-10-04 mechanic audit classifies all 692 slots: Standard 653 (including hybrid Urgot W), Charge 19 (including conditional Syndra Q), Toggle 4, ResourceBased 3, Passive 4, NotApplicable 1, Unknown 8. There are 24 null cast-cooldown arrays across these categories; a null cast timer is not an unknown mechanic. `manifest.unavailableCooldowns` lists the 24 null cast arrays; `unknownCooldownMechanics` separately lists 8 Unknown slots and `unavailableRechargeTimes` lists 19 unresolved recharge timers. Base-form Data Dragon QWER remains an incomplete reference for transformation subspells.

### Special cooldown rules — reviewed 2026-10-04 KST

- **Standard cooldown:** preserve exact positive `cooldown[]` rank values; display “재사용 대기시간”. Urgot W alone retains `[12,9,6,3,0]` and “5랭크: 토글 전환”, based on its source tooltip and pinned rank shape. Never display a standalone all-zero timer.
- **Charge recharge time:** scan every spell's actual `maxammo`, description and tooltip storage/recharge references. Channel charging (`<charge>`), recasts and ordinary stacks are not stored-cast charges. Exclude ambiguous `maxammo` candidates Karthus Q, Leblanc R, Warwick W and Rengar Q/W/E with explicit source-scoped non-charge overrides (Karthus Q remains Unknown; positive cooldowns for the other five are preserved). Positive `cooldown[]` belongs to the separate cast row, not the recharge row. Unknown cast timers keep a visible “정보 없음” row. The separate mechanic label says “충전형 스킬”.
- **Source limitation:** actual 16.17.1 spell fields are `cooldown`, `cooldownBurn`, `cost`, `costBurn`, `costType`, `datavalues`, `description`, `effect`, `effectBurn`, `id`, `image`, `leveltip`, `maxammo`, `maxrank`, `name`, `range`, `rangeBurn`, `resource`, `tooltip`, `vars`. There is no resolved `rechargeTime`/`ammoRechargeTime` field. All `datavalues` objects and `vars` arrays are empty. Variables such as `ammorechargetime`, `ammorechargeratetooltip`, `ammorechargetooltip`, `chargecooldown`, `maximumcharges`, `maxkits` remain unresolved tooltip placeholders. **All 19 imported recharge arrays remain null.** No community-derived timer is imported. The official same-patch English document was checked for terminology; it also leaves these values unresolved.
- **Maximum charges:** display a number only when the actual tooltip supplies a literal maximum. The raw scalar `maxammo` is retained in the developer audit but is insufficient by itself to establish an all-rank maximum. Dynamic/rank-dependent maximum templates and uncorroborated scalars stay null, including Gangplank E. No maximum is inferred from the number of deployed traps/turrets.
- **Conditional charge:** Syndra Q's source describes storing charges after evolution. Mark Charge with that qualification. Preserve its `[7,7,7,7,7]` separately as `baseCooldowns`, shown under “진화 전 재사용 대기시간”; neither the evolved cast interval nor recharge time is inferred from 7.
- **Toggle:** source `<toggle>` identifies Amumu W, Anivia R, Karthus E and Singed Q. Display “토글 스킬”; source switch delays are not presented as general spell cooldowns. Urgot W's max-rank hybrid exception takes precedence.
- **ResourceBased:** source-specific overrides for Shyvana R (“분노 기반”), Ashe Q (“기본 공격 중첩 기반”), Yunara Q (“방출 중첩 기반”). Each tooltip explicitly generates/consumes the prerequisite resource. Mana costs alone do not classify a spell as ResourceBased.
- **Passive:** a zero-timer, passive-only tooltip without an active/toggle section identifies Teemo E, TwistedFate E, Vayne W, Vi W. Display “해당 없음 · 지속 효과”. A spell containing both active and passive effects is not automatically Passive.
- **NotApplicable:** Aphelios E's description explicitly states that there is no third skill. Display “해당 없음”. This does not remove other active weapon slots; Data Dragon's base QWER does not enumerate every weapon variant.
- **Unknown:** display “정보 없음”. Do not convert an all-zero array to “0초”, force a charge interpretation from ambiguous `maxammo`, or claim resolved timing. Current 8-slot list: Kalista E, Karthus Q, Rakan E, TahmKench R, Talon E, Veigar W, Yuumi W, Zeri Q.

Full 19-entry charge list with maximum, separate timers and field-path evidence: [charge-skills-16.17.1.md](charge-skills-16.17.1.md). Gangplank E's mechanic is now identified as a `maxammo` charge candidate; numerical fidelity remains UNVERIFIED. Karthus Q and Zeri Q remain UNVERIFIED. This resolves presentation ambiguity, not the missing source numbers.

Validation commands additionally include `powershell.exe -NoProfile -ExecutionPolicy Bypass -File Tools/RegressionScratch/verify-skill-mechanics.ps1` and `dotnet run --project Tools/RegressionScratch -- --skill-shell-ui-only`. The first checks every generated slot against the pinned source and generates the complete charge table. The latter tests the compiled MainWindow shell with the real skill view, excluding real-game polling and settings persistence. Production statistics and navigation behavior are not changed by the tests.

The service does no I/O at construction, lazily reads one champion on demand, and caches results (including failure) for the session. Logs contain fixed success/missing/malformed events without champion/player payloads. Malformed/missing JSON cannot make startup fatal. Icons accept local PNG basenames only; missing/invalid images keep text readable. Patching files requires an app restart to clear the cache.

## Skill Order source review and implementation gate

Sources inspected on 2026-10-03:

1. [OP.GG data-use help](https://help.op.gg/hc/en-us/articles/31091405109401-Can-I-use-OP-GG-data), updated 2025-12-31: describes general crawling permission with attribution and service-load conditions, and says game data is not supplied to third parties.
2. [OP.GG Terms of Use](https://op.gg/lol/policies/agreement), marked updated 2026-09-14: prohibits scraping/data mining and automated collection, and restricts copying/distribution of site content without permission.
3. [robots.txt](https://op.gg/robots.txt): wildcard allows paths; specific agents have additional rules. This does not settle reuse permission or override the terms.
4. [Akali MID build page](https://op.gg/lol/champions/akali/build/mid): a SkillOrder table presents skill priority, a representative level sequence, pick rate, games and win rate. This was inspected for schema only; no values were imported into the product. Page HTML is not a documented stable third-party API contract.

**Gate not passed:** the public guidance conflicts with the terms, and stable third-party API/republication terms were not established. No OP.GG scraper, API client, bulk requests or live app requests were implemented. Seek explicit provider permission/contract before revisiting. An alternative for a later stage is an independently scoped Riot timeline `SKILL_LEVEL_UP` aggregation; that would require validation and changes outside this task's protected statistics collector scope.

`orders.json` ships as `[]`. Runtime lookup is exact `(ChampionId, Role, Patch)` with the existing five standard role constants; no role/patch fallback. The reserved model has `source`, `collectedAtUtc`, `priority`, optional `levelUpSequence`, `sampleSize`, `pickRate`, `winRate` (rates are 0–1). Duplicate keys are rejected rather than arbitrarily choosing one. Priority contains each of Q/W/E once. Level sequences are optional 1–18 QWER entries. Real sources need a separate champion-specific sequence validation before import; generic structural checks do not prove a build is playable.

UI can show priority, sequence and source attribution when an authorized dataset exists. Synthetic values occur only in temporary test directories. Without a record, the normal UI says “이 역할의 스킬 순서 통계를 아직 사용할 수 없습니다.” QWER remains usable. No OP.GG endorsement or data availability is implied.

## Navigation, patch and distribution

InGame enemy portrait uses a dedicated button whose activation executes only the enemy command, suppressing bubbling Click to its role-card parent. Existing role card behavior remains Matchup Detail. The shared app navigation has a separate skill page/context, Back and Escape return to InGame, and a disappeared opponent/game end clears the page. Role comes from the current matchup; no re-inference. Enemy identity uses the existing purple theme.

The detail header shows champion icon, Korean name, role, exact data patch and Riot attribution. QWER cards have icons, descriptions and prominent cyan base cooldowns. The single-column vertical scroller works in portrait and landscape; narrow windows are the priority. `AppDiagnosticsService.Compare` supplies a nonblocking mismatch warning. Production currently lacks a verified game-patch source, so unknown game patch must not be misrepresented as a match.

The existing Riot legal notice is retained in About. External Skill Order attribution requirements remain unresolved with the data-use gate. `validate-release.ps1` now requires every champion skill file and QWER icon and rejects the new tool EXE/DLL, alongside existing dev/privacy scans. Installer wildcard packaging includes the same validated final assets; no tool/cache/source is packaged.

Stage 10-E QWER reference and order extension/fallback are implemented. Populated real Skill Priority is deferred by the requested gate. Actual League game acceptance, transformed/champion-specific static-data fidelity, every DPI/screen reader, signing and a new installer install/uninstall lifecycle remain separate acceptance work.
