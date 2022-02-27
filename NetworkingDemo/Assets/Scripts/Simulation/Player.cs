using System;

public class Player 
{
    FP_AABB CollisionBox;
    FPosition Position;
    private static readonly FInt32 START_POS = 10;
    struct FPosition { public FInt32 x, y; }

    public Player(bool isPlayerOne) 
    {
        CollisionBox = new FP_AABB();
        CollisionBox.SetBoundsAndPosition(0, 0, 10, 10);
        Position.y = 0;
        Position.x = isPlayerOne ? -START_POS : START_POS;
    }
}
