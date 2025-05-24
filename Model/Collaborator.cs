using CredipathAPI.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CredipathAPI.Model
{
    public class Collaborator : BaseEntity
    {
        public string Identifier { get; set; } 
        public string Phone { get; set; }
        public string Mobile { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        public int CreatedById { get; set; }
        
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }
        
    }
    
    // Esta clase se usa configurar las relaciones y evitar ciclos de eliminación en cascada
    public static class CollaboratorModelConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
