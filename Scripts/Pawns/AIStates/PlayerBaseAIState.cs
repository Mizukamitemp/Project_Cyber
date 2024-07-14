using UnityEngine;

public abstract class PlayerBaseAIState
{
public abstract void EnterAIState(PlayerStateManager player);
public abstract void ExitAIState(PlayerStateManager player);
public abstract void UpdateAIState(PlayerStateManager player);
}
