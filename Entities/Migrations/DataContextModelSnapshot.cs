﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Entities.Models.ChatRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CoverImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ChatRoom", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdTarget")
                        .HasColumnType("int");

                    b.Property<DateTime>("SavedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TargetTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Entities.Models.HangfireJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ScheduledId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("ScheduledJob");
                });

            modelBuilder.Entity("Entities.Models.HistoryTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime")
                        .HasColumnName("deadline");

                    b.Property<int?>("IdTransaction")
                        .HasColumnType("int")
                        .HasColumnName("idTransaction");

                    b.Property<int?>("IdUserFrom")
                        .HasColumnType("int")
                        .HasColumnName("idUserFrom");

                    b.Property<int?>("IdUserTo")
                        .HasColumnType("int")
                        .HasColumnName("idUserTo");

                    b.Property<decimal?>("MoneyTrans")
                        .HasColumnType("money")
                        .HasColumnName("moneyTrans");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("IdTransaction");

                    b.ToTable("HistoryTransaction", (string)null);
                });

            modelBuilder.Entity("Entities.Models.HistoryWallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Amount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("amount");

                    b.Property<int?>("IdUser")
                        .HasColumnType("int")
                        .HasColumnName("idUser");

                    b.Property<int?>("IdWallet")
                        .HasColumnType("int")
                        .HasColumnName("idWallet");

                    b.Property<int?>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime")
                        .HasColumnName("time");

                    b.Property<int?>("Type")
                        .HasColumnType("int")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("IdWallet");

                    b.ToTable("HistoryWallet", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Messages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoomId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<DateTime>("NotiDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Entities.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AddressSlot")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("addressSlot");

                    b.Property<string>("CategorySlot")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("categorySlot");

                    b.Property<string>("ContentPost")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("contentPost");

                    b.Property<int?>("IdType")
                        .HasColumnType("int")
                        .HasColumnName("idType");

                    b.Property<int?>("IdUserTo")
                        .HasColumnType("int")
                        .HasColumnName("idUserTo");

                    b.Property<string>("ImageUrls")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imgUrl");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LevelSlot")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("levelSlot");

                    b.Property<DateTime>("SavedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SlotsInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit")
                        .HasColumnName("status");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TotalViewer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdType");

                    b.HasIndex("IdUserTo");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Entities.Models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("IdPost")
                        .HasColumnType("int");

                    b.Property<int?>("IdRoom")
                        .HasColumnType("int")
                        .HasColumnName("idRoom");

                    b.Property<int?>("IdTransaction")
                        .HasColumnType("int");

                    b.Property<int?>("IdUserFrom")
                        .HasColumnType("int")
                        .HasColumnName("idUserFrom");

                    b.Property<int?>("IdUserTo")
                        .HasColumnType("int")
                        .HasColumnName("idUserTo");

                    b.Property<string>("ReportTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<DateTime?>("TimeReport")
                        .HasColumnType("datetime")
                        .HasColumnName("timeReport");

                    b.Property<string>("reportContent")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdTransaction");

                    b.ToTable("Report", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("roleName");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Entities.Models.Slot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ContentSlot")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("contentSlot");

                    b.Property<int?>("IdPost")
                        .HasColumnType("int")
                        .HasColumnName("idPost");

                    b.Property<int?>("IdUser")
                        .HasColumnType("int")
                        .HasColumnName("idUser");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Price")
                        .HasColumnType("money")
                        .HasColumnName("price");

                    b.Property<int?>("SlotNumber")
                        .HasColumnType("int")
                        .HasColumnName("slotNumber");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit")
                        .HasColumnName("status");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("int");

                    b.Property<int?>("TransactionId1")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdUser");

                    b.HasIndex("TransactionId");

                    b.HasIndex("TransactionId1")
                        .IsUnique()
                        .HasFilter("[TransactionId1] IS NOT NULL");

                    b.ToTable("Slot", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("IsBanded")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSubcription")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.Property<int>("UserSubId")
                        .HasColumnType("int")
                        .HasColumnName("userSubId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserSubId");

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("DeadLine")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IdUser")
                        .HasColumnType("int")
                        .HasColumnName("idUser");

                    b.Property<string>("MethodTrans")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("methodTrans");

                    b.Property<decimal?>("MoneyTrans")
                        .HasColumnType("money")
                        .HasColumnName("moneyTrans");

                    b.Property<int?>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<DateTime?>("TimeTrans")
                        .HasColumnType("datetime")
                        .HasColumnName("timeTrans");

                    b.Property<string>("TypeTrans")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("typeTrans");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Entities.Models.TypePost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("TypePost1")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("typePost");

                    b.HasKey("Id");

                    b.ToTable("TypePost", (string)null);
                });

            modelBuilder.Entity("Entities.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeviceToken")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("deviceToken");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("fullName");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imgUrl");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<bool>("IsAndroidDevice")
                        .HasColumnType("bit");

                    b.Property<bool>("IsBanFromLogin")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogingingDevice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .HasColumnName("phoneNumber")
                        .IsFixedLength();

                    b.Property<string>("PlayingArea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayingLevel")
                        .HasColumnType("int");

                    b.Property<string>("PlayingWay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Rate")
                        .HasColumnType("float")
                        .HasColumnName("rate");

                    b.Property<string>("SortProfile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TotalRate")
                        .HasColumnType("int")
                        .HasColumnName("totalRate");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("userName");

                    b.Property<string>("UserPassword")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("userPassword");

                    b.Property<int?>("UserRole")
                        .HasColumnType("int")
                        .HasColumnName("userRole");

                    b.HasKey("Id");

                    b.HasIndex("UserRole");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Entities.Models.UserChatRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("RoomId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserChatRoom", (string)null);
                });

            modelBuilder.Entity("Entities.Models.UserRating", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<double?>("Friendly")
                        .HasColumnType("float")
                        .HasColumnName("friendly");

                    b.Property<double?>("Helpful")
                        .HasColumnType("float")
                        .HasColumnName("helpful");

                    b.Property<int?>("IdTransaction")
                        .HasColumnType("int")
                        .HasColumnName("idTransaction");

                    b.Property<int?>("IdUserRate")
                        .HasColumnType("int")
                        .HasColumnName("idUserRate");

                    b.Property<int?>("IdUserRated")
                        .HasColumnType("int")
                        .HasColumnName("idUserRated");

                    b.Property<double?>("LevelSkill")
                        .HasColumnType("float")
                        .HasColumnName("levelSkill");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime")
                        .HasColumnName("time");

                    b.Property<double?>("Trusted")
                        .HasColumnType("float")
                        .HasColumnName("trusted");

                    b.HasKey("Id");

                    b.HasIndex("IdTransaction");

                    b.HasIndex("IdUserRated");

                    b.ToTable("UserRating", (string)null);
                });

            modelBuilder.Entity("Entities.Models.VerifyToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("VerifyToken");
                });

            modelBuilder.Entity("Entities.Models.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal?>("Balance")
                        .HasColumnType("money")
                        .HasColumnName("balance");

                    b.Property<int?>("IdUser")
                        .HasColumnType("int")
                        .HasColumnName("idUser");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Wallet", (string)null);
                });

            modelBuilder.Entity("Entities.Models.Wishlist", b =>
                {
                    b.Property<int?>("IdPost")
                        .HasColumnType("int")
                        .HasColumnName("idPost");

                    b.Property<int?>("IdUser")
                        .HasColumnType("int")
                        .HasColumnName("idUser");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdUser");

                    b.ToTable("Wishlist", (string)null);
                });

            modelBuilder.Entity("Entities.Models.HangfireJob", b =>
                {
                    b.HasOne("Entities.Models.Transaction", "Transaction")
                        .WithMany("ScheduledJob")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_HangfireJob_Transaction");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Entities.Models.HistoryTransaction", b =>
                {
                    b.HasOne("Entities.Models.Transaction", "IdTransactionNavigation")
                        .WithMany("HistoryTransactions")
                        .HasForeignKey("IdTransaction")
                        .HasConstraintName("FK_HistoryTransaction_Transactions");

                    b.Navigation("IdTransactionNavigation");
                });

            modelBuilder.Entity("Entities.Models.HistoryWallet", b =>
                {
                    b.HasOne("Entities.Models.Wallet", "IdWalletNavigation")
                        .WithMany("HistoryWallets")
                        .HasForeignKey("IdWallet")
                        .HasConstraintName("FK_HistoryWallet_Wallet");

                    b.Navigation("IdWalletNavigation");
                });

            modelBuilder.Entity("Entities.Models.Messages", b =>
                {
                    b.HasOne("Entities.Models.ChatRoom", "ChatRoom")
                        .WithMany("Messages")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_Message_Room");

                    b.HasOne("Entities.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Message_User");

                    b.Navigation("ChatRoom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Models.Notification", b =>
                {
                    b.HasOne("Entities.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_User_Notifications");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Models.Post", b =>
                {
                    b.HasOne("Entities.Models.TypePost", "IdTypeNavigation")
                        .WithMany("Posts")
                        .HasForeignKey("IdType")
                        .HasConstraintName("FK_Posts_TypePost");

                    b.HasOne("Entities.Models.User", "IdUserToNavigation")
                        .WithMany("Posts")
                        .HasForeignKey("IdUserTo")
                        .HasConstraintName("FK_Posts_Users");

                    b.Navigation("IdTypeNavigation");

                    b.Navigation("IdUserToNavigation");
                });

            modelBuilder.Entity("Entities.Models.Report", b =>
                {
                    b.HasOne("Entities.Models.Post", "Post")
                        .WithMany("Reports")
                        .HasForeignKey("IdPost")
                        .HasConstraintName("FK_Post_Reports");

                    b.HasOne("Entities.Models.Transaction", "Transaction")
                        .WithMany("Reports")
                        .HasForeignKey("IdTransaction")
                        .HasConstraintName("FK_Transaction_Reports");

                    b.Navigation("Post");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Entities.Models.Slot", b =>
                {
                    b.HasOne("Entities.Models.Post", "IdPostNavigation")
                        .WithMany("Slots")
                        .HasForeignKey("IdPost")
                        .HasConstraintName("FK_Slot_Posts");

                    b.HasOne("Entities.Models.User", "User")
                        .WithMany("Slots")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Slot_User");

                    b.HasOne("Entities.Models.Transaction", "Transaction")
                        .WithMany("Slots")
                        .HasForeignKey("TransactionId")
                        .HasConstraintName("FK_Slot_Transaction");

                    b.HasOne("Entities.Models.Transaction", null)
                        .WithOne("IdSlotNavigation")
                        .HasForeignKey("Entities.Models.Slot", "TransactionId1");

                    b.Navigation("IdPostNavigation");

                    b.Navigation("Transaction");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Models.Subscription", b =>
                {
                    b.HasOne("Entities.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Subcribe_Users");

                    b.HasOne("Entities.Models.User", "UserSub")
                        .WithMany()
                        .HasForeignKey("UserSubId")
                        .IsRequired()
                        .HasConstraintName("FK_Subcribe_Users1");

                    b.Navigation("User");

                    b.Navigation("UserSub");
                });

            modelBuilder.Entity("Entities.Models.Transaction", b =>
                {
                    b.HasOne("Entities.Models.User", "IdUserNavigation")
                        .WithMany("Transactions")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Transactions_Users");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("Entities.Models.User", b =>
                {
                    b.HasOne("Entities.Models.Role", "UserRoleNavigation")
                        .WithMany("Users")
                        .HasForeignKey("UserRole")
                        .HasConstraintName("FK_Users_Roles");

                    b.Navigation("UserRoleNavigation");
                });

            modelBuilder.Entity("Entities.Models.UserChatRoom", b =>
                {
                    b.HasOne("Entities.Models.ChatRoom", "ChatRoom")
                        .WithMany("Users")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_ChatRoomUser_Room");

                    b.HasOne("Entities.Models.User", "User")
                        .WithMany("ChatRooms")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ChatRoomUser_User");

                    b.Navigation("ChatRoom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Models.UserRating", b =>
                {
                    b.HasOne("Entities.Models.HistoryTransaction", "IdTransactionNavigation")
                        .WithMany("UserRatings")
                        .HasForeignKey("IdTransaction")
                        .HasConstraintName("FK_UserRating_HistoryTransaction");

                    b.HasOne("Entities.Models.User", "IdUserRatedNavigation")
                        .WithMany("UserRatings")
                        .HasForeignKey("IdUserRated")
                        .HasConstraintName("FK_UserRating_Users");

                    b.Navigation("IdTransactionNavigation");

                    b.Navigation("IdUserRatedNavigation");
                });

            modelBuilder.Entity("Entities.Models.VerifyToken", b =>
                {
                    b.HasOne("Entities.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Tokens_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Models.Wallet", b =>
                {
                    b.HasOne("Entities.Models.User", "IdUserNavigation")
                        .WithMany("Wallets")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Wallet_Users");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("Entities.Models.Wishlist", b =>
                {
                    b.HasOne("Entities.Models.Post", "IdPostNavigation")
                        .WithMany()
                        .HasForeignKey("IdPost")
                        .HasConstraintName("FK_Wishlist_Posts");

                    b.HasOne("Entities.Models.User", "IdUserNavigation")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Wishlist_Users");

                    b.Navigation("IdPostNavigation");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("Entities.Models.ChatRoom", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Entities.Models.HistoryTransaction", b =>
                {
                    b.Navigation("UserRatings");
                });

            modelBuilder.Entity("Entities.Models.Post", b =>
                {
                    b.Navigation("Reports");

                    b.Navigation("Slots");
                });

            modelBuilder.Entity("Entities.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Entities.Models.Transaction", b =>
                {
                    b.Navigation("HistoryTransactions");

                    b.Navigation("IdSlotNavigation");

                    b.Navigation("Reports");

                    b.Navigation("ScheduledJob");

                    b.Navigation("Slots");
                });

            modelBuilder.Entity("Entities.Models.TypePost", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Entities.Models.User", b =>
                {
                    b.Navigation("ChatRooms");

                    b.Navigation("Messages");

                    b.Navigation("Notifications");

                    b.Navigation("Posts");

                    b.Navigation("Slots");

                    b.Navigation("Tokens");

                    b.Navigation("Transactions");

                    b.Navigation("UserRatings");

                    b.Navigation("Wallets");
                });

            modelBuilder.Entity("Entities.Models.Wallet", b =>
                {
                    b.Navigation("HistoryWallets");
                });
#pragma warning restore 612, 618
        }
    }
}
