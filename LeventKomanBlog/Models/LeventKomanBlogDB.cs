namespace LeventKomanBlog.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LeventKomanBlogDB : DbContext
    {
        public LeventKomanBlogDB()
            : base("name=LeventKomanBlogDB")
        {
        }

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Sticker> Sticker { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasMany(e => e.Sticker)
                .WithMany(e => e.Article)
                .Map(m => m.ToTable("ArticleSticker").MapLeftKey("ArticleId").MapRightKey("StickerId"));

            modelBuilder.Entity<User>()
                .Property(e => e.Surname)
                .IsFixedLength();
        }
    }
}
