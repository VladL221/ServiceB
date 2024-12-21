# Service B - Graph Storage Service

This service handles graph storage operations and provides a WebSocket interface for communication with Service A.

## Design Patterns & SOLID Principles

### SOLID Principles Implementation
1. **Single Responsibility Principle**
   - `JsonStorageStrategy`: Handles only storage operations
   - `GraphOperationHandler`: Manages graph operations
   - `WebSocketMessageHandler`: Handles WebSocket communication

2. **Open/Closed Principle**
   - Storage strategy pattern allows adding new storage implementations
   - Message handling can be extended without modifying existing code

3. **Liskov Substitution Principle**
   - Storage strategies can be swapped without affecting the rest of the application
   - All implementations follow the defined interfaces

4. **Interface Segregation Principle**
   - `IStorageStrategy`: Focused on storage operations
   - `IGraphOperationHandler`: Specific to graph operations
   - `IWebSocketMessageHandler`: Handles only WebSocket messages

5. **Dependency Inversion Principle**
   - All components depend on abstractions
   - Dependencies injected through constructors

### Design Patterns
1. **Strategy Pattern**
   - `IStorageStrategy` interface
   - `JsonStorageStrategy` implementation
   - Allows for different storage implementations (JSON, Database, etc.)

2. **Singleton Pattern**
   - `JsonStorageStrategy` registered as singleton
   - Ensures consistent storage access across the application

## Project Structure
```
ServiceB/
├── Interfaces/
│   ├── IGraphOperationHandler.cs
│   ├── IStorageStrategy.cs
│   └── IWebSocketMessageHandler.cs
├── Models/
│   ├── Edge.cs
│   ├── Graph.cs
│   ├── Node.cs
│   ├── WebSocketMessage.cs
│   └── WebSocketResponse.cs
└── Services/
    ├── GraphOperationHandler.cs
    ├── JsonStorageStrategy.cs
    └── WebSocketMessageHandler.cs
```

## How to Run

1. Configure ports in `launchSettings.json`:
```json
{
    "profiles": {
        "http": {
            "applicationUrl": "http://localhost:5002"
        }
    }
}
```

2. Start the service:
```bash
dotnet run
```

## WebSocket Testing with Postman

1. Connect to WebSocket:
   - URL: `ws://localhost:5002/ws`

2. Send messages:

### Create Graph
```json
{
    "action": "create",
    "data": {
        "node": {
            "label": "WebSocket Node"
        },
        "edges": [
            {
                "sourceNodeId": "00000000-0000-0000-0000-000000000000",
                "targetNodeId": "00000000-0000-0000-0000-000000000001",
                "label": "WebSocket Edge 1"
            },
            {
                "sourceNodeId": "00000000-0000-0000-0000-000000000000",
                "targetNodeId": "00000000-0000-0000-0000-000000000002",
                "label": "WebSocket Edge 2"
            }
        ]
    }
}
```

### Get Graph
```json
{
    "action": "get",
    "data": "your-graph-id-here"
}
```

### Update Graph
```json
{
    "action": "update",
    "data": {
        "id": "your-graph-id-here",
        "node": {
            "label": "Updated WebSocket Node"
        },
        "edges": [
            {
                "sourceNodeId": "00000000-0000-0000-0000-000000000000",
                "targetNodeId": "00000000-0000-0000-0000-000000000001",
                "label": "Updated Edge 1"
            },
            {
                "sourceNodeId": "00000000-0000-0000-0000-000000000000",
                "targetNodeId": "00000000-0000-0000-0000-000000000002",
                "label": "Updated Edge 2"
            }
        ]
    }
}
```

### Delete Graph
```json
{
    "action": "delete",
    "data": "your-graph-id-here"
}
```

## Storage
- Graphs are stored in a local `graphs.json` file
- Thread-safe file operations using SemaphoreSlim
- JSON storage can be replaced with other implementations

## Dependencies
- .NET 8.0
- System.Text.Json for JSON handling

## Important Notes
- WebSocket server must be running before Service A can connect
- All graph operations are persisted to the JSON file
- Thread-safe storage operations for concurrent access
- JSON storage is used for simplicity, can be replaced with database