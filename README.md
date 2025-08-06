# 🎮 Snake Game

## 📖 Introduzione Generale

Il progetto **Snake Game Enterprise** rappresenta un'implementazione moderna e sofisticata del classico gioco Snake, realizzato seguendo i principi dell'architettura software enterprise e i design pattern più avanzati. Questo sistema è stato progettato non solo per fornire un'esperienza di gioco coinvolgente, ma anche per dimostrare l'applicazione pratica di principi architetturali solidi in un contesto real-world.

L'architettura adottata segue una **Clean Architecture** stratificata, dove ogni livello ha responsabilità specifiche e ben definite. Il sistema è stato costruito utilizzando **.NET 8** e **C# 12**, sfruttando le più recenti innovazioni linguistiche come i costruttori primari, le collection expressions e i pattern matching avanzati.

---

## 🏗️ Architettura Generale del Sistema

### Panoramica Architetturale

Il sistema è organizzato secondo una **Layered Architecture** che separa nettamente le responsabilità:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│           (User Interface & Input/Output)                   │
├─────────────────────────────────────────────────────────────┤
│                    APPLICATION LAYER                        │
│         (Use Cases, Services, Orchestration)                │
├─────────────────────────────────────────────────────────────┤
│                      CORE LAYER                             │
│         (Business Logic, Game Engine, Physics)              │
├─────────────────────────────────────────────────────────────┤
│                     DOMAIN LAYER                            │
│    (Value Objects, Events, Repositories, Exceptions)        │
├─────────────────────────────────────────────────────────────┤
│                 INFRASTRUCTURE LAYER                        │
│        (External Services, Persistence, Audio)              │
└─────────────────────────────────────────────────────────────┘
```

### Principi Architetturali Fondamentali

1. **Inversione delle Dipendenze**: Tutti i layer superiori dipendono dalle astrazioni definite nei layer inferiori
2. **Separazione delle Responsabilità**: Ogni classe ha un unico scopo ben definito
3. **Immutabilità**: Ampio utilizzo di Value Objects immutabili e record types
4. **Event-Driven Design**: Comunicazione asincrona attraverso eventi di dominio
5. **Testabilità**: Architettura progettata per facilitare unit testing e integration testing

---

## 🎯 Domain Layer - Il Cuore del Business

### Value Objects: Immutabilità e Type Safety

Il **Domain Layer** è il fondamento dell'intera architettura, contenendo i concetti di business fondamentali espressi attraverso **Value Objects** immutabili.

#### Position - Coordinate Spaziali Tipizzate

La classe `Position` rappresenta un esempio perfetto di Value Object:

```csharp
public readonly struct Position(int x, int y) : IEquatable<Position>
{
    public int X { get; } = x;
    public int Y { get; } = y;
}
```

Questa implementazione sfrutta il **primary constructor** di C# 12 per garantire immutabilità fin dalla costruzione. La `Position` non è semplicemente una coppia di interi, ma un concetto di dominio che incapsula:

- **Semantica spaziale**: Rappresenta una posizione nel campo di gioco
- **Operazioni matematiche**: Supporta addizione vettoriale attraverso operator overloading
- **Conversioni naturali**: Interoperabilità con tuple native tramite implicit operators
- **Value semantics**: Equality e hash code basati sui valori, non sui riferimenti

#### Score - Punteggio con Business Rules

Il Value Object `Score` dimostra come incapsulare business logic direttamente nel dominio:

```csharp
public readonly struct Score : IEquatable<Score>, IComparable<Score>
{
    public int Value { get; }
    
    public Score(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Score cannot be negative");
        Value = value;
    }
    
    public Score Add(int points) => new(Value + points);
    public bool IsNewRecord(Score previousRecord) => Value > previousRecord.Value;
}
```

Questa implementazione garantisce che:
- **Invarianti di dominio**: Un punteggio non può mai essere negativo
- **Operazioni immutabili**: Ogni modifica produce una nuova istanza
- **Business methods**: Logica di confronto record incapsulata nel dominio

### Domain Events - Comunicazione Asincrona

Il sistema implementa il **Domain Events Pattern** per la comunicazione loose-coupled tra componenti:

```csharp
public abstract record GameEvent : IGameEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record FoodEatenEvent(Position FoodPosition, int NewScore) : GameEvent;
public record LevelUpEvent(int NewLevel) : GameEvent;
public record CollisionEvent(Position CollisionPosition, string CollisionType) : GameEvent;
```

Gli eventi utilizzano **record types** per garantire immutabilità e forniscono:
- **Tracciabilità**: Ogni evento ha ID univoco e timestamp
- **Rich data**: Informazioni complete sul contesto dell'evento
- **Type safety**: Eventi fortemente tipizzati per evitare errori runtime

### Repository Pattern - Astrazione della Persistenza

Le interfacce repository definiscono contratti puliti per l'accesso ai dati:

```csharp
public interface IRecordRepository
{
    int LoadRecord();
    void SaveRecord(int score);
    bool CheckAndUpdateRecord(int score);
}
```

Questa astrazione permette di:
- **Isolare il dominio**: Nessuna dipendenza da specifiche tecnologie di persistenza
- **Facilitare testing**: Mock facilmente implementabili
- **Sostituibilità**: Diverse implementazioni (file, database, cloud) intercambiabili

---

## ⚙️ Core Layer - Logica di Gioco e Motore

### Game Engine - Orchestrazione Principale

Il **Core Layer** contiene il motore di gioco e tutta la logica business-critical. La classe `SnakeGameEngine` rappresenta il cuore pulsante del sistema:

```csharp
public class SnakeGameEngine(
    GameConfig config,
    IGameRenderer renderer,
    IRecordManager recordManager,
    ISoundManager soundManager,
    IEventAggregator eventAggregator,
    IGameSessionManager sessionManager) : IGameEngine
