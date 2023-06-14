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

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.ActiveGroupPost", b =>
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

                    b.ToTable("ActiveGroupPosts");
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

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPost", b =>
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

                    b.ToTable("GroupPosts");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPostComment", b =>
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

                    b.Property<Guid>("GroupPostId")
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

                    b.HasIndex("GroupPostId");

                    b.ToTable("GroupPostComments");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Reaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Unicode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.ActiveGroupPost", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("ActiveGroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Role")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ActiveGroupPostId");

                            b1.ToTable("ActiveGroupPosts");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("ActiveGroupPostId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("ActiveGroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ActiveGroupPostId");

                            b1.ToTable("ActiveGroupPosts");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("ActiveGroupPostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaActiveGroupPostId")
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

                                    b2.HasKey("MediaActiveGroupPostId", "Id");

                                    b2.ToTable("ActiveGroupPosts");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaActiveGroupPostId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("ActiveGroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("ActiveGroupPostId");

                            b1.ToTable("ActiveGroupPosts");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("ActiveGroupPostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryActiveGroupPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryActiveGroupPostId", "Id");

                                    b2.ToTable("ActiveGroupPosts");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryActiveGroupPostId");
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

                            b1.Property<string>("Role")
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

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPost", b =>
                {
                    b.HasOne("SocialMediaApi.Domain.Entities.Group", "Group")
                        .WithMany("Posts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("GroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Role")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("GroupPostId");

                            b1.ToTable("GroupPosts");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("GroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("GroupPostId");

                            b1.ToTable("GroupPosts");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaGroupPostId")
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

                                    b2.HasKey("MediaGroupPostId", "Id");

                                    b2.ToTable("GroupPosts");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaGroupPostId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("GroupPostId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("GroupPostId");

                            b1.ToTable("GroupPosts");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryGroupPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryGroupPostId", "Id");

                                    b2.ToTable("GroupPosts");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryGroupPostId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.Navigation("Creator")
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Media")
                        .IsRequired();

                    b.Navigation("Reactions")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPostComment", b =>
                {
                    b.HasOne("SocialMediaApi.Domain.Entities.GroupPost", "GroupPost")
                        .WithMany("GroupPostComments")
                        .HasForeignKey("GroupPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("GroupPostCommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Role")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("GroupPostCommentId");

                            b1.ToTable("GroupPostComments");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostCommentId");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Media", "Media", b1 =>
                        {
                            b1.Property<Guid>("GroupPostCommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("MediaType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("GroupPostCommentId");

                            b1.ToTable("GroupPostComments");

                            b1.ToJson("Media");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostCommentId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.MediaContent", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaGroupPostCommentId")
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

                                    b2.HasKey("MediaGroupPostCommentId", "Id");

                                    b2.ToTable("GroupPostComments");

                                    b2.WithOwner()
                                        .HasForeignKey("MediaGroupPostCommentId");
                                });

                            b1.Navigation("Content");
                        });

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.ReactionSummary", "Reactions", b1 =>
                        {
                            b1.Property<Guid>("GroupPostCommentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ReactionsCount")
                                .HasColumnType("int");

                            b1.HasKey("GroupPostCommentId");

                            b1.ToTable("GroupPostComments");

                            b1.ToJson("Reactions");

                            b1.WithOwner()
                                .HasForeignKey("GroupPostCommentId");

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Emoji", "Emojis", b2 =>
                                {
                                    b2.Property<Guid>("ReactionSummaryGroupPostCommentId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionSummaryGroupPostCommentId", "Id");

                                    b2.ToTable("GroupPostComments");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionSummaryGroupPostCommentId");
                                });

                            b1.Navigation("Emojis");
                        });

                    b.Navigation("Creator")
                        .IsRequired();

                    b.Navigation("GroupPost");

                    b.Navigation("Media")
                        .IsRequired();

                    b.Navigation("Reactions")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Reaction", b =>
                {
                    b.OwnsOne("SocialMediaApi.Domain.Entities.Base.BaseUser", "Creator", b1 =>
                        {
                            b1.Property<Guid>("ReactionId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Role")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ReactionId");

                            b1.ToTable("Reactions");

                            b1.ToJson("Creator");

                            b1.WithOwner()
                                .HasForeignKey("ReactionId");
                        });

                    b.Navigation("Creator")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Group", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPost", b =>
                {
                    b.Navigation("GroupPostComments");
                });
#pragma warning restore 612, 618
        }
    }
}
