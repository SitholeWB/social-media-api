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
    }

    class MediaFile {
        +Guid Id
        +string FileName
        +string ContentType
        +long Size
    }

    User "1" -- "*" Post : Creates
    User "1" -- "*" Comment : Writes
    User "1" -- "*" Like : Likes
    User "1" -- "*" Vote : Votes
    
    Post "1" -- "*" Comment : Has
    Post "1" -- "*" Like : Has
    Post "1" -- "0..1" MediaFile : Contains
    
    Comment "1" -- "*" Like : Has
    Comment "1" -- "0..1" MediaFile : Contains

    Poll "1" -- "*" PollOption : Has
    PollOption "1" -- "*" Vote : Receives
```

## Entities Description

- **User**: Represents a registered user of the system.
- **Post**: A content item created by a user, which can contain text and media.
- **Comment**: A response to a post, which can also contain text and media.
- **Like**: Represents a user's positive reaction to a post or comment.
- **Poll**: A question with multiple options for users to vote on.
- **Group**: A collection of users and posts (if applicable).
- **MediaFile**: Represents an uploaded file (image, video, etc.).
