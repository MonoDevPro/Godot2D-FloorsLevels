# Sistema ECS com Sincronização Godot - Documentação

## Visão Geral
Este projeto implementa uma arquitetura ECS (Entity Component System) usando ArchECS integrada com Godot 4, mantendo baixo acoplamento e alta manutenibilidade.

## Arquitetura

### Componentes ECS
- **PositionComponent**: Posição da entidade no grid
- **VelocityComponent**: Velocidade de movimento (tiles por segundo)
- **MovementComponent**: Estado atual do movimento
- **InputComponent**: Comandos de input pendentes
- **GodotNodeComponent**: Vinculação com nodes do Godot
- **CharacterTag**: Marca entidades controláveis

### Sistemas ECS
1. **InputSystem**: Captura inputs WASD/Setas e converte em comandos
2. **MovementSystem**: Processa movimentos baseados em grid 32x32
3. **GodotSyncSystem**: Sincroniza ECS → Godot com interpolação suave

### Sincronização ECS-Godot
- **Unidirecional**: ECS controla a lógica, Godot apenas renderiza
- **Interpolação**: Movimento suave entre tiles usando easing
- **Desacoplamento**: Zero dependência do Godot no core ECS

## Uso

### 1. Configuração Automática
O DIContainer (Autoload) configura automaticamente:
- Inputs (WASD + Setas direcionais)
- WorldECS e sistemas
- Loop de atualização

### 2. Criando um Personagem
```csharp
public partial class PlayerNode : GodotBody2D
{
    [Export] public float MovementSpeed { get; set; } = 3f;
    
    protected override void CreateEcsEntity()
    {
        var gridPos = CreateGridPosition(GlobalPosition);
        EcsEntity = CharacterECS.CreatePlayer(WorldECS.World, this, gridPos, MovementSpeed);
    }
}
```

### 3. Movimentação Grid 32x32
- Tiles de 32x32 pixels
- Movimento discreto entre tiles
- Validação de limites do grid
- Interpolação suave para visual fluido

## Value Objects (Core)
- **Position**: Coordenadas do grid com operações matemáticas
- **GridPosition**: Position + Grid com validação de limites
- **MovementAction**: Ação de movimento com duração e velocidade
- **Speed**: Velocidade em tiles por segundo

## Benefícios da Arquitetura
1. **Separação clara**: Core → ECS → Godot
2. **Testabilidade**: Lógica isolada do engine
3. **Performance**: Sistema ECS otimizado
4. **Flexibilidade**: Fácil adição de novos sistemas/componentes
5. **Manutenibilidade**: Responsabilidades bem definidas
