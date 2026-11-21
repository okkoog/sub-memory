using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ItemPortraitHD
{
    internal static class PortraitImportRules
    {
        internal static readonly string[] Categories = LocalPortraitRules.Factions.Concat(new[] { LocalPortraitRules.Unrestricted }).ToArray();
        internal static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 80 || name != name.Trim() || name.EndsWith(".", StringComparison.Ordinal) ||
                name == "." || name == ".." || name.Any(c => c < 32 || "<>:\"/\\|?*".Contains(c)))
                throw new ArgumentException("来源名需为 1–80 个字符，不能含路径分隔符、非法字符或首尾空格。");
            string stem = name.Split('.')[0].ToUpperInvariant();
            if (new[] { "CON", "PRN", "AUX", "NUL" }.Contains(stem) ||
                (stem.Length == 4 && (stem.StartsWith("COM", StringComparison.Ordinal) || stem.StartsWith("LPT", StringComparison.Ordinal)) &&
                stem[3] >= '1' && stem[3] <= '9')) throw new ArgumentException("来源名不能使用 Windows 保留设备名。");
        }
        internal static string Destination(string root, string provider, string faction)
        {
            ValidateName(provider);
            if (!Categories.Contains(faction)) throw new ArgumentException("请选择五大阵营之一或不限阵营。");
            string absoluteRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string result = Path.GetFullPath(Path.Combine(absoluteRoot, provider, faction));
            if (!result.StartsWith(absoluteRoot, StringComparison.OrdinalIgnoreCase)) throw new IOException("导入目录超出 Portraits。");
            RejectLinks(absoluteRoot.TrimEnd(Path.DirectorySeparatorChar), result);
            return result;
        }
        private static void RejectLinks(string root, string destination)
        {
            string cursor = destination;
            while (cursor != null && cursor.Length >= root.Length)
            {
                if (System.IO.Directory.Exists(cursor) && (File.GetAttributes(cursor) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("导入目录不能是符号链接或目录联接。");
                if (string.Equals(cursor, root, StringComparison.OrdinalIgnoreCase)) break;
                cursor = Path.GetDirectoryName(cursor);
            }
        }
        internal static void ValidatePng(byte[] bytes)
        {
            byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
            if (bytes == null || bytes.Length < 33 || bytes.Length > 32 * 1024 * 1024 || !bytes.Take(8).SequenceEqual(signature) ||
                bytes[12] != 'I' || bytes[13] != 'H' || bytes[14] != 'D' || bytes[15] != 'R')
                throw new InvalidDataException("来源不是支持的 PNG，或文件超过 32MB。");
            if (PngDimension(bytes, 16) < 1 || PngDimension(bytes, 20) < 1 || PngDimension(bytes, 16) > 4096 || PngDimension(bytes, 20) > 4096)
                throw new InvalidDataException("PNG 尺寸须为 1–4096 像素。");
        }
        internal static string SavePrepared(string root, string provider, string faction, byte[] bytes, int size, out bool existed)
        {
            ValidatePng(bytes);
            if ((size != 128 && size != 256) || PngDimension(bytes, 16) != size || PngDimension(bytes, 20) != size)
                throw new InvalidDataException("预处理图片尺寸与清晰度不匹配。");
            return SaveCore(root, provider, faction, bytes, "iph_fit_v1_" + size + "_", out existed);
        }

        private static uint PngDimension(byte[] bytes, int offset) => ((uint)bytes[offset] << 24) |
            ((uint)bytes[offset + 1] << 16) | ((uint)bytes[offset + 2] << 8) | bytes[offset + 3];

        private static string ContentHash(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        internal static bool IsPrepared(string file, byte[] bytes)
        {
            if (file == null || bytes == null || bytes.Length < 33) return false;
            uint size = PngDimension(bytes, 16);
            if ((size != 128 && size != 256) || PngDimension(bytes, 20) != size) return false;
            string prefix = "iph_fit_v1_" + size + "_";
            string name = Path.GetFileNameWithoutExtension(file);
            // Verify content before skipping normalization; edited files use the ordinary path.
            return name.StartsWith(prefix, StringComparison.Ordinal) && name.Length == prefix.Length + 64 &&
                string.Equals(name.Substring(prefix.Length), ContentHash(bytes), StringComparison.Ordinal);
        }

        private static string SaveCore(string root, string provider, string faction, byte[] bytes, string prefix, out bool existed)
        {
            string destination = Destination(root, provider, faction);
            System.IO.Directory.CreateDirectory(destination);
            string hash = ContentHash(bytes);
            // Content names avoid collisions across providers and repeated imports without an index file.
            string path = Path.Combine(destination, prefix + hash + ".png");
            if (File.Exists(path))
            {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0 || !File.ReadAllBytes(path).SequenceEqual(bytes))
                    throw new IOException("目标文件已存在但内容不匹配，未覆盖。");
                existed = true; return path;
            }
            string temporary = Path.Combine(destination, ".import_" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    stream.Write(bytes, 0, bytes.Length);
                File.Move(temporary, path);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
            existed = false; return path;
        }
    }
}
