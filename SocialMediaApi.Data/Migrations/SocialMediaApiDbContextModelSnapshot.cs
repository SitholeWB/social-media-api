﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialMediaApi.Data;

#nullable disable

namespace SocialMediaApi.Repositories.Migrations
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

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.GroupPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Downloads")
                        .HasColumnType("int");

                    b.Property<int>("EntityStatus")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("ExpireDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalComments")
                        .HasColumnType("int");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupPosts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("GroupPost");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.ActiveGroupPost", b =>
                {
                    b.HasBaseType("SocialMediaApi.Domain.Entities.GroupPost");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("ActiveGroupPost");
                });

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.NewGroupPost", b =>
                {
                    b.HasBaseType("SocialMediaApi.Domain.Entities.GroupPost");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.ToTable("GroupPosts", t =>
                        {
                            t.Property("Rank")
                                .HasColumnName("NewGroupPost_Rank");
                        });

                    b.HasDiscriminator().HasValue("NewGroupPost");
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

                            b1.OwnsMany("SocialMediaApi.Domain.Entities.JsonEntities.Content", "Content", b2 =>
                                {
                                    b2.Property<Guid>("MediaGroupPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<string>("Description")
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

                    b.OwnsOne("SocialMediaApi.Domain.Entities.JsonEntities.Reaction", "Reactions", b1 =>
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
                                    b2.Property<Guid>("ReactionGroupPostId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    b2.Property<int>("Count")
                                        .HasColumnType("int");

                                    b2.Property<string>("Unicode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ReactionGroupPostId", "Id");

                                    b2.ToTable("GroupPosts");

                                    b2.WithOwner()
                                        .HasForeignKey("ReactionGroupPostId");
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

            modelBuilder.Entity("SocialMediaApi.Domain.Entities.Group", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
