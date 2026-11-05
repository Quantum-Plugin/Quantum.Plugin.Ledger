# Quantum.Plugin.Ledger

[English](../en/README.md)

提供收支记录、精确金额、关联线索和日历关联。

[架构](architecture.md) · [开发](contributing.md) · [术语表](glossary.md)

需要 .NET 10，依赖从公开 NuGet 源还原。已发布的 [SDK](glossary.md#sdk) 版本固定在 Directory.Packages.props，无需相邻项目源码。

```sh
dotnet restore Quantum.Plugin.Ledger.slnx
dotnet test Quantum.Plugin.Ledger.slnx -c Release
pwsh .ci/Build.ps1
```

构建工具在 artifacts 中生成经过验证的压缩包，解压后交给 Quantum 插件加载器；清单和 wwwroot 须与程序集一起保留。[CI](glossary.md#ci) 验证 Windows 和 Linux。

[Apache License 2.0](../../LICENSE) · [NOTICE](../../NOTICE)
