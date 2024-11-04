

public enum SlingShotState
{
    /// <summary>
    /// The slingshot is idle and ready for the player to interact.
    /// </summary>
    Idle,

    /// <summary>
    /// The player is currently pulling back the slingshot to launch a bird.
    /// </summary>
    UserPulling,

    /// <summary>
    /// The bird has been launched and is in flight.
    /// </summary>
    BirdFlying,

    /// <summary>
    /// The slingshot is resetting to its initial position after a launch.
    /// </summary>
    Resetting
}