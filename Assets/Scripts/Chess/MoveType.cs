namespace Chess.Pieces
{
    public enum MoveType
    {
        // Pieces will complete their move in a constant timeframe irrespective of distance
        TimeConstant,
        // Pieces will complete their move in a timeframe that is relative to distance
        TimeRelativeToDistance
    }
}