```

Il motore implementa un **game loop asincrono** che:
- **Gestisce sessioni**: Delega la gestione delle singole partite al `GameSessionManager`
- **Coordina componenti**: Orchestra renderer, audio, input e persistenza
- **Gestisce lifecycle**: Supporto per start/stop graceful e cancellation token
- **Error handling**: Gestione robusta degli errori con fallback appropriati

### Physics Engine - Rilevamento Collisioni

Il sistema di fisica implementa algoritmi di collision detection ottimizzati:

```csharp
public class CollisionDetector : ICollisionDetector
{
    public bool IsOutOfBounds(Position position, int width, int height)
        => position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height;
    
    public bool IsOnObstacle(Position position, List<Position> obstacles)
        => obstacles.Contains(position);
    
    public bool IsSelfCollision(Position position, List<Position> snakeSegments)
        => snakeSegments.Contains(position);
}
```

La physics engine sfrutta:
- **Pure functions**: Metodi senza side effects per predicibile behavior
- **Optimized algorithms**: Utilizzo efficiente delle equality semantics dei Value Objects
- **Clear semantics**: Nomi di metodi che esprimono chiaramente l'intent

### Snake Entity - Gestione Stato Complesso

La classe `SnakeShape` incapsula tutta la logica di gestione del serpente:

```csharp
public class SnakeShape : ISnakeShape
{
    private readonly List<Position> _segments;
    
    public void Move(Position newHeadPosition)
    {
        _segments.Insert(0, newHeadPosition);      // Aggiungi nuova testa
        _segments.RemoveAt(_segments.Count - 1);   // Rimuovi coda
    }
    
