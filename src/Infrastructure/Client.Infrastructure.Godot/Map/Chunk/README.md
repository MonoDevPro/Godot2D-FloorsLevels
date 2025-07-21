.# 🗺️ Sistema de Chunks para Godot 4.4

Sistema robusto de carregamento dinâmico de chunks para mundos 2D infinitos em Godot 4.4 com C#.

## 📋 Características

### ✅ **Funcionalidades Principais**
- **Carregamento dinâmico** baseado na posição do player
- **Cache inteligente** com persistência em disco
- **Geração procedural** com múltiplos biomas
- **Sistema de priorização** para otimizar performance
- **Level of Detail (LOD)** automático
- **Debug visual** completo
- **API pública** para controle manual

### 🚀 **Performance Otimizada**
- Carregamento assíncrono (não trava o jogo)
- Limite de chunks carregados por frame
- Cache em memória para acesso rápido
- Unload delay para evitar carregamento constante
- Sistema de priorização inteligente

### 🎮 **Facilidade de Uso**
- Configuração simples via editor do Godot
- Sistema de sinais para comunicação externa
- Exemplo de uso completo incluído
- Debug visual com teclas de atalho (F1-F7)

## 🏗️ Arquitetura do Sistema

```
ChunkManager (Núcleo)
├── ChunkCache (Persistência)
├── ChunkGenerator (Geração Procedural)
├── ChunkPrioritizer (Otimização)
├── ChunkNotifier (Detecção)
└── ChunkDebugVisualizer (Debug)
```

### 📦 **Componentes**

| Componente | Função |
|------------|--------|
| `ChunkManager` | Gerenciador central - coordena todo o sistema |
| `ChunkData` | Estrutura de dados serializada para chunks |
| `ChunkCache` | Sistema de cache com persistência em disco |
| `ChunkGenerator` | Gerador procedural com biomas |
| `ChunkPrioritizer` | Sistema de priorização baseado em distância |
| `ChunkNotifier` | Detecção de visibilidade de chunks |
| `ChunkLODManager` | Level of Detail para performance |
| `ChunkDebugVisualizer` | Interface visual de debug |

## 🔧 Configuração Rápida

### 1. **Estrutura da Cena**
```
Main
├── Player (grupo: "player")
├── ChunkManager
└── ChunkDebugVisualizer (CanvasLayer)
```

### 2. **Configuração do ChunkManager**
```csharp
// No editor do Godot ou via código
ChunkManager.ChunkSize = 16;           // 16x16 tiles por chunk
ChunkManager.TileSize = 32;            // 32x32 pixels por tile
ChunkManager.MaxLoadedChunks = 9;      // Máximo 9 chunks (3x3 grid)
ChunkManager.NotifierRange = 3;        // Range de detecção
ChunkManager.UnloadDelay = 2.0f;       // Delay antes de descarregar
ChunkManager.EnableProcGen = true;     // Ativar geração procedural
```

### 3. **Player Setup**
```csharp
// Certifique-se que o player está no grupo "player"
AddToGroup("player");
```

## 🎲 Sistema de Geração Procedural

### **Biomas Suportados**
- 🌊 **Oceano** - Águas profundas e rasas
- 🏖️ **Praia** - Transição entre água e terra
- 🌱 **Planície** - Grama baixa e alta
- 🌲 **Floresta** - Árvores e vegetação densa
- ⛰️ **Montanha** - Rochas e terreno elevado

### **Configuração do Gerador**
```csharp
// Personalizar geração
ChunkManager.ProcGenSeed = 12345;      // Seed do mundo
ChunkManager.UpdateGeneratorSeed(54321); // Mudar seed em runtime
```

## 💾 Sistema de Cache

### **Cache em Memória**
- Mantém até 25 chunks em memória por padrão
- Algoritmo LRU (Least Recently Used)
- Acesso instantâneo para chunks frequentes

### **Persistência em Disco**
- Salva chunks em `user://chunks/`
- Formato JSON para fácil depuração
- Carregamento automático entre sessões

### **Controle de Cache**
```csharp
// Limpar cache
chunkManager.GetChunkCache().ClearMemoryCache();

// Verificar status
bool inCache = chunkManager.GetChunkCache().IsInMemoryCache(coord);
int cacheSize = chunkManager.GetMemoryCacheSize();
```

## 🎯 Sistema de Priorização

O sistema prioriza chunks baseado em:
- **Distância do player** (mais próximo = maior prioridade)
- **Chunks centrais** (bônus para chunks ao redor do player)
- **Chunks importantes** (marcados como especiais)
- **Penalidades** para chunks muito distantes

## 🐛 Sistema de Debug

### **Teclas de Debug**
| Tecla | Função |
|-------|--------|
| F1 | Toggle informações de debug |
| F2 | Toggle bordas dos chunks |
| F3 | Toggle estatísticas de performance |
| F4 | Imprimir status dos chunks no console |
| F5 | Executar exemplo de uso da API |
| F6 | Imprimir status detalhado |
| F7 | Regenerar mundo com nova seed |

### **Informações Exibidas**
- Chunks carregados vs máximo
- Uso do cache em memória
- Notificadores ativos
- FPS e frame time
- Estatísticas de performance

## 📚 API Pública

