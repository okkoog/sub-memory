using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 写通引擎(M4a,纯逻辑:System.* only;spec §4.5 write-through 键)。
    /// 接管目标 = 另一 mod 的官方 ModConfig 条目:Enable 首次调用时快照对方当前值、
    /// 写入我们的值;Disable 恢复快照(无快照则纯 no-op,不调 write)。
    /// 快照锚定开启时刻(0.6.0 修正裁定):同一开启周期内(自首次 Enable 至
    /// Disable 之间)重复 Enable 只写值、**不覆盖快照**——值变化重写走同一
    /// Enable,恢复点保持为开启时刻的对方原值;Disable 清除快照后,再次
    /// Enable 开始新周期、重新锚定(spec §4.5「关 = 恢复开启时的快照」)。
    /// 被动 last-writer-wins——从不订阅对方 OnChanged,对方后续的自家写回直接
    /// 生效,本引擎不拦截不回调。快照仅存会话内存,不落盘(spec §6.1),存档
    /// 载入即丢失,下次 Enable 重新快照。值统一 string 进/string 出
    /// (ConfigEntry.Serialize 语义;游戏侧取值/写回的转换由 Task 11 载体完成,
    /// 本引擎对值内容零解释)。
    /// </summary>
    public sealed class WriteThroughEngine
    {
        /// <summary>
        /// (modId, key) → 开启时刻快照值(锚定于本开启周期的首次 Enable)。ValueTuple
        /// 键按字符串序数相等比较(string.Equals 即 Ordinal,与 ConfigRegistry 同
        /// 风格);写入动作是调用方注入的 <see cref="Action{T}"/>,本引擎只经它
        /// 落值,不感知游戏。
        /// </summary>
        private readonly Dictionary<(string ModId, string Key), string> Snapshots =
            new Dictionary<(string ModId, string Key), string>();

        /// <summary>
        /// 启用接管:write(ourValue) 写入我们的值;无快照时以 currentValue 建立
        /// 开启时刻快照(锚定),**已有快照时只写、不覆盖快照**(周期内值变化
        /// 重写走本方法,恢复点不动)。currentValue == null(目标键缺席,对方
        /// mod 未定义该条目)→ 不写、不建、不清既有快照(周期内键消失属理论
        /// 不可达,保守保留恢复点;日志归调用方)。
        /// </summary>
        public void Enable(string modId, string key, string currentValue, string ourValue,
                           Action<string> write)
        {
            if (currentValue == null) return; // 目标键不存在:纯 no-op,由调用方记日志
            write(ourValue);
            // 锚定开启时刻:首个 Enable 建快照,周期内后续 Enable 不覆盖
            if (!Snapshots.ContainsKey((modId, key)))
                Snapshots[(modId, key)] = currentValue;
        }

        /// <summary>
        /// 停用接管:有快照 → write(snapshot) 恢复对方原值并清除快照;无快照
        /// (从未 Enable / 已恢复过)→ 不调 write,纯 no-op。
        /// </summary>
        public void Disable(string modId, string key, Action<string> write)
        {
            string snap;
            if (!Snapshots.TryGetValue((modId, key), out snap)) return; // 无快照:不碰 write
            write(snap); // 先恢复(简报字面序;write 抛异常时快照仍在,可重试)
            Snapshots.Remove((modId, key)); // 一次性:恢复后即清除(再次 Disable 为 no-op)
        }

        /// <summary>该 (modId, key) 是否持有未恢复的快照。</summary>
        public bool HasSnapshot(string modId, string key)
        {
            return Snapshots.ContainsKey((modId, key));
        }
    }
}