    public void Grow(Position newHeadPosition)
    {
        _segments.Insert(0, newHeadPosition);      // Aggiungi testa, mantieni coda
    }
}
```

Questa implementazione dimostra:
- **Encapsulation**: Stato interno privato con API pubblica controllata
- **Defensive copying**: Le proprietà pubbliche restituiscono copie per preservare immutabilità
- **Efficient operations**: Algoritmi ottimizzati per le operazioni più frequenti

### Configuration System - Adattabilità Dinamica

Il sistema di configurazione implementa **adaptive behavior**:

```csharp
public class GameConfig
{
    public void AdaptToFieldSize()
    {
        var fieldSize = Width * Height;
        
        PointsPerLevel = Math.Max(
            GameConstants.LevelProgression.MinimumPointsPerLevel,
            (int)(fieldSize * GameConstants.LevelProgression.PointsPerLevelFieldPercentage));
            
        MaxCactus = Math.Max(
            GameConstants.ObstacleGeneration.MinimumObstacles,
            (int)(fieldSize * GameConstants.ObstacleGeneration.ObstacleFieldPercentage));
    }
}
```

La configurazione adattiva garantisce:
- **Scalabilità**: Il gioco si adatta automaticamente a diverse dimensioni del campo
- **Balance**: Progressione di difficoltà proporzionale alla dimensione del campo
- **Configurabilità**: Parametri facilmente modificabili per tuning del gameplay

---

## 🔄 Application Layer - Orchestrazione e Use Cases

### Application Services - Coordinamento Business Logic

Il **Application Layer** orchestrea i use cases del sistema attraverso services dedicati. Il `ScoreService` esemplifica questa responsabilabilità:

```csharp
public sealed class ScoreService(IRecordManager recordManager, IEventAggregator eventAggregator) : IScoreService
{
    private Score _currentScore = Score.Zero;
    
    public void AddPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be positive", nameof(points));

        _currentScore = _currentScore.Add(points);
        _eventAggregator.Publish(new ScoreChangedEvent(_currentScore.Value));
    }
    
    public bool UpdateRecordIfNeeded()
    {
        var currentRecord = GetRecordScore();
        if (_currentScore.Value > currentRecord.Value)
        {
            _recordManager.CheckAndUpdateRecord(_currentScore.Value);
            _eventAggregator.Publish(new NewRecordSetEvent(_currentScore.Value));
            return true;
        }
        return false;
    }
}
```

Il service coordina:
- **Domain operations**: Utilizza Value Objects per operazioni business
- **Event publishing**: Notifica cambiamenti attraverso domain events
- **Cross-cutting concerns**: Integra persistenza e notifiche

### Command Pattern - Operazioni Reversibili

Il sistema implementa il **Command Pattern** per operazioni del gioco:

```csharp
public sealed class ChangeDirectionCommand(
    GameLogic game, 
    int newDx, 
    int newDy, 
    IEventAggregator eventAggregator) : GameCommandBase(eventAggregator)
{
    private readonly (int dx, int dy) _previousDirection = game.GetDirection();
    
    public override void Execute() => _game.ChangeDirection(_newDx, _newDy);
    public override void Undo() => _game.ChangeDirection(_previousDirection.dx, _previousDirection.dy);
    public override bool CanUndo => true;
}
```

Il Command Pattern fornisce:
- **Undo/Redo capabilities**: Operazioni reversibili per migliore UX
- **Macro operations**: Possibilità di comporre comandi complessi
- **Audit trail**: Tracciamento delle operazioni per debugging

### Event Aggregator - Hub Comunicazione

L'`EnterpriseEventAggregator` implementa un sistema di messaggistica avanzato:

```csharp
public sealed class EnterpriseEventAggregator : IEventAggregator, IDisposable
{
    private readonly ConcurrentDictionary<Type, List<EventSubscription>> _subscriptions = [];
    
