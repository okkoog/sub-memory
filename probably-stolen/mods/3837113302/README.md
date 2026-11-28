# Snake Ranch 0.7.19.6 — 旧测试箱读档修复 / Legacy test case load fix

- 仅修复旧档 `snake_ranch.test_case` 的库存节点索引：把已确认旧布局的直属子物品从节点 `2` 迁移到当前节点 `1`，让原生读档重新挂接箱内物品。不改物品、实例 ID、数量、落点或箱内蛇笼自己的子库存索引。 / This hotfix only remaps the confirmed legacy `snake_ranch.test_case` direct-child inventory index from node `2` to current node `1`, so native loading can attach the test-kit contents again. Item references, instance IDs, counts, placement data and each cage's own child indices are unchanged.
- 其他容器、未知或混合布局不迁移；严格读档校验保留。 / Other containers and unknown or mixed layouts are not migrated; strict load validation remains.

# Snake Ranch 0.7.19.5 — 交配与检查员修复 / Mating and inspection fixes

- 双方平时照常进食，首夜至少半饱即可开始交配。蛇球展示持续到下一次夜结，亲蛇在完成夜各多消耗一份维持量。孕期挨饿仍会掉血。 / Both parents feed as usual and need to be at least half full to mate. The group stays visible until the next night, when each parent spends one extra maintenance ration. Starvation during gestation still costs health.
- 检查员的蛇提醒记住已选答案。若结果页被接管，等界面空出来后按原选项结算。两个提示的选项改用原版按钮图标。 / The inspection prompt keeps your answer through dialogue changes and resolves once the screen is free. Both prompts use the native choice icon.

单蛇在储备行显示昨夜新吃的肉量和维持量缺口。蛇笼最多显示一行笼内合计。仅在吃到公共肉食或维持量不足时出现。 / A snake's reserve line shows last night's new meat intake and any maintenance shortfall. The cage shows one summary line when snakes ate shared meat or came up short.

## 同笼分食 / Shared cage feeding

- 每条蛇先用自己的可用储备。笼内肉食按扣除当晚消耗后的相对饱食度分配，先给最饿的蛇补一晚维持量，余粮才用来补饱食和成长。仍在消化鼠餐的蛇只从公共食物补维持缺口。 / Snakes use their own available reserves first. Shared meat covers up to one night of maintenance per snake, starting with the lowest relative fullness after nightly costs. Any food left can replenish fullness or fund growth. A snake still digesting prey can draw only for maintenance.
- 投料机可按量分配。散肉按整件吃，超出当下所需的热量保存在该蛇账本里。笼内专用槽和格子里的投料机均可供肉。 / Dispensers supply measured portions. Loose meat is eaten in whole units, with excess calories kept in the snake's ledger. Dispensers in the dedicated slot and cage grid both supply meat.

## 已包含的更新 / Included updates

- **淬毒成品保留原物的出售类别。** 普通 .22 每发用 0.5 mL，基础价由 20 升到 40 信用点。单件近战用 1.0 mL，满涂层为原武器增加 45 信用点基础价，涂层消耗后价格恢复。成交价依买家和原版议价规则计算。 / Coated items keep the original trade category. A .22 round uses 0.5 mL and rises from 20 to 40 base credits. A full 1.0 mL coating adds 45 base credits to an individual melee weapon until the coating is spent. Actual payouts follow each buyer's trade rules.
- **.22 首个否决可诊断。** 弹药加工入口现在按一次操作记录 `spray-ammo-entry`、`spray-ammo-reject` 和 `spray-ammo-result`，包含源/目标实例 ID、堆叠数、真实 `MaxNumRemove()`、枪内毒量和首个失败条件；预览日志只记录首个否决，不做每帧刷屏。 / The ammo entry now logs one `spray-ammo-entry`, `spray-ammo-reject` and `spray-ammo-result` per action, including source/target instance IDs, stack size, the real `MaxNumRemove()`, gun venom and the first failed condition. Preview logging records the first rejection only, without per-frame spam.
- **单发原位替换。** 喷枪把可移除的普通 .22 换为同格的一发淬毒弹。批量数量受原弹移除上限、成品落位容量和枪内毒量限制。 / A removable plain .22 is replaced with one coated round in the same cell. Batch size depends on the native removal limit, destination capacity, and available venom.
- **保留原版不可移除判定。** `MayRemove()`、父库存移除锁、非自有容器和枪内毒量不足仍会拒绝；失败不消耗原弹，也不扣枪内毒液。 / Native rejection rules are preserved: `MayRemove()`, parent remove lock, non-owned container and insufficient gun venom still reject. A rejection consumes neither the original round nor gun venom.
- **近战武器用喷枪淬毒。** 装好普通眼镜蛇毒的喷枪可以直接拖到/点选近战武器：只接受自有、当前可用作近战的物品（`MELEE_WEAPON` 类型或 `MELEE_WEAPON_TAG`，含武器态蛇杖），每次 1.0 mL，写入原武器实例的持久化涂层。 / A loaded ordinary-cobra spray gun can target a melee weapon directly: only owned items currently usable as melee (`MELEE_WEAPON` type or `MELEE_WEAPON_TAG`, including a snake in weapon form), 1.0 mL per coat, stored on the exact weapon instance.
- **一次合格命中消耗一次。** 首个实际接受 `xeno_toxin_attack` 的敌方目标消耗涂层。空挥、未命中和毒效未成功应用时保留涂层。 / The coating is consumed when the first eligible enemy accepts `xeno_toxin_attack`. It remains on a miss or if no target accepts the effect.
- **重复涂抹有提示。** 已有涂层时再次拖枪会提示“已经淬毒，剩余 1 次”，枪内毒液不变；枪内不足 1.0 mL 或目标不可用时也会给出对应失败提示。 / A repeated coat reports "already coated, 1 hit remaining" and spends no venom; less than 1.0 mL or an invalid target also reports the specific failure.
- **涂层显示与形态。** 武器 tooltip 显示 `已淬毒 1.00/1.00 mL · 剩余 1 次命中`。蛇杖恢复蛇形后涂层处于不可触发状态，同一实例回到武器形态后继续可用；死亡或离开自有库存时清除。 / The weapon tooltip shows `Coated 1.00/1.00 mL; 1 hit remaining`. A snake weapon keeps the coating while in animal form but cannot trigger it; the same instance resumes use in weapon form. Death or leaving owned inventory clears it.
- **喷枪提示同步。** 枪的描述和 tooltip 同时说明 `.22`（0.5 mL/发）与近战淬毒（1.0 mL/次）两种用途。 / The gun description and tooltip now state both `.22` (0.5 mL per round) and melee coating (1.0 mL per use).

## 安装 / Install

1. 关闭游戏。
2. 保留现装版本备份，将包内的 `snake_ranch` 整个目录放到 `Probably Stolen/Mods/snake_ranch`。
3. 只使用专门的测试档；不要在真实存档上第一次试这个包。

## 范围 / Scope

此前的 0.7.19.1 已在游戏里验证 .22 点选、拖放及切肉刀涂层。本版交配蛇球的两夜流程、检查员对话、同笼分食和交易规则经过离线验证。Unity 中蛇球的实际显示、检查员弹窗、多蛇共笼夜结、柜台实收和近战命中仍待验证。 / The previous 0.7.19.1 build was playtested for .22 point-and-click and drag-and-drop, plus cleaver coating. The two-night mating lifecycle, inspection dialogue, shared feeding and trading have offline coverage. Live Unity mating visuals, the inspection prompt, shared feeding in the actual night cycle, in-game payouts and combat hits remain to be checked.
