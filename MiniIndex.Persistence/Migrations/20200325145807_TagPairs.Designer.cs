﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniIndex.Persistence;

namespace MiniIndex.Migrations
{
    [DbContext(typeof(MiniIndexContext))]
    [Migration("20200325145807_TagPairs")]
    partial class TagPairs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MiniIndex.Models.Creator", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Creator");
                });

            modelBuilder.Entity("MiniIndex.Models.Mini", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ApprovedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Cost")
                        .HasColumnType("int");

                    b.Property<int?>("CreatorID")
                        .HasColumnType("int");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("CreatorID");

                    b.HasIndex("UserId");

                    b.ToTable("Mini");
                });

            modelBuilder.Entity("MiniIndex.Models.MiniSourceSite", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MiniID")
                        .HasColumnType("int");

                    b.Property<int?>("SiteID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("MiniID");

                    b.HasIndex("SiteID");

                    b.ToTable("MiniSourceSite");
                });

            modelBuilder.Entity("MiniIndex.Models.MiniTag", b =>
                {
                    b.Property<int>("MiniID")
                        .HasColumnType("int");

                    b.Property<int>("TagID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ApprovedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TaggerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MiniID", "TagID");

                    b.HasIndex("TagID");

                    b.HasIndex("TaggerId");

                    b.ToTable("MiniTag");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSite", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatorID")
                        .HasColumnType("int");

                    b.Property<string>("CreatorUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CreatorID");

                    b.ToTable("SourceSite");

                    b.HasDiscriminator<string>("SiteName").HasValue("SourceSite");
                });

            modelBuilder.Entity("MiniIndex.Models.Starred", b =>
                {
                    b.Property<int>("MiniID")
                        .HasColumnType("int");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MiniID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("Starred");
                });

            modelBuilder.Entity("MiniIndex.Models.Tag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Category")
                        .HasColumnType("int");

                    b.Property<string>("TagName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("MiniIndex.Models.TagPair", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Tag1ID")
                        .HasColumnType("int");

                    b.Property<int?>("Tag2ID")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("Tag1ID");

                    b.HasIndex("Tag2ID");

                    b.ToTable("TagPair");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.Cults3dSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Cults3d");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.DuncanShadowSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("DuncanShadow");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.GumroadSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Gumroad");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.MyMiniFactorySource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("MyMiniFactory");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.PatreonSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Patreon");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.ShapewaysSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Shapeways");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.ThingiverseSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Thingiverse");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.Wargaming3dSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.HasDiscriminator().HasValue("Wargaming3d");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSites.WebsiteSource", b =>
                {
                    b.HasBaseType("MiniIndex.Models.SourceSite");

                    b.Property<string>("WebsiteProfilePageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebsiteURL")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Website");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiniIndex.Models.Mini", b =>
                {
                    b.HasOne("MiniIndex.Models.Creator", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorID");

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MiniIndex.Models.MiniSourceSite", b =>
                {
                    b.HasOne("MiniIndex.Models.Mini", "Mini")
                        .WithMany("Sources")
                        .HasForeignKey("MiniID");

                    b.HasOne("MiniIndex.Models.SourceSite", "Site")
                        .WithMany()
                        .HasForeignKey("SiteID");
                });

            modelBuilder.Entity("MiniIndex.Models.MiniTag", b =>
                {
                    b.HasOne("MiniIndex.Models.Mini", "Mini")
                        .WithMany("MiniTags")
                        .HasForeignKey("MiniID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiniIndex.Models.Tag", "Tag")
                        .WithMany("MiniTags")
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "Tagger")
                        .WithMany()
                        .HasForeignKey("TaggerId");
                });

            modelBuilder.Entity("MiniIndex.Models.SourceSite", b =>
                {
                    b.HasOne("MiniIndex.Models.Creator", "Creator")
                        .WithMany("Sites")
                        .HasForeignKey("CreatorID");
                });

            modelBuilder.Entity("MiniIndex.Models.Starred", b =>
                {
                    b.HasOne("MiniIndex.Models.Mini", "Mini")
                        .WithMany()
                        .HasForeignKey("MiniID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiniIndex.Models.TagPair", b =>
                {
                    b.HasOne("MiniIndex.Models.Tag", "Tag1")
                        .WithMany()
                        .HasForeignKey("Tag1ID");

                    b.HasOne("MiniIndex.Models.Tag", "Tag2")
                        .WithMany()
                        .HasForeignKey("Tag2ID");
                });
#pragma warning restore 612, 618
        }
    }
}
