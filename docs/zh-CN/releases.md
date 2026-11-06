# 版本说明

[English](../en/releases.md) · [项目说明](README.md)

## 0.7.0

- 通过 `calendar.entry.list` 和 `calendar.entry.get` 读取日程与待办，替换原 `calendar.list/get`。
- 选择日历关联时，同一提供方的重复实例按系列 ID 合并；不同提供方之间冲突的 ID 仍视为歧义。
- 依赖已发布的 `Quantum.Plugin.Abstraction` **0.3.0-rc.3**，通过已验证且支持 SDK 兼容选项的 CLI（0.2.0 或更高版本），使用宿主 SDK 兼容范围 `--sdk-version-support ">=0.3.0-rc.2 <0.4.0"` 发布。

此 1.0 前的次版本更新改变可选日历联动契约，需与 Calendar 4.0.0 同步升级。没有日历提供方时，普通记账操作仍可使用。账本结构、已存金额、关联 ID、既有记账能力及身份认证／权限行为保持不变。

回退时同时恢复 Ledger 0.6.0、Calendar 3.0.0 和匹配的消费方，保留应用数据。仅发布持续集成验证过的不可变安装包；签名版本标签对应已合并历史，发布说明关联 PR、源码提交、持续集成运行和校验值。提交候选版本前执行[开发检查](contributing.md)。
