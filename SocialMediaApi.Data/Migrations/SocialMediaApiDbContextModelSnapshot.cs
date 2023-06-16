﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialMediaApi.Data;

#nullable disable

namespace SocialMediaApi.Data.Migrations
{
    [DbContext(typeof(SocialMediaApiDbContext))]
    partial class SocialMediaApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.ActivePost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ActionBasedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Downloads")
                        .HasColumnType("int");

                    b.Property<int>("EntityStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalComments")
                        .HasColumnType("int");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ActionBasedDate");

                    b.HasIndex("GroupId");

                    b.ToTable("ActivePosts");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ActionBasedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("EntityStatus")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalComments")
                        .HasColumnType("int");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.EntityDetails", b =>
                {
                    b.Property<Guid>("EntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("EntityId");

                    b.ToTable("EntityDetails");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntityStatus")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EntityStatus");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ActionBasedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Downloads")
                        .HasColumnType("int");

                    b.Property<int>("EntityStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalComments")
                        .HasColumnType("int");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ActionBasedDate");

                    b.HasIndex("GroupId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.UserDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("UserDetails");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.UserPost", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsFull")
                        .HasColumnType("bit");

                    b.Property<int>("Page")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPosts");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.ActivePost", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("ActivePostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ActivePostId");

                            b1.ToTable("ActivePosts");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("ActivePostId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("ActivePostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ActivePostId");

                            b1.ToTable("ActivePosts");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("ActivePostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaActivePostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Duration")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Url")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("MediaActivePostId", "Id");

                                    b2.ToTable("ActivePosts");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaActivePostId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("ActivePostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("ActivePostId");

                            b1.ToTable("ActivePosts");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("ActivePostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryActivePostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryActivePostId", "Id");

                                    b2.ToTable("ActivePosts");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryActivePostId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.Navigation("Creator")
                        .IsRequired();

                    b.Navigation("Media")
                        .IsRequired();

                    b.Navigation("Reactions")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Comment", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("CommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CommentId");

                            b1.ToTable("Comments");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("CommentId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("CommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CommentId");

                            b1.ToTable("Comments");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("CommentId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaCommentId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Duration")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Url")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("MediaCommentId", "Id");

                                    b2.ToTable("Comments");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaCommentId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("CommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("CommentId");

                            b1.ToTable("Comments");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("CommentId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryCommentId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryCommentId", "Id");

                                    b2.ToTable("Comments");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryCommentId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.Navigation("Creator")
                        .IsRequired();

                    b.Navigation("Media")
                        .IsRequired();

                    b.Navigation("Reactions")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.EntityDetails", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Summary", b1 =>
                        {
                            b1.Property<Guid>("EntityDetailsEntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("EntityDetailsEntityId");

                            b1.ToTable("EntityDetails");

                            b1.ToJson("Summary");

                            b1.WithOwner()
                                .HasForeignKey("EntityDetailsEntityId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryEntityDetailsEntityId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryEntityDetailsEntityId", "Id");

                                    b2.ToTable("EntityDetails");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryEntityDetailsEntityId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.OwnsMany("SocialMediaApi.Domain.JsonEntities.Reaction", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("EntityDetailsEntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<string>("Unicode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EntityDetailsEntityId", "Id");

                            b1.ToTable("EntityDetails");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("EntityDetailsEntityId");

                            b1.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b2 =>
                                {
                                    b2.Property<Guid>("ReactionEntityDetailsEntityId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("ReactionId")
                                        .HasColumnType("int");

                                    b2.Property<Guid>("Id")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<string>("ImageUrl")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionEntityDetailsEntityId", "ReactionId");

                                    b2.ToTable("EntityDetails");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionEntityDetailsEntityId", "ReactionId");
                                });

                            b1.Navigation("Creator")
                                .IsRequired();
                        });

                    b.Navigation("Reactions");

                    b.Navigation("Summary")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Group", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("GroupId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("GroupId");

                            b1.ToTable("Groups");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("GroupId");
                        });

                    b.Navigation("Creator")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Post", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("PostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PostId");

                            b1.ToTable("Posts");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("PostId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("PostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PostId");

                            b1.ToTable("Posts");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("PostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Duration")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Url")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("MediaPostId", "Id");

                                    b2.ToTable("Posts");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaPostId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("PostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("PostId");

                            b1.ToTable("Posts");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("PostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryPostId", "Id");

                                    b2.ToTable("Posts");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryPostId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.Navigation("Creator")
                        .IsRequired();

                    b.Navigation("Media")
                        .IsRequired();

                    b.Navigation("Reactions")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.UserDetails", b =>
                {
                    b.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MiniReaction", "CommentReactions", b1 =>
                        {
                            b1.Property<Guid>("UserDetailsId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<Guid>("EntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Unicode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserDetailsId", "Id");

                            b1.ToTable("UserDetails");

                            b1.ToJson("CommentReactions");

                            b1.WithOwner()
                                .HasForeignKey("UserDetailsId");
                        });

                    b.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MiniReaction", "PostReactions", b1 =>
                        {
                            b1.Property<Guid>("UserDetailsId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<Guid>("EntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Unicode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserDetailsId", "Id");

                            b1.ToTable("UserDetails");

                            b1.ToJson("PostReactions");

                            b1.WithOwner()
                                .HasForeignKey("UserDetailsId");
                        });

                    b.Navigation("CommentReactions");

                    b.Navigation("PostReactions");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.UserPost", b =>
                {
                    b.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MiniEntity", "Posts", b1 =>
                        {
                            b1.Property<string>("UserPostId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<DateTimeOffset>("CreatedDate")
                                .HasColumnType("datetimeoffset");

                            b1.Property<Guid>("EntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("UserPostId", "Id");

                            b1.ToTable("UserPosts");

                            b1.ToJson("Posts");

                            b1.WithOwner()
                                .HasForeignKey("UserPostId");
                        });

                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
