using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TravelMate.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutPage> AboutPages { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<ContactPage> ContactPages { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<HomePage> HomePages { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Testmonial> Testmonials { get; set; }

    public virtual DbSet<Userlogin> Userlogins { get; set; }

    public virtual DbSet<Userr> Userrs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("USER ID=C##Hotels;PASSWORD=Test321;DATA SOURCE=localhost:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##HOTELS")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<AboutPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008587");

            entity.ToTable("ABOUT_PAGE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Imagemain)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGEMAIN");
            entity.Property(e => e.PAbout)
                .IsUnicode(false)
                .HasColumnName("P_ABOUT");
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008577");

            entity.ToTable("BANK");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("BALANCE");
            entity.Property(e => e.Creditexp)
                .HasColumnType("DATE")
                .HasColumnName("CREDITEXP");
            entity.Property(e => e.Creditnumber)
                .HasPrecision(16)
                .HasColumnName("CREDITNUMBER");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Banks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008578");
        });

        modelBuilder.Entity<ContactPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008589");

            entity.ToTable("CONTACT_PAGE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LOCATION");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008580");

            entity.ToTable("EVENTS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Eventdescription)
                .IsUnicode(false)
                .HasColumnName("EVENTDESCRIPTION");
            entity.Property(e => e.Eventname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EVENTNAME");
            entity.Property(e => e.HotelId)
                .HasColumnType("NUMBER")
                .HasColumnName("HOTEL_ID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Events)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008581");
        });

        modelBuilder.Entity<HomePage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008585");

            entity.ToTable("HOME_PAGE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Imagelogo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGELOGO");
            entity.Property(e => e.Imagemain)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGEMAIN");
            entity.Property(e => e.PCopyrigth)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("P_COPYRIGTH");
            entity.Property(e => e.PFooter)
                .IsUnicode(false)
                .HasColumnName("P_FOOTER");
            entity.Property(e => e.PWelcome)
                .IsUnicode(false)
                .HasColumnName("P_WELCOME");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("SYS_C008568");

            entity.ToTable("HOTELS");

            entity.Property(e => e.HotelId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("HOTEL_ID");
            entity.Property(e => e.Hotelname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HOTELNAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008573");

            entity.ToTable("RESERVATIONS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Checkindate)
                .HasColumnType("DATE")
                .HasColumnName("CHECKINDATE");
            entity.Property(e => e.Checkoutdate)
                .HasColumnType("DATE")
                .HasColumnName("CHECKOUTDATE");
            entity.Property(e => e.RoomId)
                .HasColumnType("NUMBER")
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008575");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008574");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("SYS_C008560");

            entity.ToTable("ROLE");

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ROLE_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("SYS_C008570");

            entity.ToTable("ROOMS");

            entity.Property(e => e.RoomId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.HotelId)
                .HasColumnType("NUMBER")
                .HasColumnName("HOTEL_ID");
            entity.Property(e => e.Isavailable)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ISAVAILABLE");
            entity.Property(e => e.Pricepernight)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRICEPERNIGHT");
            entity.Property(e => e.Roomnumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ROOMNUMBER");
            entity.Property(e => e.Roomtype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROOMTYPE");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008571");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008583");

            entity.ToTable("SERVICES");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Servicedescription)
                .IsUnicode(false)
                .HasColumnName("SERVICEDESCRIPTION");
            entity.Property(e => e.Servicename)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SERVICENAME");
        });

        modelBuilder.Entity<Testmonial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008591");

            entity.ToTable("TESTMONIAL");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Msg)
                .IsUnicode(false)
                .HasColumnName("MSG");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Testmonials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008592");
        });

        modelBuilder.Entity<Userlogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008564");

            entity.ToTable("USERLOGIN");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Passwordd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORDD");
            entity.Property(e => e.RoleId)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLE_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER")
                .HasColumnName("USER_ID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Role).WithMany(p => p.Userlogins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008566");

            entity.HasOne(d => d.User).WithMany(p => p.Userlogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C008565");
        });

        modelBuilder.Entity<Userr>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("SYS_C008562");

            entity.ToTable("USERR");

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("USER_ID");
            entity.Property(e => e.Emale)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMALE");
            entity.Property(e => e.Fname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Lname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LNAME");
            entity.Property(e => e.Phone)
                .HasColumnType("NUMBER")
                .HasColumnName("PHONE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