    public async Task PublishAsync<T>(T gameEvent, CancellationToken cancellationToken = default) 
        where T : IGameEvent
    {
        // Esecuzione parallela degli handler con gestione priorità
        var handlerTasks = subscriptionsSnapshot
            .Where(s => s.CanHandle(gameEvent))
            .Select(s => ExecuteHandlerSafelyAsync(s, gameEvent, cancellationToken));
            
        await Task.WhenAll(handlerTasks);
    }
}
```

L'event aggregator supporta:
- **Async processing**: Gestione asincrona degli eventi per performance
- **Priority handling**: Esecuzione ordinata per priorità degli handler
- **Error isolation**: Failure di un handler non compromette gli altri
- **Thread safety**: Operazioni concurrent-safe per ambiente multi-threaded

### Mediator Pattern - Decoupling Comunicazione

Il `GameMediator` implementa il Mediator Pattern per request/response:

```csharp
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetService(handlerType) ??
            throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
            
        return await ((Task<TResponse>)method.Invoke(handler, [request, cancellationToken]));
    }
}
```

Il mediator facilita:
- **Loose coupling**: Sender e receiver non si conoscono direttamente
- **Dynamic dispatch**: Risoluzione runtime degli handler appropriati
- **Cross-cutting concerns**: Gestione uniforme di logging, validation, caching

### Factory Pattern - Creazione Oggetti Complessi

Il `GameFactory` centralizza la creazione di oggetti di gioco:

```csharp
public sealed class GameFactory(...) : IGameFactory
{
    public GameLogic CreateGame()
    {
        Guard.ValidGameConfiguration(_config.Width, _config.Height);
        
        var snakeShape = CreateSnakeShape();
        return new GameLogic(_config.Width, _config.Height, _scoreService, 
            _levelService, snakeShape, _foodGenerator, _collisionDetector, 
            _obstacleManager, _eventAggregator, _gameProgressionService);
    }
}
```

La factory garantisce:
- **Consistent creation**: Oggetti sempre creati con dipendenze corrette
- **Validation**: Controlli di validità centralizzati
- **Dependency resolution**: Gestione automatica delle dipendenze

---

## 🔌 Infrastructure Layer - Servizi Esterni

### Audio System - Gestione Multimedia

Il sistema audio implementa il **Strategy Pattern** per gestire diverse modalità di output:

```csharp
public class SoundManager : ISoundManager
{
    private IMidiPlayer _currentPlayer;
    
    public void SetSoundEnabled(bool enabled)
    {
        _currentPlayer = enabled ? _midiPlayer : new SilentMidiPlayer();
    }
}
```

Il sistema audio supporta:
- **Strategy switching**: Cambio runtime tra audio attivo e silenzioso
- **Async operations**: Riproduzione non-bloccante per mantenere performance
- **Event-driven**: Risposta automatica agli eventi di gioco

### Persistence System - Gestione Dati

Il sistema di persistenza implementa **Repository** e **Unit of Work** patterns:

```csharp
public class FileRecordRepository : IRecordRepository
{
    private const string RecordFilePath = "game_record.dat";
    
