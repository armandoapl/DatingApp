using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions options) : base(options){}

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messsages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Likes configurations
            builder.Entity<UserLike>()
                   .HasKey(k => new {k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()// IMPORTANT if I were UsingSQLServer here I need to use "DeleteBehavior.NoAction" otherwise I will get an error.
                   .HasOne(s => s.SourceUser)
                   .WithMany(l => l.LikedUsers)
                   .HasForeignKey(s => s.SourceUserId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<UserLike>()
                   .HasOne(s => s.LikedUser)
                   .WithMany(l => l.LikedByUsers)
                   .HasForeignKey(s => s.LikedUserId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Messages configuration
            builder.Entity<Message>()
                   .HasOne(u => u.Sender)
                   .WithMany(m => m.MessagesSent)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Message>()
                   .HasOne(u => u.Recipient)
                   .WithMany(m => m.MessagesRecived)
                   .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
