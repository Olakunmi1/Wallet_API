using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wallet.Data.Entities;

namespace Wallet.Data.Dbcontext
{
    //inheriting from IdentityDB Context give sus acces to AspNet users table and d rest 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
           : base(options)
        {
           
        }

        //have DB sets below 
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<WalletAccount> WalletAccounts { get; set; }
        public DbSet<TransactionHistory>  TransactionHistories { get; set; }


        //this piece below is used to set a global precision for decimal's in my Entitiy Models.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                //("decimal(20, 4)");
                property.SetPrecision(20);
                property.SetScale(4); // set  precision of decimal 
            }
        }
    }
}
