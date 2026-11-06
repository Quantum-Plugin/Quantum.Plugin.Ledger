# Releases

[简体中文](../zh-CN/releases.md) · [Project guide](README.md)

## 0.7.0

- Read calendar events and tasks through `calendar.entry.list` and `calendar.entry.get`, replacing `calendar.list/get`.
- Group repeating occurrences by series ID within each provider when choosing calendar associations. Keep IDs returned by separate conflicting providers ambiguous.
- Build against published `Quantum.Plugin.Abstraction` **0.3.0-rc.3**. Publish with the host SDK compatibility range `--sdk-version-support ">=0.3.0-rc.2 <0.4.0"` using a verified CLI supporting that option (0.2.0 or later).

This pre-1.0 minor release changes the optional calendar integration contract: use it with Calendar 4.0.0. Upgrade both plugins together. Ordinary ledger operations remain usable without a calendar provider. The ledger schema, stored amounts, association IDs, public ledger capabilities, and authentication/authorization behavior are unchanged.

For rollback, restore Ledger 0.6.0 together with Calendar 3.0.0 and matching consumer versions, preserving existing application data. Publish only the immutable CI-verified archive; signed release tags identify merged history. Release notes link the PR, source commit, CI run and archive checksums. Run the [development checks](contributing.md) before submitting the release candidate.
