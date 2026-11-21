using System;
using System.Collections.Generic;
using HarmonyLib;

namespace ItemPortraitHD
{
    internal static class ShapeRules
    {
        [ThreadStatic] private static bool changing;
        private static readonly Queue<GameItem> overflow = new Queue<GameItem>();
        private static readonly HashSet<GameItem> queued = new HashSet<GameItem>();
        internal static string LastNotice = "";

        internal static GridShape Square() => Footprint.Square();
        internal static bool IsSquare(GridShape shape) => Footprint.IsSquare(shape);

        internal static bool Ensure(GameItem item, bool allowOverflow = false)
        {
            if (changing || ModEntry.Instance?.Active != true || !ModEntry.Target(item)) return false;
            if (IsSquare(item.shape) && IsSquare(item.modifiedShape)) return false;
            changing = true;
            try
            {
                var square = Square();
                var placement = item.modifiedShape != null ? new GridShapeBuilder(square).SetTransform(item.modifiedShape).Build() : square.Clone();
                if (item.parentInventory is GameGridInventory grid)
                {
                    var ignored = new List<GameItem> { item };
                    if (!grid.CheckShapePlacement(item, placement, ignored))
                    {
                        // Old saves can have another item in a portrait's transparent
                        // columns. Find a free rectangle without ejecting or deleting it.
                        GridShape found = null;
                        var bounds = grid.inventoryShape;
                        for (int y = bounds.minY; y <= bounds.maxY - 3 && found == null; y++)
                            for (int x = bounds.minX; x <= bounds.maxX - 3; x++)
                            {
                                var candidate = new GridShapeBuilder(square).SetPosition(x, y).Build();
                                if (grid.CheckShapePlacement(item, candidate, ignored)) { found = candidate; break; }
                            }
                        if (found == null)
                        {
                            if (allowOverflow) return MoveOverflow(item, square, placement);
                            if (queued.Add(item)) overflow.Enqueue(item);
                            return false;
                        }
                        placement = found;
                    }
                }
                else if (item.parentInventory != null)
                {
                    // Respect non-grid inventories' own placement restrictions.
                    if (!item.parentInventory.CheckInventorySlotStillValid(item, placement, item.modifiedState))
                    {
                        if (allowOverflow) return MoveOverflow(item, square, placement);
                        if (queued.Add(item)) overflow.Enqueue(item);
                        return false;
                    }
                }
                item.shape = square;
                item.modifiedShape = placement;
                item.modifiedXOrigin = 0;
                item.modifiedYOrigin = 0;
                return true;
            }
            catch (Exception ex) { ModEntry.Warn(ex); return false; }
            finally { changing = false; }
        }

        internal static void ProcessOverflow()
        {
            // Move on the next Unity Update, outside inventory Validate loops.
            // Removing a child during their enumeration would invalidate that loop.
            int count = Math.Min(overflow.Count, 4);
            while (count-- > 0)
            {
                var item = overflow.Dequeue(); queued.Remove(item);
                try { if (Ensure(item, true)) item.ValidateShapeState(); }
                catch (Exception ex) { ModEntry.Warn(ex); }
            }
        }

        internal static void ResetQueue()
        {
            overflow.Clear(); queued.Clear(); LastNotice = "";
        }

        private static bool MoveOverflow(GameItem item, GridShape square, GridShape placement)
        {
            var store = EmporiumEntry.Instance;
            var originalInventory = item.parentInventory;
            if (!store || originalInventory == null) return false;
            GridShape oldBase = item.shape, oldModified = item.modifiedShape;
            int oldX = item.modifiedXOrigin, oldY = item.modifiedYOrigin;
            bool moved = false;
            try
            {
                item.shape = square;
                item.modifiedShape = placement;
                item.modifiedXOrigin = 0; item.modifiedYOrigin = 0;
                // Matches EmporiumEntry.TryAddToPlayerInv's first two destinations:
                // player-side counter, then main storage. Use the normal transfer
                // routine so removal/addition hooks and rendering parents stay valid.
                var destinations = new[] { store.backInvinvElement, store.invElement };
                for (int i = 0; i < destinations.Length; i++)
                {
                    var destination = destinations[i];
                    if (destination == null || ReferenceEquals(destination, originalInventory)) continue;
                    int remaining = GraphUtils.TryAcceptAll(destination, item, -1);
                    if (remaining == 0 && item.parentInventory != null && !ReferenceEquals(item.parentInventory, originalInventory))
                    {
                        moved = true;
                        LastNotice = item.name + " 已调整为完整 4×4，并移至" + (i == 0 ? "己方柜台。" : "仓库。");
                        return true;
                    }
                }
                LastNotice = "柜台和仓库都没有可用 4×4 空间：" + item.name + " 暂留原位，腾出空间后点击重载图片。";
                return false;
            }
            finally
            {
                if (!moved)
                {
                    item.shape = oldBase; item.modifiedShape = oldModified;
                    item.modifiedXOrigin = oldX; item.modifiedYOrigin = oldY;
                    // Normal failed searches leave the item attached. Restore its
                    // original parent if a transfer callback rejected it after removal.
                    if (item.parentInventory == null) originalInventory.UncheckedAccept(item);
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameItem), "ValidateShapeState")]
    internal static class ShapeStatePatch
    {
        [HarmonyPrefix]
        private static void Prefix(GameItem __instance) => ShapeRules.Ensure(__instance);
    }

    [HarmonyPatch(typeof(GameItemElement), "ValidateSelectLocked")]
    internal static class ExistingShapePatch
    {
        [HarmonyPrefix]
        private static void Prefix(GameItemElement __instance)
        {
            if (ShapeRules.Ensure(__instance)) __instance.ValidateShapeState();
        }
    }
}
