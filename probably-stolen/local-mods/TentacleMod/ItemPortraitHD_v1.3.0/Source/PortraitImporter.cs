using System;
using System.IO;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class PortraitImporter
    {
        internal static bool Eligible(Source source) => source != null && source.Id != null &&
            !source.Id.StartsWith("local:", StringComparison.Ordinal) && !source.Id.StartsWith("game:", StringComparison.Ordinal) &&
            !source.Id.StartsWith("vanilla:", StringComparison.Ordinal) &&
            (source.File != null || source.ReadBytes != null || source.Resolve != null);

        internal static void CreateProvider(string root, string name)
        {
            // Validate every destination before making any directories.
            foreach (string faction in PortraitImportRules.Categories) PortraitImportRules.Destination(root, name, faction);
            foreach (string faction in PortraitImportRules.Categories)
                System.IO.Directory.CreateDirectory(PortraitImportRules.Destination(root, name, faction));
        }
        internal static string Import(Source source, string root, string provider, string faction, out bool existed)
        {
            if (!Eligible(source)) throw new InvalidOperationException("请选择其他模组的图片来源。");
            PortraitImportRules.Destination(root, provider, faction);
            // The preview and capture path already normalizes from the full source
            // and caches at the active resolution. Reuse it instead of decoding again.
            int size = ModEntry.Size;
            var sprite = Portraits.Get(source);
            if (!sprite || !sprite.texture) throw new InvalidOperationException("来源图片不可用，无法生成预处理立绘。");
            byte[] bytes = ImageConversion.EncodeToPNG(sprite.texture);
            return PortraitImportRules.SavePrepared(root, provider, faction, bytes, size, out existed);
        }
    }
}
