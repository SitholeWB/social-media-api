# API Documentation

The SocialMedia API is a RESTful interface built with ASP.NET Core. It uses standard HTTP methods and status codes.

## Controllers Overview

| Controller | Description | Base Path |
|------------|-------------|-----------|
| **Auth** | User registration and login. | `/api/v1/Auth` |
| **Posts** | Manage posts (read, report, delete). | `/api/v1/Posts` |
| **Comments** | Manage comments on posts. | `/api/v1/Comments` |
| **Likes** | Like/unlike posts and comments. | `/api/v1/reactions` |
| **Polls** | Create and vote on polls. | `/api/v1/Polls` |
| **Users** | User profile management. | `/api/v1/Users` |
| **Groups** | Group management and group-specific posts. | `/api/v1/Groups` |
| **Notifications** | User notifications. | `/api/v1/Notifications` |
| **Moderation** | Content moderation tools. | `/api/v1/Moderation` |
| **Reports** | Handling user reports. | `/api/v1/Reports` |
| **Defaults** | System initialization and defaults. | `/api/v1/Defaults` |
| **Stats** | Dashboard and usage statistics. | `/api/v1/Stats` |

## Authentication Flow

The API uses JWT (JSON Web Tokens) for authentication.

### Login Sequence

```mermaid
sequenceDiagram
    participant Client
    participant API as AuthController
    participant Dispatcher
    participant Handler as LoginCommandHandler
    participant Repo as UserRepository
    participant Token as TokenService

    Client->>API: POST /api/v1/Auth/login
    API->>Dispatcher: Send(LoginCommand)
    Dispatcher->>Handler: Handle(LoginCommand)
    Handler->>Repo: GetByEmail(email)
    Repo-->>Handler: User Entity
    Handler->>Handler: Verify Password
    Handler->>Token: GenerateToken(User)
    Token-->>Handler: JWT Token
    Handler-->>Dispatcher: AuthResponse
    Dispatcher-->>API: AuthResponse
    API-->>Client: 200 OK (AuthResponse)
```

## Content Flow

### Create Post Sequence

Posts are created within the context of a Group.

```mermaid
sequenceDiagram
    participant Client
    participant API as GroupsController
    participant Dispatcher
    participant Handler as CreatePostCommandHandler
    participant Repo as PostRepository
    participant DB as Database

    Client->>API: POST /api/v1/groups/{groupId}/posts
    API->>Dispatcher: Send(CreatePostCommand)
    Dispatcher->>Handler: Handle(CreatePostCommand)
    Handler->>Handler: Validate Request
    Handler->>Repo: Add(Post)
    Repo->>DB: SaveChangesAsync()
    DB-->>Repo: Success
    Repo-->>Handler: PostId
    Handler-->>Dispatcher: PostId
    Dispatcher-->>API: PostId
    API-->>Client: 201 Created (Location Header)
```

## Detailed Request Flows

### Create Post with Event Processing

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Dispatcher
    participant CommandHandler
    participant Repository
    participant DB
    participant EventProcessor
    participant EventHandler
    participant ReadRepo
    participant ReadDB

    Client->>Controller: POST /api/v1/groups/{groupId}/posts
    Controller->>Dispatcher: Send(CreatePostCommand)
    Dispatcher->>CommandHandler: Handle(command)
    CommandHandler->>Repository: AddAsync(post)
    Repository->>DB: Save Post
    DB-->>Repository: Post Saved
    
    CommandHandler->>Dispatcher: Publish(PostCreatedEvent)
    Dispatcher->>EventProcessor: EnqueueEventAsync(event)
    EventProcessor->>DB: Save OutboxEvent
    EventProcessor-->>Dispatcher: Enqueued
    
    CommandHandler-->>Controller: Post ID
    Controller-->>Client: 201 Created
    
    Note over EventProcessor: Background Processing
    EventProcessor->>DB: Poll Pending Events
    DB-->>EventProcessor: PostCreatedEvent
    EventProcessor->>EventHandler: Handle(PostCreatedEvent)
    EventHandler->>ReadRepo: AddAsync(PostReadModel)
    ReadRepo->>ReadDB: Save Read Model
    EventHandler-->>EventProcessor: Success
    EventProcessor->>DB: Mark Event Completed
```

### Get Posts Query

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Dispatcher
    participant QueryHandler
    participant ReadRepo
    participant ReadDB

    Client->>Controller: GET /api/v1/groups/{groupId}/posts?page=1&pageSize=10
    Controller->>Dispatcher: Query(GetPostsQuery)
    Dispatcher->>QueryHandler: Handle(query)
    QueryHandler->>ReadRepo: GetLatestAsync(page, pageSize)
    ReadRepo->>ReadDB: SELECT * FROM PostReads
    ReadDB-->>ReadRepo: PostReadModels
    QueryHandler->>ReadRepo: GetTotalCountAsync()
    ReadRepo->>ReadDB: SELECT COUNT(*)
    ReadDB-->>ReadRepo: Count
    QueryHandler-->>Dispatcher: PagedResult<PostDto>
    Dispatcher-->>Controller: PagedResult
    Controller-->>Client: 200 OK + Posts
```

