using Godot;

namespace Game.Shared.Scripts.ECS.Components
{
    // --- Componentes de Dados Principais ---

    /// <summary>
    /// Armazena a posição lógica da entidade no mundo do jogo.
    /// Esta é a fonte da verdade para a posição.
    /// </summary>
    public struct PositionComponent { public Vector2 Value; }

    /// <summary>
    /// Armazena a velocidade lógica atual da entidade.
    /// </summary>
    public struct VelocityComponent { public Vector2 Value; }

    /// <summary>
    /// Armazena a velocidade de movimento base da entidade.
    /// </summary>
    public struct SpeedComponent { public float Value; }
    
    /// <summary>
    /// Armazena o estado de input atual da entidade após ser processado.
    /// É resetado após ser aplicado à velocidade.
    /// </summary>
    public struct InputComponent { public Vector2 Value; }

    
    // --- Componentes de Comando ---

    /// <summary>
    /// Um comando temporário que representa uma requisição de input a ser processada.
    /// É criado pelo sistema de rede (no servidor) ou pelo sistema de input local (no cliente).
    /// </summary>
    public struct InputRequestCommand { public Vector2 Value; }
    
    
    // --- Componentes de Referência de Cena ---
    
    /// <summary>
    /// Referência a um corpo físico Godot que representa a entidade no mundo 2D.
    /// Usado para sincronizar a física entre o ECS e Godot.
    /// </summary>
    public struct SceneBodyRefComponent { public CharacterBody2D Value; }

    
    // --- Componentes de Tag (Marcadores) ---

    /// <summary>
    /// Marca uma entidade como sendo sincronizada pela rede e fornece seu ID de rede.
    /// </summary>
    public struct NetworkedTag { public int Id; }

    /// <summary>
    /// Marca a entidade que é controlada diretamente pelo jogador local.
    /// Usado no cliente para aplicar predição e processar input local.
    /// </summary>
    public struct PlayerControllerTag { }

    /// <summary>
    /// Marca uma entidade que representa outro jogador conectado (um "proxy").
    /// Usado no cliente para aplicar interpolação de estado em vez de predição.
    /// </summary>
    public struct RemoteProxyTag { }
}
