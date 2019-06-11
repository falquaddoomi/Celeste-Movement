public interface ICollision {
    bool onGround { get; }
    bool onWall { get; }
    bool onRightWall { get; }
    bool onLeftWall { get; }
    int wallSide { get; }
}