    public bool CheckAndUpdateRecord(int score)
    {
        var currentRecord = LoadRecord();
        if (score > currentRecord)
        {
            SaveRecord(score);
            return true;
        }
        return false;
    }
}
```

La persistenza fornisce:
- **Abstraction**: Logica di storage nascosta dietro interfacce
- **Atomic operations**: Transazioni per operazioni multi-step
- **Error handling**: Gestione robusta di errori I/O

### Console Abstraction - Input/Output Unificato

Il sistema di I/O console astrae le operazioni di sistema:

```csharp
public interface IConsoleService
{
    ConsoleKeyInfo ReadKey(bool intercept = false);
    void SetCursorPosition(int left, int top);
    bool KeyAvailable { get; }
    Task InitializeAsync();
}
```

L'astrazione console permette:
- **Testability**: Mock dell'input/output per unit tests
- **Cross-platform**: Potenziale supporto per diverse piattaforme
- **Async initialization**: Setup non-bloccante delle risorse

---

## 🎨 Presentation Layer - Interfaccia Utente

### Rendering System - Visualizzazione Modulare

Il sistema di rendering implementa **Template Method** e **Strategy** patterns:

```csharp
public class ConsoleGameRenderer : IGameRenderer
{
    public void DrawGame(int width, int height, List<Position> snake, 
        Position food, List<Position> obstacles, int score, int level, int record)
    {
        _menuRenderer.DrawGameBoard(width, height);
        DrawSnake(snake);
        DrawFood(food);
        DrawObstacles(obstacles);
        DrawUI(score, level, record);
    }
}
```

Il rendering system supporta:
- **Modular drawing**: Componenti separati per diversi elementi di gioco
- **Template rendering**: Uso di template per layout consistenti
- **Performance optimization**: Rendering selettivo per minimizzare flickering

### Menu System - Navigazione Strutturata

Il sistema di menu utilizza **Composite Pattern** per gestire UI complesse:

```csharp
public class MenuRow
{
    public required string Text { get; init; }
    public required ConsoleColor Color { get; init; }
    public bool IsCentered { get; init; } = false;
}
```

Il menu system offre:
- **Hierarchical structure**: Menu e sottomenu componibili
- **Flexible styling**: Controllo granulare di colori e posizionamento
- **Responsive layout**: Adattamento automatico a diverse dimensioni console

---

## 🔄 Pattern di Design Implementati

### 1. Clean Architecture
- **Separation of Concerns**: Ogni layer ha responsabilità specifiche
- **Dependency Inversion**: Dipendenze puntano verso l'interno
- **Testability**: Architettura facilitante unit e integration testing

### 2. Domain-Driven Design (DDD)
- **Value Objects**: Modellazione di concetti di business immutabili
- **Domain Events**: Comunicazione di cambiamenti significativi
- **Repository Pattern**: Astrazione dell'accesso ai dati

### 3. Event-Driven Architecture
- **Event Sourcing elements**: Eventi come first-class citizens
- **Pub/Sub Pattern**: Disaccoppiamento attraverso eventi asincroni
- **Event Aggregator**: Hub centralizzato per la gestione eventi

### 4. Command Query Separation (CQS)
- **Command Pattern**: Operazioni che modificano stato
- **Query Methods**: Operazioni di sola lettura
- **Mediator Pattern**: Gestione uniforme di commands e queries

### 5. Strategy Pattern
- **Audio Strategies**: Diversi comportamenti audio (sound/silent)
- **Food Generation**: Algoritmi intercambiabili per posizionamento cibo
- **Collision Detection**: Diversi algoritmi di collision detection

### 6. State Machine Pattern
- **Game States**: Gestione stati di gioco (Running, Paused, GameOver)
- **Transition Logic**: Logica controllata per cambio stato
- **State Encapsulation**: Comportamenti specifici per ogni stato

### 7. Factory Pattern
- **Game Factory**: Creazione centralizzata di oggetti di gioco
- **Abstract Factory**: Famiglie di oggetti correlati
- **Builder Pattern elements**: Costruzione step-by-step di oggetti complessi

### 8. Observer Pattern
- **Event Aggregator**: Notifica automatica di cambiamenti
- **Domain Events**: Osservazione di eventi di business
- **Reactive Extensions**: Reattività ai cambiamenti di stato

---

## 🔗 Interazioni tra Componenti

### Flusso di Esecuzione Principale

1. **Bootstrap**: Il `Program.cs` configura DI container e inizializza host
2. **Engine Start**: `SnakeGameEngine` avvia il game loop principale
3. **Session Management**: `GameSessionManager` gestisce singole partite
4. **Game Loop**: `GameRunner` esegue il ciclo render-input-update
5. **Business Logic**: `GameLogic` processa regole di gioco e collisioni
6. **Event Processing**: `EventAggregator` propaga eventi ai listeners
7. **Presentation**: `ConsoleGameRenderer` aggiorna la visualizzazione

### Comunicazione Cross-Layer

```
User Input → InputHandler → GameRunner → GameLogic → Domain Events
                                ↓
Domain Events → EventAggregator → Application Services → Infrastructure
                                ↓
Infrastructure → Persistence/Audio → External Resources
```

### Dependency Injection Flow

Il sistema utilizza **Microsoft.Extensions.DependencyInjection** con extension methods per ogni layer:

```csharp
services.AddCoreServices()          // Physics, Game Engine, Configuration
        .AddApplicationServices()   // Use Cases, Services, Factories  
        .AddInfrastructureServices()// Persistence, Audio, Console
        .AddPresentationServices(); // Rendering, Menu System
