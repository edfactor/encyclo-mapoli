using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Configuration;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Data.Interceptors;

public sealed class AuditSaveChangesInterceptorHashTests
{
    [Fact]
    [Description("PS-1453 : Hash calculation produces consistent SHA256 hash")]
    public void CalculateChangesHash_WithSameData_ProducesSameHash()
    {
        // Arrange
        var changes1 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" },
            new() { ColumnName = "Status", OriginalValue = "Active", NewValue = "Inactive" }
        };

        var changes2 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" },
            new() { ColumnName = "Status", OriginalValue = "Active", NewValue = "Inactive" }
        };

        // Act
        var hash1 = AuditSaveChangesInterceptor.CalculateChangesHash(changes1);
        var hash2 = AuditSaveChangesInterceptor.CalculateChangesHash(changes2);

        // Assert
        hash1.ShouldNotBeNull();
        hash2.ShouldNotBeNull();
        hash1.ShouldBe(hash2);
        hash1.Length.ShouldBe(64); // SHA256 produces 64 hex characters
    }

    [Fact]
    [Description("PS-1453 : Hash calculation produces different hash for different data")]
    public void CalculateChangesHash_WithDifferentData_ProducesDifferentHash()
    {
        // Arrange
        var changes1 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
        };

        var changes2 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "Modified" }
        };

        // Act
        var hash1 = AuditSaveChangesInterceptor.CalculateChangesHash(changes1);
        var hash2 = AuditSaveChangesInterceptor.CalculateChangesHash(changes2);

        // Assert
        hash1.ShouldNotBeNull();
        hash2.ShouldNotBeNull();
        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    [Description("PS-1453 : Hash calculation returns null for null input")]
    public void CalculateChangesHash_WithNullInput_ReturnsNull()
    {
        // Act
        var hash = AuditSaveChangesInterceptor.CalculateChangesHash(null);

        // Assert
        hash.ShouldBeNull();
    }

    [Fact]
    [Description("PS-1453 : Hash calculation produces different hash when order changes")]
    public void CalculateChangesHash_WithDifferentOrder_ProducesDifferentHash()
    {
        // Arrange
        var changes1 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" },
            new() { ColumnName = "Status", OriginalValue = "Active", NewValue = "Inactive" }
        };

        var changes2 = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Status", OriginalValue = "Active", NewValue = "Inactive" },
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
        };

        // Act
        var hash1 = AuditSaveChangesInterceptor.CalculateChangesHash(changes1);
        var hash2 = AuditSaveChangesInterceptor.CalculateChangesHash(changes2);

        // Assert
        hash1.ShouldNotBeNull();
        hash2.ShouldNotBeNull();
        hash1.ShouldNotBe(hash2); // Order matters in JSON serialization
    }

    [Fact]
    [Description("PS-1453 : Verify audit event integrity returns true for valid hash")]
    public void VerifyAuditEventIntegrity_WithValidHash_ReturnsTrue()
    {
        // Arrange
        var changes = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
        };

        var auditEvent = new AuditEvent
        {
            TableName = "TestTable",
            Operation = "Modified",
            ChangesJson = changes,
            ChangesHash = AuditSaveChangesInterceptor.CalculateChangesHash(changes)
        };

        // Act
        var isValid = AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(auditEvent);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1453 : Verify audit event integrity returns false for tampered data")]
    public void VerifyAuditEventIntegrity_WithTamperedData_ReturnsFalse()
    {
        // Arrange
        var originalChanges = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
        };

        var auditEvent = new AuditEvent
        {
            TableName = "TestTable",
            Operation = "Modified",
            ChangesJson = originalChanges,
            ChangesHash = AuditSaveChangesInterceptor.CalculateChangesHash(originalChanges)
        };

        // Tamper with the data
        auditEvent.ChangesJson = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "Tampered" }
        };

        // Act
        var isValid = AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(auditEvent);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-1453 : Verify audit event integrity returns false for missing hash")]
    public void VerifyAuditEventIntegrity_WithMissingHash_ReturnsFalse()
    {
        // Arrange
        var auditEvent = new AuditEvent
        {
            TableName = "TestTable",
            Operation = "Modified",
            ChangesJson = new List<AuditChangeEntry>
            {
                new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
            },
            ChangesHash = null
        };

        // Act
        var isValid = AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(auditEvent);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-1453 : Verify audit event integrity throws for null audit event")]
    public void VerifyAuditEventIntegrity_WithNullAuditEvent_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(null!));
    }

    [Fact]
    [Description("PS-1453 : Hash verification is case-insensitive")]
    public void VerifyAuditEventIntegrity_WithDifferentHashCase_ReturnsTrue()
    {
        // Arrange
        var changes = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Name", OriginalValue = "Old", NewValue = "New" }
        };

        var hash = AuditSaveChangesInterceptor.CalculateChangesHash(changes);
        var auditEvent = new AuditEvent
        {
            TableName = "TestTable",
            Operation = "Modified",
            ChangesJson = changes,
            ChangesHash = hash?.ToLower() // Store lowercase version
        };

        // Act (CalculateChangesHash returns uppercase)
        var isValid = AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(auditEvent);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1453 : Hash is calculated and stored when audit event is created")]
    public async Task SaveChanges_CreatesAuditEventWithHash()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var config = new DataConfig { EnableAudit = true };
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        // HttpContext can be null for unit tests (no HTTP context available in interceptor)
        mockHttpContextAccessor.Setup(h => h.HttpContext).Returns((HttpContext?)null);
        var interceptor = new AuditSaveChangesInterceptor(config, null, mockHttpContextAccessor.Object);

        await using var context = new TestDbContext(options, interceptor);

        var testEntity = new TestEntity { Id = 1, Name = "Test" };
        context.TestEntities.Add(testEntity);

        // Act
        await context.SaveChangesAsync();

        // Modify entity to trigger audit
        testEntity.Name = "Modified";
        await context.SaveChangesAsync();

        // Assert
        var auditEvent = await context.AuditEvents.FirstOrDefaultAsync();
        auditEvent.ShouldNotBeNull();
        auditEvent.ChangesHash.ShouldNotBeNull($"ChangesHash should not be null. ChangesJson: {auditEvent.ChangesJson?.Count ?? -1} items");
        auditEvent.ChangesJson.ShouldNotBeNull();
        auditEvent.ChangesHash.Length.ShouldBe(64); // SHA256 hex string

        // Verify the hash is correct
        var recalculatedHash = AuditSaveChangesInterceptor.CalculateChangesHash(auditEvent.ChangesJson);
        auditEvent.ChangesHash.ShouldBe(recalculatedHash, $"Stored: {auditEvent.ChangesHash}, Calculated: {recalculatedHash}");

        var isValid = AuditSaveChangesInterceptor.VerifyAuditEventIntegrity(auditEvent);
        isValid.ShouldBeTrue();
    }

    // Test DbContext for in-memory testing
    private sealed class TestDbContext : DbContext
    {
        private readonly AuditSaveChangesInterceptor _interceptor;

        public TestDbContext(DbContextOptions<TestDbContext> options, AuditSaveChangesInterceptor interceptor)
            : base(options)
        {
            _interceptor = interceptor;
        }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
        public DbSet<AuditEvent> AuditEvents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_interceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<TestEntity>().ToTable("TestEntities");

            // Configure AuditEvent for in-memory database (simplified - no conversion needed for in-memory)
            modelBuilder.Entity<AuditEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<AuditEvent>().ToTable("AuditEvents");
            modelBuilder.Entity<AuditEvent>().Property(e => e.ChangesJson).HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Web),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<List<AuditChangeEntry>>(v, System.Text.Json.JsonSerializerOptions.Web)
            );
        }
    }

    // Test entity
    private sealed class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
