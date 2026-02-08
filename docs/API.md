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
| **Stats** | Dashboard and usage statistics (history, weekly, monthly). | `/api/v1/Stats` |
| **Feedback** | User feedback submission. | `/api/v1/feedback` |
| **Recommendations** | AI-powered post recommendations based on vector similarity. | `/api/v1/recommendations` |

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

| Group Type | Viewing Posts/Polls | Creating Posts/Polls | Voting on Polls | Membership |
|------------|---------------|----------------|----------------|------------|
| **Everyone** | Anyone | Anyone | Anyone | Open |
| **Public** | Anyone | Members Only | Members Only | Auto-join (if enabled) |
| **Private** | Members Only | Members Only | Members Only | Invitation/Approval |

### Implementation Details

- **Viewing Posts/Polls**: Handled in `GetPostsQueryHandler` and `GetActivePollsQueryHandler` / `GetPollQueryHandler`. For **Private** groups, the `UserId` must be provided and the user must be a member.
- **Creating Posts/Polls**: Handled in `CreatePostCommandHandler` and `CreatePollCommandHandler`. For **Public** and **Private** groups, the user must be a member to create content.
- **Voting on Polls**: Handled in `VoteOnPollCommandHandler`. Users must have posting permissions in the group to vote.
- **Group Creation**: The user creating a group is automatically assigned as the `CreatorId`.

| Method | Path | Description | Access |
|--------|------|-------------|--------|
| **POST** | `/api/v1/groups/{groupId}/polls` | Create a new poll in a group. | Group Member |
| **GET** | `/api/v1/groups/{groupId}/polls` | Get active polls for a group. | Group Viewer |

## Recommendations System

The API includes an AI-powered recommendation system that suggests posts based on semantic similarity using vector embeddings.

### How It Works

1. **Embedding Generation**: Posts are converted to vector embeddings using a TinyBERT model (TensorFlow.NET)
2. **Vector Storage**: Embeddings are stored in a SQLite vector database
3. **Similarity Search**: User queries are compared against stored vectors to find semantically similar posts
4. **Personalization**: Recommendations are based on user activity and preferences

### Get Recommended Posts

```mermaid
sequenceDiagram
    participant Client
    participant Controller as RecommendationsController
    participant Dispatcher
    participant Handler as GetRecommendedPostsHandler
    participant VectorService
    participant VectorDB[(Vector DB)]
    participant ReadRepo

    Client->>Controller: GET /api/v1/recommendations/recommended
    Controller->>Dispatcher: Query(GetRecommendedPostsQuery)
    Dispatcher->>Handler: Handle(query)
    Handler->>VectorService: GetRecommendedPostIds(userId)
    VectorService->>VectorDB: Similarity Search
    VectorDB-->>VectorService: Similar Post IDs
    VectorService-->>Handler: Post IDs
    Handler->>ReadRepo: GetPostsByIds(postIds)
    ReadRepo-->>Handler: PostReadModels
    Handler-->>Dispatcher: PagedResult<PostDto>
    Dispatcher-->>Controller: PagedResult
    Controller-->>Client: 200 OK + Recommended Posts
```

**Endpoint**: `GET /api/v1/recommendations/recommended`

**Query Parameters**:
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 10)

**Response**: `PagedResult<PostDto>` with semantically similar posts

## Statistics & Analytics

The Stats controller provides dashboard analytics with period-based tracking.

### Available Endpoints

| Method | Path | Description |
|--------|------|-------------|
| **GET** | `/api/v1/stats/history` | Get historical stats records |
| **GET** | `/api/v1/stats/weekly` | Get weekly statistics for a specific date |
| **GET** | `/api/v1/stats/monthly` | Get monthly statistics for a specific date |

### Stats Collection Flow

```mermaid
sequenceDiagram
    participant Scheduler as Background Service
    participant Handler as CollectStatsHandler
    participant Repo as StatsRepository
    participant DB[(Database)]

    Note over Scheduler: Runs periodically (daily/weekly)
    Scheduler->>Handler: Trigger Stats Collection
    Handler->>DB: Query Active Users
    Handler->>DB: Query New Posts
    Handler->>DB: Query Comments Count
    Handler->>DB: Query Reactions Count
    Handler->>DB: Query Reaction Breakdown
    Handler->>Repo: Save StatsRecord
    Repo->>DB: INSERT StatsRecord
    DB-->>Repo: Success
```

### StatsRecord Structure

Each `StatsRecord` contains:
- **StatsType**: `Weekly` or `Monthly`
- **Date**: The period start date
- **TotalPosts**: Total posts in the system
- **ActiveUsers**: Users active in the period
- **NewPosts**: Posts created in the period
- **ResultingComments**: Comments on posts in the period
- **ResultingReactions**: Reactions on posts in the period
- **ReactionBreakdown**: Array of `{emoji, count}` showing reaction distribution

## Feedback System

Users can submit feedback about the application.

**Endpoint**: `POST /api/v1/feedback`

**Request Body**:
```json
{
  "content": "User feedback message"
}
```

**Response**: `200 OK` with boolean success indicator

## File Management API

The file management functionality is handled by a separate microservice (`SocialMedia.Files.API`) with database sharding support. See [Files API Documentation](FILES_API.md) for detailed information.

