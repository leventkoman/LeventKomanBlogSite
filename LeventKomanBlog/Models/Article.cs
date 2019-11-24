namespace LeventKomanBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Article")]
    public partial class Article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Article()
        {
            Comment = new HashSet<Comment>();
            Sticker = new HashSet<Sticker>();
        }

        public int Id { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public string Contents { get; set; }

        [StringLength(500)]
        public string Photo { get; set; }

        public DateTime? Date { get; set; }

        public int? CategoryId { get; set; }

        public int? UserId { get; set; }

        public int? ReadsCount { get; set; }

        public virtual Category Category { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sticker> Sticker { get; set; }
    }
}
