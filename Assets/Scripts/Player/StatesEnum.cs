public enum States
{
    None,
    Grounded,
    InAir,
    Abilities
}
public enum GroundedStates
{
    None,
    Idle,
    Walk,
    Run,
    Land,
    Transition
}

public enum AbilityStates
{
    None,
    Dodge,
    Leap,
    Grapple,
}

public enum LandStates
{
    None,
    Soft,
    Hard
}
public enum InAirStates
{
    None,
    Grapple,
    Falling
}
