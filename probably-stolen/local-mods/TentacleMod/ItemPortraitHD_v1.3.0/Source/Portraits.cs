using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemPortraitHD
{
    internal static class Portraits
    {
        private sealed class Entry { internal string SourceId; internal Sprite Sprite, Input; internal Texture2D Texture; internal long Used; internal int Size; }
        private static readonly Dictionary<string, Entry> cache = new Dictionary<string, Entry>();
        private static readonly HashSet<int> owned = new HashSet<int>();
        private static readonly List<Entry> retired = new List<Entry>();
        internal static Dictionary<string, Sprite> Injected;
        private static readonly Dictionary<string, Sprite> failed = new Dictionary<string, Sprite>();
        private static long tick;
        internal static bool Owns(Sprite s) => s && owned.Contains(s.GetInstanceID());
        internal static Sprite For(GameItem item)
        {
            string selection = ModEntry.Get(item, "IPH_SOURCE");
            if (string.IsNullOrEmpty(selection)) return OriginalFor(item);
            if (Catalog.All.Count == 0) Catalog.Scan();
            var choice = Catalog.Find(selection);
            if (choice == null && !string.IsNullOrEmpty(selection))
                choice = RandomPortraits.Find(RandomPortraits.Prefix + selection);
            return Get(choice) ?? OriginalFor(item);
        }

        // Resolve the persisted original independently of the F7 override. This
        // also lets the restore button verify availability before clearing it.
        internal static Sprite OriginalFor(GameItem item)
        {
            if (ModEntry.UseOriginalPortraitSelection) return OriginalSelectedPortraits.For(item);
            var observed = OriginalSelectedPortraits.Observed(item);
            if (observed) return observed;
            if (Catalog.All.Count == 0) Catalog.Scan();
            return Get(Catalog.Original(ModEntry.Get(item, "MAIDEN_SPRITE_SRC"))) ?? OriginalSelectedPortraits.For(item);
        }

        internal static bool TryRestoreOriginal(GameItemElement item, out string error)
        {
            error = null;
            string previous = ModEntry.Get(item, "IPH_SOURCE");
            bool changed = false;
            try
            {
                RetryUnavailable();
                if (!OriginalFor(item))
                {
                    error = "原立绘来源当前不可用，已保留现有立绘。请确认对应立绘包已启用、PNG 未被移动：" +
                        ModEntry.Get(item, "MAIDEN_SPRITE_SRC");
                    return false;
                }
                ModEntry.Set(item, "");
                changed = true;
                item.ValidatePartialSprite();
                return true;
            }
            catch (Exception ex)
            {
                if (changed)
                {
                    ModEntry.Set(item, previous);
                    try { item.ValidatePartialSprite(); }
                    catch (Exception refreshError) { ModEntry.Warn(refreshError); }
                }
                ModEntry.Warn(ex);
                error = "恢复原立绘失败，已保留现有选择：" + ex.Message;
                return false;
            }
        }

        internal static void RetryUnavailable()
        {
            failed.Clear();
            OriginalSelectedPortraits.ResetTextures();
        }

        internal static Sprite Get(Source source)
        {
            if (source == null) return null;
            string key = source.Id + "@" + ModEntry.Size;
            bool fileSource = source.File != null || source.ReadBytes != null;
            if (fileSource && failed.ContainsKey(key)) return null;
            Sprite input = null;
            bool temporary = false;
            bool prepared = false;
            Texture2D decodedTexture = null;
            long started = 0;
            bool converted = false;
            try
            {
                if (source.File == null && source.ReadBytes == null)
                    using (PerformanceTrace.Measure("来源取图 " + source.Id)) input = source.Resolve?.Invoke();
                // A live character key can start resolving to a different Sprite
                // after a provider loads/reloads. A failed old input must not block it.
                if (!fileSource && failed.TryGetValue(key, out var failedInput))
                {
                    if (ReferenceEquals(failedInput, input)) return null;
                    failed.Remove(key);
                }
                if (cache.TryGetValue(key, out var existing))
                {
                    if (existing.Sprite && existing.Texture &&
                        (source.File != null || source.ReadBytes != null || existing.Input == input))
                    { existing.Used = ++tick; return existing.Sprite; }
                    retired.Add(existing); cache.Remove(key);
                }
                started = System.Diagnostics.Stopwatch.GetTimestamp();
                if (fileSource)
                {
                    if (source.File != null && !File.Exists(source.File)) return null;
                    if (source.File != null && new FileInfo(source.File).Length > 32 * 1024 * 1024)
                        throw new InvalidDataException("PNG 超过 32MB: " + source.Name);
                    byte[] bytes;
                    using (PerformanceTrace.Measure("读取PNG " + source.Id))
                        bytes = source.ReadBytes != null ? source.ReadBytes() : File.ReadAllBytes(source.File);
                    if (bytes == null) return null;
                    PortraitImportRules.ValidatePng(bytes);
                    prepared = source.Id.StartsWith("local:", StringComparison.Ordinal) && PortraitImportRules.IsPrepared(source.File, bytes);
                    var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    decodedTexture = texture;
                    bool decoded;
                    using (PerformanceTrace.Measure("解码PNG " + source.Id)) decoded = ImageConversion.LoadImage(texture, bytes, false);
                    if (!decoded)
                    { throw new InvalidDataException("无法读取 PNG: " + source.Name); }
                    input = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
                    temporary = true;
                }
                if (!input) return null;
                // Different source IDs may resolve to the exact same selected Sprite.
                // Reuse its existing conversion, including F7/capture previews.
                foreach (var entry in cache.Values)
                    if (entry.Input == input && entry.Size == ModEntry.Size && entry.Sprite)
                    { entry.Used = ++tick; RebindPrevious(source.Id, entry.Sprite); return entry.Sprite; }
                Texture2D output;
                if (prepared && input.texture.width == ModEntry.Size && input.texture.height == ModEntry.Size)
                {
                    // Transfer ownership of the decoded PNG directly into the cache.
                    output = input.texture;
                    output.filterMode = FilterMode.Bilinear; output.wrapMode = TextureWrapMode.Clamp;
                    Object.Destroy(input); input = null; temporary = false; decodedTexture = null;
                }
                else if (prepared)
                {
                    using (PerformanceTrace.Measure("预处理画布缩放 " + source.Id))
                        output = ResizePrepared(input.texture, ModEntry.Size);
                }
                else using (PerformanceTrace.Measure("裁切缩放 " + source.Id)) output = Render(input, ModEntry.Size);
                converted = true;
                if (!output) { failed[key] = input; return null; }
                Sprite result = Sprite.Create(output, new Rect(0, 0, ModEntry.Size, ModEntry.Size),
                    new Vector2(.5f, .5f), 100f * ModEntry.Size / 64f, 0, SpriteMeshType.FullRect);
                result.name = "IPH_" + key;
                // Trim unused previews while keeping every live inventory sprite.
                if (cache.Count >= 192) EvictPreview();
                cache[key] = new Entry { SourceId = source.Id, Sprite = result, Texture = output, Input = temporary ? null : input, Used = ++tick, Size = ModEntry.Size };
                owned.Add(result.GetInstanceID());
                // Update only registrations still holding this source's previous result.
                // Visible UI references keep the old texture alive until their refresh.
                RebindPrevious(source.Id, result);
                return result;
            }
            catch (Exception ex) { failed[key] = input; ModEntry.Warn(ex); return null; }
            finally
            {
                if (temporary && input) Object.Destroy(input);
                if (decodedTexture) Object.Destroy(decodedTexture);
                double ms = started == 0 ? 0 : (System.Diagnostics.Stopwatch.GetTimestamp() - started) *
                    1000.0 / System.Diagnostics.Stopwatch.Frequency;
                if (converted && ms >= 30)
                    Debug.Log("[ItemPortraitHD] 立绘首次转换耗时 " + ms.ToString("F0") + "ms: " + key);
            }
        }
        private static Texture2D ResizePrepared(Texture2D source, int size)
        {
            var output = new Texture2D(size, size, TextureFormat.RGBA32, false)
                { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            output.SetPixels(Rasterizer.ResizeCanvas(source.GetPixels32(), source.width, source.height, size));
            output.Apply(false, false);
            return output;
        }
        private static IEnumerable<Dictionary<string, Sprite>> Registries()
        {
            if (Injected != null) yield return Injected;
            foreach (var mod in ModLoader.Mods)
                if (mod.Manifest?.ID == "lewd_meme" && mod.Sprites != null) yield return mod.Sprites;
        }
        internal static void Rebind(Dictionary<string, Sprite> registry, Sprite previous, Sprite replacement)
        {
            foreach (string key in registry.Where(p => ReferenceEquals(p.Value, previous)).Select(p => p.Key).ToArray())
                registry[key] = replacement;
        }
        private static void RebindPrevious(string sourceId, Sprite replacement)
        {
            foreach (var entry in retired.Where(e => e.SourceId == sourceId))
                foreach (var registry in Registries()) Rebind(registry, entry.Sprite, replacement);
        }
        private static HashSet<int> Referenced()
        {
            var inUse = new HashSet<int>();
            foreach (var registry in Registries())
                foreach (var sprite in registry.Values)
                    if (sprite) inUse.Add(sprite.GetInstanceID());
            // Retired conversions can themselves be inputs to a still-live conversion.
            foreach (var entry in cache.Values.Concat(retired))
                if (entry.Input) inUse.Add(entry.Input.GetInstanceID());
            foreach (var item in Seen.Live())
            {
                var sprite = item.background?.image?.sprite;
                if (sprite) inUse.Add(sprite.GetInstanceID());
                sprite = item.fakeBackground?.image?.sprite;
                if (sprite) inUse.Add(sprite.GetInstanceID());
            }
            return inUse;
        }
        private static void EvictPreview()
        {
            var inUse = Referenced();
            string oldest = null; long used = long.MaxValue;
            foreach (var pair in cache)
                if (pair.Value.Sprite && !inUse.Contains(pair.Value.Sprite.GetInstanceID()) && pair.Value.Used < used)
                { oldest = pair.Key; used = pair.Value.Used; }
            if (oldest != null) { Release(cache[oldest]); cache.Remove(oldest); }
        }
        private static void Release(Entry entry)
        {
            if (entry.Sprite) { owned.Remove(entry.Sprite.GetInstanceID()); Object.Destroy(entry.Sprite); }
            if (entry.Texture) Object.Destroy(entry.Texture);
        }
        internal static void Clear()
        {
            OriginalSelectedPortraits.ResetTextures();
            retired.AddRange(cache.Values);
            cache.Clear(); failed.Clear();
            CollectRetired();
        }
        internal static void CollectRetired()
        {
            if (retired.Count == 0) return;
            var inUse = Referenced();
            for (int i = retired.Count - 1; i >= 0; i--)
                if (!retired[i].Sprite || !inUse.Contains(retired[i].Sprite.GetInstanceID()))
                { Release(retired[i]); retired.RemoveAt(i); }
        }

        private static Texture2D Render(Sprite source, int size)
        {
            var rect = source.textureRect;
            int w = Mathf.RoundToInt(rect.width), h = Mathf.RoundToInt(rect.height);
            if (w < 1 || h < 1) return null;
            var previous = RenderTexture.active;
            RenderTexture rt = null;
            Texture2D copy = null;
            try
            {
                Color32[] pixels = ReadablePixels(source, rect, w, h);
                if (pixels == null)
                {
                    // GPU readback is needed only for non-readable textures.
                    rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
                    Graphics.Blit(source.texture, rt,
                        new Vector2(rect.width / source.texture.width, rect.height / source.texture.height),
                        new Vector2(rect.x / source.texture.width, rect.y / source.texture.height));
                    RenderTexture.active = rt;
                    copy = new Texture2D(w, h, TextureFormat.RGBA32, false);
                    copy.ReadPixels(new Rect(0, 0, w, h), 0, 0, false);
                    pixels = copy.GetPixels32();
                }
                var output = Rasterizer.FitPixels(pixels, w, h, size);
                if (output == null) return null;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                    { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
                texture.SetPixels(output);
                // This sprite is also published to lewd_meme's sprite dictionary.
                // GameItem.SetSpriteAndShapeFromMod reads its alpha with GetPixel
                // during both item creation and save restoration. Keep CPU pixels.
                texture.Apply(false, false);
                return texture;
            }
            finally
            {
                RenderTexture.active = previous;
                if (rt) RenderTexture.ReleaseTemporary(rt);
                if (copy) Object.Destroy(copy);
            }
        }

        internal static Color32[] ReadablePixels(Sprite source, Rect rect, int w, int h)
        {
            var texture = source.texture;
            if (!texture.isReadable) return null;
            try
            {
                var pixels = texture.GetPixels32();
                int x = Mathf.RoundToInt(rect.x), y = Mathf.RoundToInt(rect.y);
                if (x == 0 && y == 0 && w == texture.width && h == texture.height) return pixels;
                var cropped = new Color32[w * h];
                for (int row = 0; row < h; row++)
                    Array.Copy(pixels, (y + row) * texture.width + x, cropped, row * w, w);
                return cropped;
            }
            catch (UnityException) { return null; }
        }

    }
}


