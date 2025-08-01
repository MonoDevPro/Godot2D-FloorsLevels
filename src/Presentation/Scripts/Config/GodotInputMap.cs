using Godot;

namespace GodotFloorLevels.Scripts.Config;

/// <summary>
/// Gerenciador de mapeamento de inputs do Godot
/// </summary>
public static class GodotInputMap
{
    public const string MOVE_UP = "move_up";
    public const string MOVE_DOWN = "move_down";
    public const string MOVE_LEFT = "move_left";
    public const string MOVE_RIGHT = "move_right";
    
    /// <summary>
    /// Configura os mapeamentos de input padrão
    /// </summary>
    public static void SetupDefaultInputs()
    {
        // Configura as teclas de movimento
        CreateInputAction(MOVE_UP, Key.W, Key.Up);
        CreateInputAction(MOVE_DOWN, Key.S, Key.Down);
        CreateInputAction(MOVE_LEFT, Key.A, Key.Left);
        CreateInputAction(MOVE_RIGHT, Key.D, Key.Right);
    }
    
    public static Vector2 GetMovementInput()
    {
        // Obtém o vetor de movimento baseado nos inputs configurados
        return new Vector2(
            Input.GetActionStrength(MOVE_RIGHT) - Input.GetActionStrength(MOVE_LEFT),
            Input.GetActionStrength(MOVE_DOWN) - Input.GetActionStrength(MOVE_UP)
        );
    }
    
    private static void CreateInputAction(string actionName, params Key[] keys)
    {
        if (!InputMap.HasAction(actionName))
        {
            InputMap.AddAction(actionName);
            
            foreach (var key in keys)
            {
                var inputKey = new InputEventKey();
                inputKey.Keycode = key;
                InputMap.ActionAddEvent(actionName, inputKey);
            }
            
            GD.Print($"Input action '{actionName}' configurado com teclas: {string.Join(", ", keys)}");
        }
    }
}
