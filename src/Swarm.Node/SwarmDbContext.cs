using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Swarm.Basic.Entity;

// ReSharper disable once CheckNamespace
namespace Swarm.Node
{
    public class SwarmDbContext : DbContext, IDesignTimeDbContextFactory<SwarmDbContext>
    {
        public DbSet<Job> Job { get; set; }
        public DbSet<JobProperty> JobProperty { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Basic.Entity.Node> Node { get; set; }
        public DbSet<ClientProcess> ClientProcess { get; set; }

        public SwarmDbContext()
        {
        }

        public SwarmDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Job>().HasIndex(x => x.Group);
            modelBuilder.Entity<Job>().HasIndex(x => x.Name);
            modelBuilder.Entity<Job>().HasIndex(x => new {x.Name, x.Group});
            modelBuilder.Entity<Job>().HasIndex(x => x.Owner);
            modelBuilder.Entity<Job>().HasIndex(x => x.CreationTime);

            modelBuilder.Entity<JobProperty>().HasIndex(x => x.JobId);
            modelBuilder.Entity<JobProperty>().HasIndex(x => new {x.JobId, x.Name}).IsUnique();

            modelBuilder.Entity<Client>().HasIndex(x => new {x.Name, x.Group}).IsUnique();
            modelBuilder.Entity<Client>().HasIndex(x => x.ConnectionId).IsUnique();
            modelBuilder.Entity<Client>().HasIndex(x => x.CreationTime);

            modelBuilder.Entity<Log>().HasIndex(x => x.JobId);
            modelBuilder.Entity<Log>().HasIndex(x => x.CreationTime);
            modelBuilder.Entity<Log>().HasIndex(x => new {x.JobId, x.TraceId});

            modelBuilder.Entity<Basic.Entity.Node>().HasIndex(x => new {x.SchedName});
            modelBuilder.Entity<Basic.Entity.Node>().HasIndex(x => new {x.SchedName, NodeId = x.SchedInstanceId})
                .IsUnique();
            modelBuilder.Entity<Basic.Entity.Node>().HasIndex(x => new {NodeId = x.SchedInstanceId}).IsUnique();
            modelBuilder.Entity<Basic.Entity.Node>().HasIndex(x => x.CreationTime);

            modelBuilder.Entity<ClientProcess>().HasIndex(x => x.CreationTime);
            modelBuilder.Entity<ClientProcess>().HasIndex(x => new {x.Name, x.Group});
        }

        public SwarmDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SwarmDbContext>();
            builder.UseSqlServer(GetConnectionString(args[0]));

            return new SwarmDbContext(builder.Options);
        }

        private string GetConnectionString(string config)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(config, optional: false);

            var configuration = builder.Build();
            return configuration.GetSection("Swarm").GetValue<string>("ConnectionString");
        }
    }
}