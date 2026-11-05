# 架构与契约

[English](../en/architecture.md)

源码按技术职责组织：Contract 放消息，Application 放用例，Domain 维护不变量，[UI](glossary.md#ui) 放页面与本地化。运行时适配器、初始化和数据库贡献器直接位于项目根目录。测试对应这些职责。

[NOF](glossary.md#nof) 生成 [RPC](glossary.md#rpc) 客户端与服务端。页面使用生成的本地客户端，跨插件请求通过 IQuantumRequester 调用命名能力。宿主负责路由、序列化、数据库生命周期与安装；宿主和跨插件集成测试由消费宿主管理。

| 标识 | 值 |
| --- | --- |
| 插件 | quantum.plugin.ledger |
| 程序集 | Quantum.LedgerPlugin |

维持既有 [CLR](glossary.md#clr) 名称、命名请求、路由组件与迁移顺序。数据库迁移由宿主执行，不修改已发布迁移；结构变更须新增编号迁移。公共契约定义于：

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
