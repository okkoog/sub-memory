using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ItemPortraitHD
{
    // Observe the selected Sprite at the dependency's render boundary. No provider
    // reflection, character mapping or random draw is needed to rebuild its pixels.
    internal static class OriginalSelectedPortraits
    {
        private static readonly Dictionary<string, Source> sources = new Dictionary<string, Source>(StringComparer.Ordinal);
        private static readonly Dictionary<int, Sprite> rendered = new Dictionary<int, Sprite>();
        private static readonly HashSet<string> restoreAttempts = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Source> restoredSources = new Dictionary<string, Source>(StringComparer.Ordinal);
        private static Func<Sprite, int, int, Texture2D> originalRender;
        private static Func<Texture2D, float, Sprite> originalMake;
        private static Func<string, int, int, string> fromName, trimmed;

        private static bool Enabled => (ModEntry.UseOriginalPortraitSelection || OriginalCompat.FixedFaction) &&
            (!GuestPortraits.Building || GuestPortraits.Enabled);

        internal static void Bind(Type injector)
        {
            Portraits.Injected = ModAccess.Static<Dictionary<string, Sprite>>(injector, "injected");
            originalRender = ModReflect.StaticMethod<Func<Sprite, int, int, Texture2D>>(injector, "RenderTrimFit");
            originalMake = ModReflect.StaticMethod<Func<Texture2D, float, Sprite>>(injector, "MakeSprite");
            fromName = ModReflect.StaticMethod<Func<string, int, int, string>>(injector, "InjectPortraitFromName");
            trimmed = ModReflect.StaticMethod<Func<string, int, int, string>>(injector, "InjectPortraitTrimmed");
        }

        internal static void Remember(Sprite input, string path)
        {
            if (!Enabled || !input || string.IsNullOrEmpty(path)) return;
            sources[path] = InputSource(input);
        }

        private static Source InputSource(Sprite input) => new Source {
            Id = "original-selected:" + input.GetInstanceID(), Name = input.name, Resolve = () => input
        };

        internal static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> instructions)
        {
            int renderCount = 0, makeCount = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.operand is MethodInfo method && method.DeclaringType?.FullName == "MidnightPawn.Game.SpriteInjector")
                {
                    if (method.Name == "RenderTrimFit")
                    {
                        instruction.operand = AccessTools.Method(typeof(OriginalSelectedPortraits), nameof(RenderSelected));
                        renderCount++;
                    }
                    else if (method.Name == "MakeSprite")
                    {
                        instruction.operand = AccessTools.Method(typeof(OriginalSelectedPortraits), nameof(MakeSelected));
                        makeCount++;
                    }
                }
                yield return instruction;
            }
            if (renderCount != 1 || makeCount != 1)
                throw new InvalidOperationException("原模组已选立绘的高清重建接口不匹配。");
        }

        private static Texture2D RenderSelected(Sprite input, int width, int height)
        {
            if (!Enabled || !input) return originalRender(input, width, height);
            var sprite = Portraits.Owns(input) && input.texture.width == ModEntry.Size ? input : Portraits.Get(InputSource(input));
            if (!sprite) return originalRender(input, width, height);
            // MakeSprite must return this exact Sprite, whose PPU preserves 4x4.
            // Returning only the larger texture to the original MakeSprite makes
            // it 2x/4x too large. This entry lives only until the following call.
            rendered[sprite.texture.GetInstanceID()] = sprite;
            return sprite.texture;
        }

        private static Sprite MakeSelected(Texture2D texture, float ppu)
        {
            if (texture && rendered.TryGetValue(texture.GetInstanceID(), out var sprite))
            {
                rendered.Remove(texture.GetInstanceID());
                if (sprite) return sprite;
            }
            return originalMake(texture, ppu);
        }

        internal static string CachePath(string source, bool isTrimmed) => "maiden_" +
            (isTrimmed ? source.Replace(':', '_').Replace(' ', '_') : source.Replace(':', '_').Replace(" ", ""));

        internal static void PrepareFromName(string __0) => Prepare(__0, false);
        internal static void PrepareTrimmed(string source) => Prepare(source, true);

        private static void Prepare(string source, bool isTrimmed)
        {
            if (!Enabled || string.IsNullOrEmpty(source)) return;
            var cache = Portraits.Injected;
            string path = CachePath(source, isTrimmed);
            // A previous 64px cache entry (or a destroyed/other-resolution HD one)
            // must not short-circuit the source observation and HD reconstruction.
            if (cache != null && cache.TryGetValue(path, out var old) &&
                (!Portraits.Owns(old) || old.texture.width != ModEntry.Size)) cache.Remove(path);
        }

        internal static Sprite Observed(GameItem item)
        {
            if (item.useSpriteFromMod && item.spriteAtlasPath == "lewd_meme" &&
                sources.TryGetValue(item.spritePath ?? "", out var source))
            {
                var exact = Portraits.Get(source);
                if (exact) return exact;
            }
            return null;
        }

        internal static Sprite For(GameItem item)
        {
            var observed = Observed(item);
            if (observed) return observed;
            var mod = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest?.ID == "lewd_meme");
            Sprite published = null;
            if (mod?.Sprites != null && !string.IsNullOrEmpty(item.spritePath)) mod.Sprites.TryGetValue(item.spritePath, out published);
            if (Portraits.Owns(published) && published.texture.width == ModEntry.Size) return published;
            // Save loading or switching modes may leave only a 64px published
            // image. Ask the original resolver to restore its saved source once,
            // so we observe the full input instead of upscaling those 64 pixels.
            string savedSource = ModEntry.Get(item, "MAIDEN_SPRITE_SRC");
            if (restoredSources.TryGetValue(savedSource, out var source))
            {
                var exact = Portraits.Get(source);
                if (exact) return exact;
            }
            if (!string.IsNullOrEmpty(savedSource) && !savedSource.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal) &&
                fromName != null && restoreAttempts.Add(savedSource + "@" + ModEntry.Size))
            {
                string path = savedSource.IndexOf(':') < 0 ? fromName(savedSource, 64, 64) : trimmed(savedSource, 64, 64);
                if (string.IsNullOrEmpty(path) && savedSource.IndexOf(':') < 0) path = trimmed(savedSource, 64, 64);
                if (!string.IsNullOrEmpty(path) && sources.TryGetValue(path, out source))
                {
                    restoredSources[savedSource] = source;
                    var exact = Portraits.Get(source);
                    return exact;
                }
            }
            // Previously captured sources from our own selection mode remain
            // explicitly restorable; this does not perform another random draw.
            if (savedSource.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal))
                return Portraits.Get(RandomPortraits.Find(savedSource));
            return null;
        }

        internal static bool IsRebuiltPath(string path) => !string.IsNullOrEmpty(path) && sources.ContainsKey(path);
        internal static void ResetTextures() { rendered.Clear(); restoreAttempts.Clear(); }
        internal static void Clear()
        {
            ResetTextures(); sources.Clear(); restoredSources.Clear();
            originalRender = null; originalMake = null; fromName = trimmed = null;
        }
    }
}
