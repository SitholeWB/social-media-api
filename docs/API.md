# API Documentation

The SocialMedia API is a RESTful interface built with ASP.NET Core. It uses standard HTTP methods and status codes.

## Controllers Overview

| Controller | Description | Base Path |
|------------|-------------|-----------|
| **Auth** | User registration and login. | `/api/v1/Auth` |
| **Posts** | Manage posts (create, read, report). | `/api/v1/Posts` |
| **Comments** | Manage comments on posts. | `/api/v1/Comments` |
| **Likes** | Like/unlike posts and comments. | `/api/v1/Likes` |
| **Polls** | Create and vote on polls. | `/api/v1/Polls` |
| **Users** | User profile management. | `/api/v1/Users` |
| **Groups** | Group management. | `/api/v1/Groups` |
| **Notifications** | User notifications. | `/api/v1/Notifications` |
| **Moderation** | Content moderation tools. | `/api/v1/Moderation` |
| **Reports** | Handling user reports. | `/api/v1/Reports` |

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

```mermaid
sequenceDiagram
    participant Client
    participant API as PostsController
    participant Dispatcher
    participant Handler as CreatePostCommandHandler
    participant Repo as PostRepository
    participant DB as Database

    Client->>API: POST /api/v1/Posts
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
