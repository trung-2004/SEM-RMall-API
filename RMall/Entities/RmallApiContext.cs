using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RMall.Entities;

public partial class RmallApiContext : DbContext
{
    public RmallApiContext()
    {
    }

    public RmallApiContext(DbContextOptions<RmallApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<BrandIntroduction> BrandIntroductions { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Gallery> Galleries { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieGenre> MovieGenres { get; set; }

    public virtual DbSet<MovieLanguage> MovieLanguages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderFood> OrderFoods { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatPricing> SeatPricings { get; set; }

    public virtual DbSet<SeatType> SeatTypes { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPromotion> UserPromotions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=RMall-API;Integrated Security=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3213E83F95E6F924");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Shop).WithMany(p => p.Brands)
                .HasForeignKey(d => d.ShopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Brands__shop_id__0A688BB1");
        });

        modelBuilder.Entity<BrandIntroduction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BrandInt__3213E83FE250BD4B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Brand).WithMany(p => p.BrandIntroductions)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BrandIntr__brand__12FDD1B2");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3213E83F1C6DB4F9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Message)
                .HasColumnType("ntext")
                .HasColumnName("message");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Floors__3213E83F1E06F4AA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FloorNumber).HasColumnName("floor_number");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Foods__3213E83F43328AB7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Gallery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gallery__3213E83FF5739C66");

            entity.ToTable("Gallery");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Movie).WithMany(p => p.Galleries)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Gallery__movie_i__65370702");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3213E83FD8A5F15A");

            entity.ToTable("Genre");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Language__3213E83F3E94CF5D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movies__3213E83F6FC4D989");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Actor)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("actor");
            entity.Property(e => e.Cast)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cast");
            entity.Property(e => e.CoverImage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cover_image");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Describe)
                .HasColumnType("text")
                .HasColumnName("describe");
            entity.Property(e => e.Director)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("director");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.MovieImage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("movie_image");
            entity.Property(e => e.Ratings)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ratings");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("release_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.Trailer)
                .HasColumnType("text")
                .HasColumnName("trailer");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MovieGen__3213E83FC0FEA233");

            entity.ToTable("MovieGenre");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");

            entity.HasOne(d => d.Genre).WithMany(p => p.MovieGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieGenr__genre__5CA1C101");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieGenres)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieGenr__movie__5BAD9CC8");
        });

        modelBuilder.Entity<MovieLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MovieLan__3213E83F6A0C88DC");

            entity.ToTable("MovieLanguage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LanguageId).HasColumnName("language_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");

            entity.HasOne(d => d.Language).WithMany(p => p.MovieLanguages)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieLang__langu__625A9A57");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieLanguages)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MovieLang__movie__6166761E");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3213E83FE9E7B8D9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.DiscountCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("discount_code");
            entity.Property(e => e.FinalTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("final_total");
            entity.Property(e => e.IsPaid).HasColumnName("is_paid");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("order_code");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Show).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__show_id__7B264821");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__user_id__7C1A6C5A");
        });

        modelBuilder.Entity<OrderFood>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderFoo__3213E83F20E93812");

            entity.ToTable("OrderFood");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FoodId).HasColumnName("food_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Food).WithMany(p => p.OrderFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderFood__food___10216507");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderFoods)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderFood__order__0F2D40CE");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3213E83F538E7E3F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("coupon_code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.MinPurchaseAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("min_purchase_amount");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3213E83F368B62D2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("message");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__movie_i__681373AD");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__user_id__690797E6");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3213E83F69F23545");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Columns).HasColumnName("columns");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Rows).HasColumnName("rows");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seats__3213E83F6233BBCA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RowNumber).HasColumnName("row_number");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__room_id__6FB49575");

            entity.HasOne(d => d.SeatType).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__seat_type__70A8B9AE");
        });

        modelBuilder.Entity<SeatPricing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SeatPric__3213E83F1C6795B3");

            entity.ToTable("SeatPricing");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.SeatType).WithMany(p => p.SeatPricings)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeatPrici__seat___7849DB76");

            entity.HasOne(d => d.Show).WithMany(p => p.SeatPricings)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeatPrici__show___7755B73D");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SeatType__3213E83FF3845DE4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sections__3213E83FF0885D7B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FloorId).HasColumnName("floor_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Floor).WithMany(p => p.Sections)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sections__floor___05A3D694");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shops__3213E83F5D842DB2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shows__3213E83FF38C6D48");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.ShowCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("show_code");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Movie).WithMany(p => p.Shows)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__movie_id__73852659");

            entity.HasOne(d => d.Room).WithMany(p => p.Shows)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__room_id__74794A92");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3213E83F8BFCC7FB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.RowNum).HasColumnName("row_num");
            entity.Property(e => e.SeatNum).HasColumnName("seat_num");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__order_i__7EF6D905");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F2F831691");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E61642C6BC3BB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Birthday)
                .HasColumnType("date")
                .HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .HasColumnName("fullname");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Money)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("money");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("resetToken");
            entity.Property(e => e.ResetTokenExpiry)
                .HasColumnType("datetime")
                .HasColumnName("resetTokenExpiry");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserPromotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProm__3213E83FE3A66FC5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsedAt)
                .HasColumnType("datetime")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Promotion).WithMany(p => p.UserPromotions)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__promo__18B6AB08");

            entity.HasOne(d => d.User).WithMany(p => p.UserPromotions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__user___17C286CF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