### Add Comment Flow

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Dispatcher
    participant CommandHandler
    participant CommentRepo
    participant DB
    participant EventProcessor
    participant EventHandler
    participant ReadRepo

    Client->>Controller: POST /api/v1/comments
    Controller->>Dispatcher: Send(CreateCommentCommand)
    Dispatcher->>CommandHandler: Handle(command)
    
    CommandHandler->>CommentRepo: AddAsync(comment)
    CommentRepo->>DB: Save Comment
    
    CommandHandler->>Dispatcher: Publish(CommentAddedEvent)
    Dispatcher->>EventProcessor: EnqueueEventAsync(event)
    
    CommandHandler-->>Controller: Comment ID
    Controller-->>Client: 201 Created
    
    Note over EventProcessor: Background Processing
    EventProcessor->>EventHandler: Handle(CommentAddedEvent)
    EventHandler->>ReadRepo: AddAsync(CommentReadModel)
    EventHandler->>ReadRepo: UpdateAsync(PostReadModel)
    Note over EventHandler: Update Post Stats & TopComments
```

### Toggle Like Flow

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Dispatcher
    participant CommandHandler
    participant LikeRepo
    participant DB
    participant EventProcessor
    participant EventHandler
    participant ReadRepo

    Client->>Controller: POST /api/v1/reactions/toggle
    Controller->>Dispatcher: Send(ToggleLikeCommand)
    Dispatcher->>CommandHandler: Handle(command)
    
    CommandHandler->>LikeRepo: GetByUserAndTargetAsync()
    LikeRepo->>DB: Find Existing Like
    
    alt Like Exists
        CommandHandler->>LikeRepo: DeleteAsync(like)
        CommandHandler-->>Controller: false (unliked)
    else Like Not Exists
        CommandHandler->>LikeRepo: AddAsync(like)
        CommandHandler->>Dispatcher: Publish(LikeAddedEvent)
        Dispatcher->>EventProcessor: EnqueueEventAsync(event)
        CommandHandler-->>Controller: true (liked)
    end
    
    Controller-->>Client: 200 OK
    
    Note over EventProcessor: Background Processing
    EventProcessor->>EventHandler: Handle(LikeAddedEvent)
    EventHandler->>ReadRepo: UpdateAsync(ReadModel)
    Note over EventHandler: Add Reaction, Update LikeCount
```

### Error Handling

```mermaid
sequenceDiagram
    participant Client
    participant Middleware
    participant Controller
    participant Dispatcher
    participant Handler

    Client->>Middleware: HTTP Request
    Middleware->>Controller: Process Request
    Controller->>Dispatcher: Send Command/Query
    Dispatcher->>Handler: Handle
    
    alt Validation Error
        Dispatcher-->>Controller: ValidationException
        Controller-->>Middleware: ValidationException
        Middleware-->>Client: 400 Bad Request + Errors
    else Not Found
        Handler-->>Dispatcher: NotFoundException
        Dispatcher-->>Controller: NotFoundException
        Controller-->>Middleware: NotFoundException
        Middleware-->>Client: 404 Not Found
    else Database Error
        Handler-->>Dispatcher: DbUpdateException
        Dispatcher-->>Controller: Exception
        Controller-->>Middleware: Exception
        Middleware->>Middleware: Log Error
        Middleware-->>Client: 500 Internal Server Error
    else Success
        Handler-->>Dispatcher: Result
        Dispatcher-->>Controller: Result
        Controller-->>Middleware: Response
        Middleware-->>Client: 200 OK + Data
    end
```

## Response Formats

All API responses follow a consistent structure with appropriate HTTP status codes.

## Group Access Control

The API enforces access rules based on the `GroupType`:

| Group Type | Viewing Posts | Creating Posts | Membership |
|------------|---------------|----------------|------------|
| **Everyone** | Anyone | Anyone | Open |
| **Public** | Anyone | Members Only | Auto-join (if enabled) |
| **Private** | Members Only | Members Only | Invitation/Approval |

### Implementation Details

- **Viewing Posts**: Handled in `GetPostsQueryHandler`. For **Private** groups, the `UserId` must be provided in the query and the user must be a member.
- **Creating Posts**: Handled in `CreatePostCommandHandler`. For **Public** and **Private** groups, the user must be a member to create a post.
- **Group Creation**: The user creating a group is automatically assigned as the `CreatorId`.
