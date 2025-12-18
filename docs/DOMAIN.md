# Domain Model

The Domain layer contains the core business entities and logic. It has no dependencies on other layers.

## Class Diagram

```mermaid
classDiagram
    class User {
        +Guid Id
        +string Username
        +string Email
        +string PasswordHash
        +UserRole Role
        +bool IsBanned
    }

    class Post {
        +Guid Id
        +string Title
        +string Content
        +Guid AuthorId
        +Guid? FileId
    }

    class Comment {
        +Guid Id
        +string Content
        +Guid PostId
        +Guid AuthorId
        +Guid? FileId
    }

    class Like {
        +Guid Id
        +Guid UserId
        +Guid? PostId
        +Guid? CommentId
    }

    class Poll {
        +Guid Id
        +string Question
        +DateTime EndsAt
    }

    class PollOption {
        +Guid Id
        +string Text
        +Guid PollId
    }

    class Vote {
        +Guid Id
        +Guid UserId
        +Guid PollOptionId
    }

    class Group {
        +Guid Id
        +string Name
        +string Description
        +GroupType Type
        +Guid CreatorId
    }

    class GroupMember {
        +Guid Id
        +Guid GroupId
        +Guid UserId
    }

    class GroupType {
        <<enumeration>>
        Public
        Private
        Everyone
    }

    class MediaFile {
        +Guid Id
        +string FileName
        +string ContentType
        +long Size
    }

    class Notification {
        +Guid Id
        +Guid UserId
        +string Message
        +bool IsRead
        +DateTime CreatedAt
    }

    class Report {
        +Guid Id
        +Guid ReporterId
        +string Reason
        +ReportStatus Status
        +Guid? PostId
        +Guid? CommentId
    }

    class UserBlock {
        +Guid Id
        +Guid BlockerId
        +Guid BlockedId
    }

    User "1" -- "*" Post : Creates
    User "1" -- "*" Comment : Writes
    User "1" -- "*" Like : Likes
    User "1" -- "*" Vote : Votes
    
    Post "1" -- "*" Comment : Has
    Post "1" -- "*" Like : Has
    Post "1" -- "0..1" MediaFile : Contains
    Post "1" -- "*" Report : Reported By
    
    Comment "1" -- "*" Like : Has
    Comment "1" -- "0..1" MediaFile : Contains
    Comment "1" -- "*" Report : Reported By

    Group "1" -- "*" GroupMember : Has
    User "1" -- "*" GroupMember : Member Of
    User "1" -- "*" Notification : Receives
    User "1" -- "*" UserBlock : Blocks/Blocked By


    class OutboxEvent {
        +Guid Id
        +string EventType
        +string EventData
        +OutboxEventStatus Status
        +int RetryCount
        +DateTime CreatedAt
        +DateTime? ProcessedAt
    }

    class OutboxEventStatus {
        <<enumeration>>
        Pending
        Processing
        Completed
        Failed
    }

    class ReportStatus {
        <<enumeration>>
        Pending
        Reviewed
        Dismissed
    }

    Poll "1" -- "*" PollOption : Has
    PollOption "1" -- "*" Vote : Receives
```

## Entities Description

- **User**: Represents a registered user of the system.
- **Post**: A content item created by a user, which can contain text and media.
- **Comment**: A response to a post, which can also contain text and media.
- **Like**: Represents a user's positive reaction to a post or comment.
- **Poll**: A question with multiple options for users to vote on.
- **Group**: A collection of users and posts. Supports three types: **Public** (members only post, anyone views), **Private** (members only view/post), and **Everyone** (anyone views/posts).
- **MediaFile**: Represents an uploaded file (image, video, etc.).
- **Notification**: A system message sent to a user.
- **Report**: A user's report of inappropriate content (Post or Comment).
- **UserBlock**: Represents a block relationship between two users.
- **OutboxEvent**: Stores domain events for reliable asynchronous processing using the Outbox pattern.
- **GroupMember**: Represents a user's membership in a group.


## Read Models

The domain also includes specialized **Read Models** optimized for query operations. These models are denormalized and include aggregated data to improve read performance.

### PostReadModel

Optimized view of a Post with embedded statistics and related data:

- **Stats** (JSON): `LikeCount`, `CommentCount`, `TrendingScore`
- **Reactions** (JSON): Collection of user reactions with emoji support
- **TopComments** (JSON): Embedded top 30 comments with their reactions
- **Author Info**: Denormalized `AuthorName` for quick access
- **Group Info**: Optional `GroupId` and `GroupName`

### CommentReadModel

Optimized view of a Comment with embedded statistics:

- **Stats** (JSON): `LikeCount`
- **Reactions** (JSON): Collection of user reactions with emoji support
- **Author Info**: Denormalized `AuthorName` and `AuthorProfilePicUrl`

### JSON Mapping

Read Models use EF Core's `ToJson()` feature to store complex properties as JSON columns, reducing the need for joins and improving query performance. This is particularly beneficial for:

- Frequently accessed aggregated data (stats)
- Collections that are always loaded together (reactions)
- Embedded related entities (top comments)

