namespace ItemPortraitHD
{
    internal static class Footprint
    {
        internal static GridShape Square() => new GridShapeBuilder(4, 4).SetDataFill(1).Build();
        internal static bool IsSquare(GridShape shape)
        {
            if (shape == null || shape.width != 4 || shape.height != 4) return false;
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)
                if (shape.GetLocal(x, y) != 1) return false;
            return true;
        }
    }
}
