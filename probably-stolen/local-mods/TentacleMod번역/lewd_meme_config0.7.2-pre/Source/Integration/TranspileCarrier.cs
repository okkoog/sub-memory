using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// transpiler 载体(0.7.0 Task 3,本 mod 首个 Harmony transpiler 形态的
    /// 执行侧;数据面 = <see cref="TranspilePlan"/>,装配方 = Task 4-8 各迁移
    /// 单元的 install 委托)。依赖游戏/Harmony 类型,不进测试工程——其正确性
    /// 由游戏内三级自检与「任一点未命中 → Dead」的失败显式化兜底。
    ///
    /// 用法(install 委托内,与 prefix/postfix 同一 Harmony 实例):
    /// <code>
    /// MethodBase original = ModReflect.Target(...);
    /// h.Patch(original, transpiler: TranspileCarrier.Transpiler);
    /// return new[] { original };
    /// </code>
    /// Run 按 original 反查 <see cref="TranspilePlan.MethodIdsFor"/>("TypeName::
    /// MethodName")——同一 original 可挂多个单元的 transpiler(NightlyTick =
    /// 数值面 + 乳汁面双单元先例,替换点不相交,叠加安全;Router 裁非 Own 的
    /// methodId 整组跳过——共享 original 的让位语义[乳汁面让 Brood]不随兄弟
    /// 单元装配连带生效,T4);不认识的方法
    /// (表内无此目标)抛 <see cref="TranspileMatchException"/> = 装配期即拒,
    /// 不静默装一个什么都不改的 transpiler(空 patch 也有重编译成本与
    /// 卸载记账负担)。
    ///
    /// 匹配器语义(锚表达设计,controller 裁决 1):每点 = Match 段(要整体
    /// 替换的连续指令)+ Before / After 窗口(紧邻上下文,内容锚定)。
    /// 一律按「操作码 + 操作数 + 前后窗口」内容匹配,禁按 IL_XXXX 偏移匹配
    /// (T2 铁律:偏移只对 1.9.2 这一份 DLL 有效)。命中次数必须恰好等于
    /// Point.Count(默认 1;corrupt_cost 扣减 5 处 / printer.grid 6 处等按
    /// Count 锁量)——多/少都抛异常,由 HookInstaller 记 Dead:版本漂移时
    /// 该单元整体拒绝,绝不静默半改(半改的数值方法比原版更糟)。
    /// 勘察注记(Fix Round 1 C-1):新注入点勘察时必须检查 IL dump 的
    /// 跳转目标是否指向被替换段——1.9.2 已实证 4 单元 5 点的分支目标恰为
    /// Match 指令本身(blt.s/brfalse.s/brtrue.s 直落 ldc),替换时需迁移
    /// labels(见 ReplaceAt),否则 Harmony 重建 IL 抛 Bad label content
    /// 而装配即 Dead——这是本缺陷的发现路径,也是未来扩表的自检项。
    ///
    /// 零能力红线:只用 HarmonyLib 公开类型(CodeInstruction / HarmonyMethod /
    /// AccessTools)与 System.Reflection.Emit 的 OpCode/ILGenerator 常量载体
    /// (transpiler 标准签名),无反射原语、无动态代码;成员/局部匹配是读取
    /// Harmony 已解码的 operand 对象做相等比较,不发起任何查找。
    /// </summary>
    public static class TranspileCarrier
    {
        /// <summary>
        /// transpiler 入口的 HarmonyMethod 包装(静态只读,Task 4-8 装配委托
        /// 直接传给 h.Patch 的 transpiler: 参数)。
        /// </summary>
        public static readonly HarmonyMethod Transpiler =
            new HarmonyMethod(AccessTools.Method(typeof(TranspileCarrier), nameof(Run)));

        /// <summary>
        /// transpiler 的幂等挂载(0.7.0 Task 7 起的公共装配形态,NightlyPatch/
        /// CorruptPatch 内联先例收敛于此):先撤自己 id 在该 original 上的全部
        /// transpiler 再重挂——HookInstaller 装机侧逐单元调 install 委托,
        /// 同一 original 的多单元各调一次,Harmony 对「同 owner 同 transpiler
        /// 方法重复 Patch」的叠加语义无文档保证(若不去重则 transpiler 在已
        /// 替换流上二次匹配必失败 → 假 Dead),先撤后挂把「Run 每次从原始 IL
        /// 单遍应用」变成自己的性质(12/10 规则:Unpatch 传自己的 id,合法)。
        /// 卸载侧归 HookInstaller 方法级引用计数(全组卸载才 Unpatch,
        /// tearl/CustomCustomer.Create 先例);组卸载后重开,本方法照常重挂
        /// (幂等语义对重装成立)。
        /// </summary>
        public static void PatchTranspiler(Harmony h, MethodBase original)
        {
            h.Unpatch(original, HarmonyPatchType.Transpiler, h.Id);
            h.Patch(original, transpiler: Transpiler);
        }

        /// <summary>
        /// transpiler 本体(Harmony 注入 instructions/generator/original)。
        /// generator 按签名收下但当前无 label/局部分配需求(替换指令不含
        /// 跳转,也不需 DeclareLocal——T7 的 ldloc 特形直接替换隐含槽位的
        /// 短形指令,不引入新局部),保留参数 = 与 Harmony 标准签名一致。
        /// methodId 过滤(0.7.0 Task 4):同一 original 反查到的多个单元让位语义
        /// 可能不同(NightlyTick = 数值面 Own × 乳汁面让 Brood 的组合真实存在)——
        /// Router 裁非 Own 的 methodId 整组跳过(其注入点保持原版常量,语义等价
        /// 于「让位 = 无载体」的物理零 hook);快照缺席(未装配的理论时序/防御)
        /// 或表内 id 不在目录时保守全应用(Task 3 语义;printer 三 methodId 与
        /// 单元 id main.printer 不同名即走此通道——恒 Own 无让位,行为等价)。
        /// </summary>
        public static IEnumerable<CodeInstruction> Run(
            IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            string target = original.DeclaringType == null
                ? original.Name
                : original.DeclaringType.Name + "::" + original.Name;
            string[] methodIds = TranspilePlan.MethodIdsFor(target);
            if (methodIds.Length == 0)
            {
                throw new TranspileMatchException(
                    "transpiler 대상 메서드가 삽입 지점 목록에 없음: " + target);
            }
            var code = new List<CodeInstruction>(instructions);
            foreach (string methodId in methodIds)
            {
                // 让位/压制过滤(T4):route 快照 = Compose ⓪ 的 Router.DecideAll,
                // 装配期与热装卸期的每次 Patch 求值都会经过这里(会话内静态,
                // 与登记期 ③ 的过滤同一数据源,零偏差)
                IReadOnlyDictionary<string, RouteDecision> route = HookRuntime.RouteSnapshot;
                if (route != null
                    && route.TryGetValue(methodId, out RouteDecision decision)
                    && decision != RouteDecision.Own)
                {
                    continue;
                }
                ApplyMethod(code, methodId);
            }
            return code;
        }

        // ---- 单单元应用:两遍式(Fix Round 1 C3) ----
        // 第一遍(纯匹配,对原始流统一求值):逐条目逐注入点滑窗记位,不替换;
        // 每点命中数必须恰等于 Point.Count(缺失/多出即抛,整单元拒绝)。
        // 第二遍(统一替换):全部点核对通过后,按记录位从后往前替换——
        // 降序保证 RemoveRange/InsertRange 只影响更后的位置,先点的注入段
        // 不可能吞吃后点的 Before/After 窗口。单遍就地替换的两个失败模式
        // (实测复审):① 同键双点(alarm/trap_grids 的 (N,N) 双参:Point1
        // 替换后 Point2 的 Before=[..., ldc.i4.N] 被 [ldstr; call] 段吞吃);
        // ② 跨键窗口互指(attr_*_range High 点 Before=Low 点目标、
        // semen_chance_base Before=cap 目标)——单遍时结果依赖表序,表序
        // 一换即 Dead。两遍式把窗口求值全部固定在原流上,与表序彻底无关。
        private static void ApplyMethod(List<CodeInstruction> code, string methodId)
        {
            // 第一遍:记位(点位, 点, 键)
            var hits = new List<(int Index, TranspilePoint Point, string Key)>();
            foreach (TranspileEntry entry in TranspilePlan.Points(methodId))
            {
                foreach (TranspilePoint point in entry.Points)
                {
                    int count = 0;
                    for (int i = 0; i + point.Match.Length <= code.Count; i++)
                    {
                        if (MatchesAt(code, i, point))
                        {
                            hits.Add((i, point, entry.Key));
                            count++;
                        }
                    }
                    if (count != point.Count)
                    {
                        throw new TranspileMatchException(
                            "transpiler 삽입 지점 일치 횟수가 예상과 다름: " + entry.Key
                            + " @ " + methodId + "(예상 " + point.Count
                            + "곳, 실제 " + count + "곳; 대상 메서드 IL이 1.9.2 기준과 다름)");
                    }
                }
            }
            if (hits.Count == 0) return;
            // 重叠防御:不同注入点的 Match 区间不得相交(表 bug 显式化,
            // 静默错替比 Dead 更糟)。排序后线性扫尾。
            hits.Sort((a, b) => a.Index.CompareTo(b.Index));
            for (int j = 1; j < hits.Count; j++)
            {
                int prevEnd = hits[j - 1].Index + hits[j - 1].Point.Match.Length;
                if (hits[j].Index < prevEnd)
                {
                    throw new TranspileMatchException(
                        "transpiler 삽입 지점 구간이 겹침: " + hits[j - 1].Key + " @ "
                        + hits[j - 1].Index + " 및 " + hits[j].Key + " @ "
                        + hits[j].Index + " (" + methodId + ")");
                }
            }
            // 第二遍:从后往前统一替换(记录位不受后方改动影响)
            for (int j = hits.Count - 1; j >= 0; j--)
                ReplaceAt(code, hits[j].Index, hits[j].Point, hits[j].Key);
        }

        // ---- 窗口匹配:Before 段 + Match 段 + After 段按序全等 ----
        private static bool MatchesAt(List<CodeInstruction> code, int i, TranspilePoint p)
        {
            // Before:Match 前必须紧邻的指令序列
            if (p.Before != null)
            {
                int start = i - p.Before.Length;
                if (start < 0) return false;
                for (int j = 0; j < p.Before.Length; j++)
                    if (!InstrMatches(p.Before[j], code[start + j])) return false;
            }
            // Match 段
            for (int j = 0; j < p.Match.Length; j++)
                if (!InstrMatches(p.Match[j], code[i + j])) return false;
            // After:Match 后必须紧邻的指令序列
            if (p.After != null)
            {
                int start = i + p.Match.Length;
                if (start + p.After.Length > code.Count) return false;
                for (int j = 0; j < p.After.Length; j++)
                    if (!InstrMatches(p.After[j], code[start + j])) return false;
            }
            return true;
        }

        /// <summary>
        /// 单指令匹配:操作码全等 + 操作数按 <see cref="AnchorInstr"/> 的类型
        /// 分派(null / 装箱字面量 / <see cref="LocalRef"/> / <see cref="MemberRef"/>
        /// / <see cref="Anchor.Any"/> 通配,语义见 TranspilePlan 的锚点注释)。
        /// 整数字面量做宽化比较(不同后端对 ldc.i4.s 的 operand 可能给 sbyte
        /// 或 int,数值相等即认);float 按 IEEE 位值 Equals(1f/7f 折叠产物在
        /// 表内与 dump 同源同值);string 按 Equals(ldstr 锚仅用于稳定 tag 名)。
        /// </summary>
        private static bool InstrMatches(AnchorInstr a, CodeInstruction c)
        {
            if (c.opcode != a.Opcode) return false;
            object want = a.Operand;
            if (ReferenceEquals(want, Anchor.Any)) return true;   // 窗口内通配
            if (want == null) return c.operand == null;
            if (want is LocalRef lr)
            {
                return c.operand is LocalBuilder lb && lb.LocalIndex == lr.Index;
            }
            if (want is MemberRef mr)
            {
                if (!(c.operand is MemberInfo mi)) return false;
                string decl = mi.DeclaringType == null ? "?" : mi.DeclaringType.Name;
                return decl + "." + mi.Name == mr.Name;
            }
            if (c.operand == null) return false;
            // 整数族宽化:ldc.i4/ldc.i4.s 的 operand 可能装箱为 int 或 sbyte
            if (want is int wi) return c.operand is int oi && oi == wi
                || c.operand is sbyte os && os == wi
                || c.operand is short ws && ws == wi
                || c.operand is byte wb && wb == wi;
            if (want is long wl) return c.operand is long ol && ol == wl;
            return want.Equals(c.operand);
        }

        /// <summary>
        /// 就地替换 code[i..i+matchLen) 为注入段。两遍式的第二遍从后往前调用,
        /// 本方法不再返回长度(记位在第一遍已完成)。
        /// 统一入口选择(见 PointForm 注释):每形态一个强类型 getter,
        /// 替换段 = [ldstr key (+ 形态实参); call getter]——栈上类型由 getter
        /// 签名静态保证(int/float 与被替换的 ldc 一致,conv.* 等后继指令原样
        /// 保留仍合法)。
        /// 多指令等价替换(0.7.0 T7 ldloc 特形):DirectInt 对 Match 段长度 &gt; 1
        /// 的点逐条生成同 key 的 getter 组(layout.brood_size/output_size 的
        /// [ldloc.0; ldloc.0] → 两组 [ldstr; call],GameGridInventory ctor 双参
        /// 各取一值;长度 1 的标准 ldc 点行为不变);其余形态 Match 恒长 1,
        /// 单组生成。
        /// </summary>
        private static void ReplaceAt(List<CodeInstruction> code, int i,
            TranspilePoint point, string key)
        {
            var inject = new List<CodeInstruction>();
            switch (point.Form)
            {
                case PointForm.DirectInt:
                    // 逐条等价替换:Match 段每条指令贡献一组 [ldstr; call]
                    for (int j = 0; j < point.Match.Length; j++)
                    {
                        inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                        inject.Add(new CodeInstruction(OpCodes.Call,
                            TranspileValues.IntMethod));
                    }
                    break;
                case PointForm.DirectFloat:
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.FloatMethod));
                    break;
                case PointForm.ReciprocalFloat:
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.ReciprocalMethod));
                    break;
                case PointForm.PerTrainScaled:
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.PerTrainMethod));
                    break;
                case PointForm.RecipeCostScaled:
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.RecipeCostMethod));
                    break;
                case PointForm.RangeLow:
                case PointForm.RangeHigh:
                    // which:0 = 下界;1 = 上界(+1,Random.Range 排除上界)
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(point.Form == PointForm.RangeLow
                        ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.RangeBoundMethod));
                    break;
                case PointForm.FactionValue:
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, key));
                    inject.Add(new CodeInstruction(OpCodes.Ldstr, point.Arg));
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.FactionValueMethod));
                    break;
                case PointForm.TacticChance:
                    // 序列替换:Match = [ldsfld; ldloc(档位); ldelem.r4] →
                    // [原 ldloc 指令实例(复用,携带正确 LocalBuilder);
                    //   call GetTacticChance(int)]——CSV 第档位值的现读等价物
                    inject.Add(code[i + 1]);
                    inject.Add(new CodeInstruction(OpCodes.Call,
                        TranspileValues.TacticChanceMethod));
                    break;
                default:
                    throw new TranspileMatchException(
                        "알 수 없는 삽입 형식: " + point.Form + " @ " + key);
            }
            // C-1(Fix Round 1):被替换段的 labels 迁到注入段首条——1.9.2 IL
            // 的跳转目标常恰好指向被替换的 ldc 本身(main.on_semen_used:
            // IL_003f blt.s IL_0052 目标 = semen 点 Match;nightly_transpile
            // semen_chance_cap@IL_02c8 双 brfalse.s 入边;maiden_roll
            // AttrLowRange low@IL_002a;maiden_faction_base 50@IL_0037 两
            // brtrue.s + 10@IL_003a 一 brfalse.s——4 单元 5 点实证踩中),
            // RemoveRange 丢弃原指令实例的 labels → Harmony 重建 IL 时悬空
            // label 抛 Bad label content in ILGenerator → 开关开即 Dead。
            // 跳转落点 = 注入段起点,等价原指令求值位置。标准形态 Match 段
            // 为单条 ldc,labels 迁到注入段首条即完备;TacticChance 的
            // Match 段是 3 条且 inject[0] 复用原 ldloc 实例(j=1 时
            // old == inject[0])——ReferenceEquals 防自追加(AddRange 自身
            // 会把 labels 翻倍成 [L,L]),该路径当前 1.9.2 dump 零入边
            // 不触发,防御为结构性的;T7 的 ldloc 特形 Match 段 2 条
            // (BuildWindow 的 [ldloc.0; ldloc.0])同走全段迁移,该两点在
            // 1.9.2 dump 零入边(BuildWindow 全方法分支仅跳 lambda 缓存
            // 汇合点),迁移为普适兜底。
            // (两遍式第二遍从后往前执行,注入段不会再被扫描,无需跳过标记)
            if (inject.Count > 0)
            {
                for (int j = 0; j < point.Match.Length; j++)
                {
                    var old = code[i + j];
                    if (ReferenceEquals(old, inject[0])) continue; // R2:复用实例不自追加
                    if (old.labels == null || old.labels.Count == 0) continue;
                    inject[0].labels.AddRange(old.labels);
                }
            }
            code.RemoveRange(i, point.Match.Length);
            code.InsertRange(i, inject);
        }
    }

    /// <summary>
    /// transpiler 注入的 getter 集合(每 <see cref="PointForm"/> 一个强类型
    /// 入口;call 指令的目标方法反射出一次缓存)。读路径与 prefix 复刻体
    /// 完全同源:<see cref="PatchCtx.Config"/> 现读(玩家层/子项开关压层/
    /// 默认值三层语义由 ConfigRegistry.Current 承载),读失败或注册表未注入
    /// → 回退 <see cref="TranspilePlan.DefaultOf"/>(= 1.9.2 原常量,原版行为)。
    ///
    /// 性能评估(controller 裁决 3):调用频率 = 被替换 ldc 原本的求值频率,
    /// 最热路径 NightlyTick 每夜每苗床一次 × 16 注入点(数值面 14 + 乳汁面 2);单次开销 = 一次
    /// string 字典查(Current 内部 defaults + playerValues 两次 Dictionary
    /// 查找,纳秒级)+ 一次静态 call。相对原方法体(循环内多处 GetInt 的
    /// 字符串 tag 查询)是噪声级。**不做装配期快照缓存**:面板值「改完下一
    /// 夜生效」语义要求现读,而字典链已是最廉价路径(ConfigRegistry 本无
    /// 缓存层,0.6.0 全部 patch 走同一读法)——若未来出现真正的每帧热键
    /// 再评估 key→slot 数组化,不在本任务预支。
    /// </summary>
    public static class TranspileValues
    {
        // call 目标 MethodInfo(启动即解析一次;本 mod 自有类型,AccessTools
        // 是 Harmony 公开 API,非反射原语)
        internal static readonly MethodInfo IntMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetInt));
        internal static readonly MethodInfo FloatMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetFloat));
        internal static readonly MethodInfo ReciprocalMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetReciprocal));
        internal static readonly MethodInfo PerTrainMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetPerTrainScaled));
        internal static readonly MethodInfo RecipeCostMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetRecipeCostScaled));
        internal static readonly MethodInfo RangeBoundMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetRangeBound));
        internal static readonly MethodInfo FactionValueMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetFactionValue));
        internal static readonly MethodInfo TacticChanceMethod =
            AccessTools.Method(typeof(TranspileValues), nameof(GetTacticChance));

        /// <summary>int 直读(DirectInt 形态)。</summary>
        public static int GetInt(string key)
        {
            return Read(key, 0);
        }

        /// <summary>float 直读(DirectFloat 形态)。</summary>
        public static float GetFloat(string key)
        {
            return Read(key, 0f);
        }

        /// <summary>倒数形态(conceive_per_sex):原 ldc 是除数 100,
        /// cfg 语义为受孕率 → 返回 1f/cfg;cfg ≤ 0 → float.MaxValue
        /// (x/MaxValue ≈ 0 = 永不受孕的保守读法,不产生 Infinity 污染比较)。</summary>
        public static float GetReciprocal(string key)
        {
            float cfg = Read(key, 0.01f);
            return cfg > 0f ? 1f / cfg : float.MaxValue;
        }

        /// <summary>组合形态(semen_chance_per_train):原 ldc 0.1 是
        /// train/10×0.1 的尾乘子,cfg = 每点调教贡献 → 返回 cfg×10f。</summary>
        public static float GetPerTrainScaled(string key)
        {
            return Read(key, 0.01f) * 10f;
        }

        /// <summary>折叠形态(printer.recipe_cost):原 ldc 150 = 15×10
        /// (进度存储 ×10)→ 返回 cfg×10。</summary>
        public static int GetRecipeCostScaled(string key)
        {
            return Read(key, 15) * 10;
        }

        /// <summary>CSV 区间界(attr_*_range):面板 "lo,hi" 闭区间;
        /// which=0 → lo;which=1 → hi+1(Random.Range 上界排除 → IL 常量 = hi+1)。
        /// 解析容错复用 MaidenRoll.ParseIntRangeSafe(坏 CSV 回退默认串,
        /// 单键降级不连坐)。</summary>
        public static int GetRangeBound(string key, int which)
        {
            string def = TranspilePlan.DefaultOf(key) as string;
            string csv = Read(key, def);
            var (lo, hi) = MaidenRoll.ParseIntRangeSafe(csv, def, out _);
            return which == 0 ? lo : hi + 1;
        }

        /// <summary>阵营基价(faction_base):按 name 查 "名:值,…" CSV,
        /// 未命中回落「其他」项(MaidenRoll.FactionBase = 原版 switch 的
        /// 字典化等价物);解析容错复用 ParseFactionBasesSafe。</summary>
        public static int GetFactionValue(string key, string name)
        {
            string def = TranspilePlan.DefaultOf(key) as string;
            string csv = Read(key, def);
            var (bases, fallback) = MaidenRoll.ParseFactionBasesSafe(csv, def, 10);
            return MaidenRoll.FactionBase(name, bases, fallback);
        }

        /// <summary>陷阱策略捕捉率(tactic_chances):读 CSV 第 tactic 档
        /// (TACTICS 数组下标,主 mod 侧已 Clamp 到 [0,2]);坏 CSV 回退默认
        /// {0.1,0.2,0.3}(TrapRules.ParseTacticChances 失败返回 null,
        /// 与 0.6.0 DataWrite 写入路径同一解析器)。</summary>
        public static float GetTacticChance(int tactic)
        {
            float[] chances = TrapRules.ParseTacticChances(
                Read(CfgKeys.TrapTacticChances, "0.1,0.2,0.3"))
                ?? TrapRules.ParseTacticChances("0.1,0.2,0.3");
            if (tactic < 0) tactic = 0;
            if (tactic > 2) tactic = 2;
            return chances[tactic];
        }

        /// <summary>
        /// 统一读通道:注册表现读(与 prefix 复刻体同源);注册表缺席
        /// (PatchCtx 未注入/已 Shutdown)或读失败(键未注册等理论不可达)
        /// → 回退表内默认值 = 1.9.2 原常量。getter 是注入 IL 的常驻调用点,
        /// 绝不向被 patch 的方法传播异常(数值面失败语义 = 原版值,同
        /// prefix 复刻体 Phase A 的降级约定)。
        /// </summary>
        private static T Read<T>(string key, T def)
        {
            try
            {
                ConfigRegistry cfg = PatchCtx.Config;
                if (cfg != null) return cfg.Current<T>(key);
            }
            catch
            {
                // 读失败回默认(防御:键缺失/类型漂移都在表测试锁定,此处兜底)
            }
            object fallback = TranspilePlan.DefaultOf(key);
            return fallback is T t ? t : def;
        }
    }
}
