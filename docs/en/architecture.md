# Architecture and contracts

[简体中文](../zh-CN/architecture.md)

Source uses technical responsibilities: Contract for messages, Application for use cases, Domain for invariants and [UI](glossary.md#ui) for pages/localization. Runtime adapters, initialization and database contributors stay at the project root. Tests mirror these responsibilities.

[NOF](glossary.md#nof) generates [RPC](glossary.md#rpc) clients and servers. Pages use local generated clients. Cross-plugin requests use IQuantumRequester and named capabilities. The host owns routing, serialization, database lifetime and installation. Host and cross-plugin integration tests belong to the consuming host.

| Identity | Value |
| --- | --- |
| Plugin | quantum.plugin.ledger |
| Assembly | Quantum.LedgerPlugin |

Preserve existing [CLR](glossary.md#clr) names, named requests, route components and migration ordering. The host executes database migrations; never edit a released migration. Add a new numbered migration for new schema changes. Public contracts are defined by:

- [ILedgerRpcService.cs](../../src/Quantum.LedgerPlugin/Contract/ILedgerRpcService.cs)
- [EntryKind.cs](../../src/Quantum.LedgerPlugin/Contract/Models/EntryKind.cs)
- [LedgerCategoryTotal.cs](../../src/Quantum.LedgerPlugin/Contract/Models/LedgerCategoryTotal.cs)
- [LedgerEntryDetails.cs](../../src/Quantum.LedgerPlugin/Contract/Models/LedgerEntryDetails.cs)
- [LedgerEntryLookup.cs](../../src/Quantum.LedgerPlugin/Contract/Models/LedgerEntryLookup.cs)
- [LedgerMonthSummary.cs](../../src/Quantum.LedgerPlugin/Contract/Models/LedgerMonthSummary.cs)
- [CalendarLedgerRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/CalendarLedgerRequest.cs)
- [CreateMobileLedgerEntryRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/CreateMobileLedgerEntryRequest.cs)
- [LedgerEntryRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/LedgerEntryRequest.cs)
- [LedgerMonthRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/LedgerMonthRequest.cs)
- [RefreshLedgerMobilePageRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/RefreshLedgerMobilePageRequest.cs)
- [SaveLedgerEntryRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/SaveLedgerEntryRequest.cs)
- [UpdateLedgerEntryRequest.cs](../../src/Quantum.LedgerPlugin/Contract/Requests/UpdateLedgerEntryRequest.cs)