### **Controle Manual de Chunks**
```csharp
// Forçar carregamento
await chunkManager.ForceLoadChunk(new Vector2I(5, 5));

// Forçar descarregamento
chunkManager.ForceUnloadChunk(new Vector2I(5, 5));

// Verificar se está carregado
bool loaded = chunkManager.IsChunkLoaded(new Vector2I(0, 0));

// Obter chunk node
Node2D chunkNode = chunkManager.GetChunk(new Vector2I(0, 0));

// Obter dados do chunk
ChunkData data = chunkManager.GetChunkData(new Vector2I(0, 0));
```

### **Configuração em Runtime**
```csharp
// Alterar limite de chunks
chunkManager.SetMaxLoadedChunks(16);

// Mudar seed da geração
chunkManager.UpdateGeneratorSeed(99999);

// Ativar debug
chunkManager.EnableDebugMode(true);
```

### **Informações do Sistema**
```csharp
// Estatísticas
int loadedCount = chunkManager.GetLoadedChunkCount();
int cacheSize = chunkManager.GetMemoryCacheSize();
int notifierCount = chunkManager.GetActiveNotifierCount();

// Listas de coordenadas
List<Vector2I> loadedChunks = chunkManager.GetLoadedChunkCoords();
List<Vector2I> notifiers = chunkManager.GetActiveNotifierCoords();
```

## 🔗 Sinais (Signals)

Conecte-se aos sinais para reagir a eventos do sistema:

```csharp
// Conectar sinais
chunkManager.ChunkLoaded += OnChunkLoaded;
chunkManager.ChunkUnloaded += OnChunkUnloaded;
chunkManager.ChunkGenerated += OnChunkGenerated;

// Handlers
private void OnChunkLoaded(Vector2I coord, Node2D node, ChunkData data)
{
    // Chunk foi carregado - spawnar inimigos, ativar sistemas, etc.
}

private void OnChunkUnloaded(Vector2I coord)
{
    // Chunk foi descarregado - salvar dados importantes
}

private void OnChunkGenerated(Vector2I coord, ChunkData data)
{
    // Novo chunk foi gerado - adicionar estruturas especiais
}
```

## 🔧 Personalização Avançada

### **Modificar Geração de Biomas**
```csharp
// Em ChunkGenerator.cs, modificar GetTileIdForTerrain()
private int GetTileIdForTerrain(float terrainValue, BiomeType biome)
{
    return biome switch
    {
        BiomeType.Ocean => terrainValue < -0.5f ? 1 : 2,
        BiomeType.Beach => 3,
        // Adicione seus próprios tipos de tile aqui
        _ => 4
    };
}
```

### **Adicionar Novos Biomas**
```csharp
// Adicionar ao enum BiomeType
public enum BiomeType
{
    Ocean, Beach, Grassland, Forest, Mountain,
    Desert, Swamp, Tundra // Novos biomas
}
```

### **Customizar Criação Visual**
```csharp
// Em ChunkManager.CreateChunkNode(), modificar:
private async Task<Node2D> CreateChunkNode(Vector2I chunkCoord, ChunkData chunkData)
{
    var chunkNode = new Node2D();
    
    // Sua lógica de criação visual aqui
    // Exemplo: usar TileMap, Sprite2D, etc.
    
    return chunkNode;
}
```

## ⚡ Otimizações de Performance

### **Configurações Recomendadas**
```csharp
// Para jogos com muita movimentação
ChunkManager.ChunkUpdateInterval = 0.3f;  // Atualizar mais frequentemente
ChunkManager.MaxLoadedChunks = 16;        // Mais chunks carregados
ChunkManager.UnloadDelay = 1.0f;          // Delay menor

// Para jogos com pouca movimentação
ChunkManager.ChunkUpdateInterval = 1.0f;  // Atualizar menos frequentemente
ChunkManager.MaxLoadedChunks = 9;         // Menos chunks carregados
ChunkManager.UnloadDelay = 3.0f;          // Delay maior
```

### **Monitoramento de Performance**
- Use F3 para ver estatísticas em tempo real
- Monitore FPS e frame time
- Ajuste `_maxChunksPerFrame` se necessário
- Use o cache eficientemente

## 🚨 Solução de Problemas

### **Player não encontrado**
```
ERRO: "Player não encontrado! Inicializando notificadores na origem."
```
**Solução:** Certifique-se que o player está no grupo "player":
```csharp
AddToGroup("player");
```

### **Chunks não carregam**
- Verifique se `EnableProcGen` está ativo
- Confirme se o player está se movendo
- Use F4 para debug do status dos chunks

### **Performance baixa**
- Reduza `MaxLoadedChunks`
- Aumente `ChunkUpdateInterval`
- Reduza `_maxChunksPerFrame`
- Use F3 para monitorar performance

### **Cache não funciona**
- Verifique permissões de escrita em `user://`
- Confirme se o diretório de cache foi criado
- Use debug para verificar status do cache

## 🎯 Exemplo de Uso Completo

Veja `ChunkSystemExample.cs` para um exemplo completo de como usar o sistema.

## 📈 Próximos Passos

Possíveis melhorias futuras:
- 🔄 Multi-threading para geração de chunks
- 🌐 Streaming de chunks via rede
- 📊 Múltiplos layers de chunks
- 🎨 Sistema de LOD visual mais avançado
- 💾 Compressão de dados de chunk
