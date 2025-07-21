#if TOOLS
using Godot;

namespace GodotFloorLevels.ChunkManager;

[Tool]
public partial class ChunkEditorPlugin : EditorPlugin
{
	public static ChunkEditorPlugin Instance { get; private set; }

	public override void _EnterTree()	
	{
		Instance = this;
		
		// Criar e adicionar o dock do editor de chunks
		GD.Print("ChunkManager Plugin ativado!");
	}

	public override void _ExitTree()
	{
		GD.Print("ChunkManager Plugin desativado!");
	}
}
#endif
