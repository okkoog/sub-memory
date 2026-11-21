using System.Collections.Generic;

namespace ItemPortraitHD
{
    internal static class CurrentPortraitItems
    {
        internal static List<GameItemElement> Scan(EmporiumEntry entry)
        {
            if (entry == null) return new List<GameItemElement>();
            // Inventory membership is authoritative here. GetAllOwnedItems also
            // requires IS_OWNED_TAG on every descendant, which can omit saved
            // container contents. Never scan the sold-items or guest UI roots.
            return Collect(new GraphNodeStorage[] {
                entry.invElement, entry.backInvinvElement, entry.backInvinvElementCounter,
                entry.frontInvinvElement, entry.docInvElement, entry.showcaseElement,
                entry.trashInvElement, entry.hiddenElement, entry.vendingFountainElement,
                entry.vendingMachineElement, entry.afterhourWindow
            });
        }

        internal static List<GameItemElement> Collect(IEnumerable<GraphNodeStorage> roots)
        {
            var result = new List<GameItemElement>();
            var visited = new HashSet<GraphNodeStorage>();
            var pending = new Queue<GraphNodeStorage>();
            foreach (var root in roots)
                if (root != null && visited.Add(root)) pending.Enqueue(root);
            while (pending.Count > 0)
            {
                var node = pending.Dequeue();
                if (node is GameItemElement item && ModEntry.Target(item)) result.Add(item);
                var children = node.children;
                if (children == null) continue;
                foreach (var child in children)
                    if (child != null && visited.Add(child)) pending.Enqueue(child);
            }
            return result;
        }
    }
}