```

---

## 🎯 Caratteristiche Tecniche Avanzate

### Utilizzo di C# 12 e .NET 8

Il progetto sfrutta le più recenti innovazioni del linguaggio:

- **Primary Constructors**: Sintassi concisa per dependency injection
- **Collection Expressions**: Inizializzazione elegante di collezioni con `[]`
- **Pattern Matching**: Switch expressions per logica decisionale
- **Record Types**: Immutabilità built-in per Value Objects e Events
- **Global Using**: Semplificazione delle dichiarazioni using
- **File-scoped Namespaces**: Riduzione dell'indentazione

### Performance e Ottimizzazioni

- **Struct Value Objects**: Riduzione allocazioni heap per oggetti frequenti
- **Async/Await**: Operazioni non-bloccanti per I/O e rendering
- **Concurrent Collections**: Thread-safety per scenari multi-threaded
- **Object Pooling elements**: Riutilizzo di oggetti per performance
- **Efficient Algorithms**: Algoritmi ottimizzati per collision detection

### Error Handling e Resilienza

- **Exception Hierarchy**: Eccezioni tipizzate per diversi scenari
- **Graceful Degradation**: Fallback behavior in caso di errori
- **Cancellation Token**: Supporto per operazioni cancellabili
- **Defensive Programming**: Validation e null-checking sistematici

---

## 📊 Metriche e Qualità del Codice

### Separazione delle Responsabilità
- **Domain Layer**: 15 classi (Value Objects, Events, Contracts)
- **Core Layer**: 25 classi (Game Engine, Physics, Configuration)
- **Application Layer**: 20 classi (Services, Use Cases, Orchestration)
- **Infrastructure Layer**: 18 classi (External Services, I/O, Persistence)
- **Presentation Layer**: 12 classi (Rendering, UI Components)

### Astrazione e Interfacce
- **90+ interfacce**: Quasi ogni componente pubblico ha un'interfaccia
- **Dependency Inversion**: Nessuna dipendenza da implementazioni concrete
- **Testability**: Ogni componente facilmente mockabile per testing

### Immutabilità e Thread Safety
- **Value Objects**: Tutti immutabili per design
- **Record Types**: Eventi immutabili con structural equality
- **Concurrent Collections**: Strutture dati thread-safe dove necessario
- **Pure Functions**: Metodi senza side effects dove possibile

---

## 🚀 Estendibilità e Manutenibilità

### Punti di Estensione

Il sistema è progettato per essere facilmente estendibile:

1. **Nuovi Game Elements**: Aggiunta di power-ups, nemici, bonus
2. **Algoritmi AI**: Implementazione di bot players
3. **Multiplayer Support**: Estensione per gioco multi-giocatore
4. **Different Renderers**: Web, WPF, mobile renderers
5. **Persistence Backends**: Database, cloud storage, cache
6. **Input Methods**: Gamepad, touch, network input

### Modalità di Estensione

- **Strategy Pattern**: Nuovi algoritmi intercambiabili
- **Event System**: Nuovi handler per eventi esistenti
- **Dependency Injection**: Sostituzione servizi esistenti
- **Interface Segregation**: Implementazione parziale di funzionalità
- **Composition**: Combinazione di comportamenti esistenti

---

## 📝 Conclusioni

Il progetto **Snake Game Enterprise** rappresenta un esempio eccellente di come i principi dell'ingegneria software moderna possano essere applicati anche a progetti apparentemente semplici. L'architettura implementata dimostra:

### Punti di Forza
- **Scalabilità**: Architettura che supporta crescita di complessità
- **Manutenibilità**: Codice organizzato e facilmente comprensibile
- **Testabilità**: Struttura che facilita testing automatizzato
- **Riusabilità**: Componenti modulari riutilizzabili in altri contesti
- **Performance**: Ottimizzazioni per gaming real-time

### Lezioni Apprese
- **Over-engineering awareness**: Bilanciamento tra robustezza e semplicità
- **Pattern application**: Uso appropriato di design patterns
- **Modern C# features**: Sfruttamento delle novità linguistiche
- **Clean code principles**: Codice leggibile e auto-documentante

### Valore Formativo
Questo progetto serve come:
- **Reference implementation**: Esempio di architettura enterprise
- **Learning platform**: Studio di design patterns in azione
- **Best practices showcase**: Dimostrazione di principi SOLID
- **Modern .NET**: Utilizzo di tecnologie e linguaggi aggiornati

Il sistema dimostra che anche un gioco semplice può beneficiare di un'architettura sofisticata, risultando in un codebase maintainable, testable ed extensible che può servire come base per progetti più complessi o come piattaforma di apprendimento per sviluppatori che vogliono approfondire i principi dell'architettura software moderna.
