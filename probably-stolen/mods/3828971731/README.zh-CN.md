# 势力声望扩展 v1.5.1

## 更新说明

修复预算，空物资箱和界面点击问题

## 安装

工坊订阅后等待下载完成，在主菜单“模组”中启用，确认文件读写、反射与动态代码能力，然后重启游戏。

本地安装：关闭游戏，将整个 `FactionReputationExpansion` 文件夹放入游戏根目录的 `Mods` 文件夹；清单应位于 `Mods/FactionReputationExpansion/manifest.xml`，DLL 与清单同级。正式版不需要 MelonLoader。不要同时加载旧版 DLL 或多个同 ID 副本；本地同 ID 副本会优先于工坊版本加载。

## 更新与转移

更新前备份原版存档及存档目录下的 `FactionReputationExpansion` 文件夹。退出游戏后替换发布文件，或等待 Steam 下载工坊更新；保留存档和侧文件夹。自行添加的语言文件也请备份并保留，翻译方法见 `LOCALIZATION.md`。

扩展进度同时保存在原生存档的 MOD 数据包与侧文件夹中。换机前，两台电脑均更新 MOD，在保有正确进度的旧电脑读档并正常保存，退出后等待 Steam 云同步完成，再在新电脑读档。旧版存档尚未用新版保存时，需同时复制原版存档与侧文件夹；仅复制 MOD 文件无法迁移进度。

默认存档目录：`%USERPROFILE%/AppData/LocalLow/Questing Goose Studio/Probably Stolen`。

## 卸载

启用 MOD 时正常保存并退出，再停用 MOD 并重启游戏。本地安装可删除 `Mods/FactionReputationExpansion`，工坊安装可取消订阅。

原版声望按原生范围保存；卸载后扩展效果、联络人、委托和未完成交付停止生效。已正常获得的原生物品与金钱保留。原版忽略存档中的 MOD 数据包；删除侧文件夹并不等于清除原生存档中保存的扩展记录。
