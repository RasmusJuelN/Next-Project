# Database Models Guide

This guide provides comprehensive documentation for creating, configuring, and maintaining Entity Framework models in the Backend project.

## Overview

The Database project uses Entity Framework Core with a Code First approach to manage the application's data model. Models are defined as C# classes with attributes and Fluent API configurations to define the database schema.

## Existing Models

### User Models (Table Per Hierarchy)

The user system implements a Table Per Hierarchy (TPH) inheritance pattern:

- **`UserBaseModel`**: Base class containing common user properties
  - Contains fundamental properties like `Id`, `Guid`, `UserName`, `FullName`
  - Includes role and permission management through `PrimaryRole` and `Permissions`
  - Has navigation properties for related entities like `ActiveQuestionnaires` and `TrackedRefreshTokens`

- **`StudentModel`**: Inherits from `UserBaseModel`
  - Currently contains no additional properties
  - Represents student users in the system

- **`TeacherModel`**: Inherits from `UserBaseModel`
  - Currently contains no additional properties
  - Represents teacher users in the system

### Questionnaire Models

- **`QuestionnaireTemplateModel`**: Defines reusable questionnaire templates
  - Contains template metadata like `Title`, `Description`, `CreatedAt`
  - Has a computed `IsLocked` property to prevent modification when active
  - Related to questions and active questionnaires

- **`QuestionnaireQuestionModel`**: Individual questions within templates
  - Links to templates and contains question options

- **`QuestionnaireOptionModel`**: Answer options for questions

- **`ActiveQuestionnaireModel`**: Active instances of questionnaire templates
  - Created when templates are activated for student-teacher pairs
  - Tracks completion status for both participants

### Response Models

- **`ActiveQuestionnaireResponseBaseModel`**: Base class for responses
- **`ActiveQuestionnaireStudentResponseModel`**: Student-specific responses
- **`ActiveQuestionnaireTeacherResponseModel`**: Teacher-specific responses

### Support Models

- **`TrackedRefreshTokenModel`**: Manages JWT refresh tokens for authentication
- **`ApplicationLogsModel`**: Stores application logging information

## Creating a New Model

### Basic Model Structure

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

/// <summary>
/// Represents a sample entity in the system.
/// </summary>
/// <remarks>
/// Detailed description of the model's purpose and usage.
/// </remarks>
[Table("SampleEntity")]
[Index(nameof(Name), IsUnique = true)]
public class SampleEntityModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the optional description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets when this entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<RelatedModel> RelatedEntities { get; set; } = [];
}
```

### Creating a Model with TPH Inheritance

When you need to create models that share common properties but have specific behaviors:

```csharp
// Base model
[Table("Vehicle")]
public abstract class VehicleBaseModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Make { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Model { get; set; }
    
    [Required]
    public int Year { get; set; }
}

// Derived models
public class CarModel : VehicleBaseModel
{
    public int NumberOfDoors { get; set; }
    public bool HasSunroof { get; set; }
}

public class TruckModel : VehicleBaseModel
{
    public decimal PayloadCapacity { get; set; }
    public bool HasTrailerHitch { get; set; }
}
```

## Common Attributes

### Class-Level Attributes

- **`[Table("TableName")]`**: Specifies the database table name
- **`[Index(nameof(Property), IsUnique = true)]`**: Creates database indexes
- **`[Index(nameof(Prop1), nameof(Prop2), IsDescending = [false, true])]`**: Multi-column indexes with sort order

### Property-Level Attributes

#### Validation Attributes
- **`[Key]`**: Marks the primary key
- **`[Required]`**: Makes the property non-nullable
- **`[MaxLength(100)]`**: Sets maximum string length
- **`[MinLength(5)]`**: Sets minimum string length
- **`[Range(1, 100)]`**: Validates numeric ranges

#### Database Mapping Attributes
- **`[Column("ColumnName")]`**: Specifies database column name
- **`[ForeignKey("PropertyName")]`**: Explicitly defines foreign keys
- **`[NotMapped]`**: Excludes property from database mapping
- **`[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`**: Auto-generated values

#### Example with Multiple Attributes
```csharp
[Required]
[MaxLength(150)]
[Column("product_name")]
public required string ProductName { get; set; }

[Range(0.01, 99999.99)]
[Column(TypeName = "decimal(18,2)")]
public decimal Price { get; set; }

[NotMapped]
public string DisplayName => $"{ProductName} - ${Price:F2}";
```

## Adding Models to Context

After creating a model, you must add it to the `Context.cs` file:

### 1. Add DbSet Property

```csharp
public class Context : DbContext
{
    // ... existing DbSets ...
    
    internal DbSet<SampleEntityModel> SampleEntities { get; set; }
    internal DbSet<VehicleBaseModel> Vehicles { get; set; }
    internal DbSet<CarModel> Cars { get; set; }
    internal DbSet<TruckModel> Trucks { get; set; }
}
```

### 2. Configure in OnModelCreating (if needed)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure SampleEntity
    modelBuilder.Entity<SampleEntityModel>(e => {
        e.Property(s => s.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        
        // Configure relationships
        e.HasMany(s => s.RelatedEntities)
         .WithOne(r => r.SampleEntity)
         .HasForeignKey(r => r.SampleEntityId)
         .OnDelete(DeleteBehavior.Cascade);
    });
    
    // Configure TPH inheritance
    modelBuilder.Entity<VehicleBaseModel>()
        .HasDiscriminator<string>("VehicleType")
        .HasValue<CarModel>("Car")
        .HasValue<TruckModel>("Truck");
    
    // ... rest of configuration
}
```

## Fluent API Configuration

The Fluent API provides more advanced configuration options than attributes allow:

### Property Configuration

```csharp
modelBuilder.Entity<SampleEntityModel>(e => {
    // Primary key
    e.HasKey(s => s.Id);
    
    // Property constraints
    e.Property(s => s.Name)
     .IsRequired()
     .HasMaxLength(100)
     .HasColumnName("entity_name");
    
    // Default values
    e.Property(s => s.CreatedAt)
     .HasDefaultValueSql("SYSUTCDATETIME()");
    
    // Computed columns
    e.Property(s => s.FullDescription)
     .HasComputedColumnSql("[Name] + ' - ' + [Description]");
    
    // Data type specification
    e.Property(s => s.Price)
     .HasColumnType("decimal(18,2)");
    
    // Enum configuration
    e.Property(s => s.Status)
     .HasConversion<string>();
});
```

### Relationship Configuration

```csharp
// One-to-Many
modelBuilder.Entity<QuestionnaireTemplateModel>(e => {
    e.HasMany(q => q.Questions)
     .WithOne(q => q.QuestionnaireTemplate)
     .HasPrincipalKey(q => q.Id)
     .HasForeignKey(q => q.QuestionnaireTemplateFK)
     .OnDelete(DeleteBehavior.Cascade);
});

// Many-to-Many
modelBuilder.Entity<StudentModel>()
    .HasMany(s => s.ActiveQuestionnaires)
    .WithMany(aq => aq.Students)
    .UsingEntity<Dictionary<string, object>>(
        "StudentActiveQuestionnaire",
        j => j.HasOne<ActiveQuestionnaireModel>().WithMany().HasForeignKey("ActiveQuestionnaireId"),
        j => j.HasOne<StudentModel>().WithMany().HasForeignKey("StudentId")
    );

// One-to-One
modelBuilder.Entity<UserModel>()
    .HasOne(u => u.Profile)
    .WithOne(p => p.User)
    .HasForeignKey<ProfileModel>(p => p.UserId);
```

### Index Configuration

```csharp
modelBuilder.Entity<SampleEntityModel>(e => {
    // Simple index
    e.HasIndex(s => s.Name).IsUnique();
    
    // Composite index
    e.HasIndex(s => new { s.Name, s.Category });
    
    // Filtered index
    e.HasIndex(s => s.Email)
     .IsUnique()
     .HasFilter("[Email] IS NOT NULL");
    
    // Index with included columns (SQL Server)
    e.HasIndex(s => s.CategoryId)
     .IncludeProperties(s => new { s.Name, s.Description });
});
```

### Advanced Configuration Examples

#### Table Per Hierarchy (TPH) Setup
```csharp
// Configure discriminator
modelBuilder.Entity<UserBaseModel>()
    .HasDiscriminator<string>("UserType")
    .HasValue<StudentModel>("Student")
    .HasValue<TeacherModel>("Teacher")
    .HasValue<AdminModel>("Admin");

// Configure shared properties
modelBuilder.Entity<UserBaseModel>(e => {
    e.Property(u => u.PrimaryRole).HasConversion<string>();
    e.HasIndex(u => u.UserName).IsUnique();
    e.HasIndex(u => u.Guid).IsUnique();
});
```

#### Soft Delete Configuration
```csharp
modelBuilder.Entity<SampleEntityModel>(e => {
    e.Property(s => s.IsDeleted).HasDefaultValue(false);
    e.HasQueryFilter(s => !s.IsDeleted);
    e.HasIndex(s => s.IsDeleted);
});
```

#### Global Query Filters
```csharp
// Apply to all entities implementing ISoftDelete
foreach (var entityType in modelBuilder.Model.GetEntityTypes())
{
    if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
    {
        var parameter = Expression.Parameter(entityType.ClrType, "e");
        var body = Expression.Equal(
            Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
            Expression.Constant(false));
        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
    }
}
```

## Best Practices

### Model Design
1. Use meaningful, descriptive names for models and properties
2. Include comprehensive XML documentation
3. Use appropriate data types and constraints
4. Consider performance implications of relationships
5. Implement interfaces for common behaviors (e.g., `IAuditable`, `ISoftDelete`)

### Attribute vs Fluent API
- Use **attributes** for simple, property-specific configurations
- Use **Fluent API** for complex relationships, advanced configurations, and global settings
- Prefer Fluent API for configurations that might change based on environment

### Performance Considerations
1. Add indexes on frequently queried properties
2. Use appropriate `DeleteBehavior` for relationships
3. Consider lazy loading implications with virtual properties
4. Use `[NotMapped]` for computed properties that don't need database storage

### Migration Strategy
1. Test model changes in development first
2. Review generated migrations before applying
3. Consider data migration scripts for complex changes
4. Use explicit migration names that describe the change

## Example: Complete Model Implementation

Here's a complete example showing a new `CourseModel` with proper configuration:

```csharp
// Models/CourseModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

/// <summary>
/// Represents a course that can be assigned to students and managed by teachers.
/// </summary>
[Table("Course")]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(Name))]
public class CourseModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(10)]
    public required string Code { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime LastUpdated { get; set; }
    
    // Navigation properties
    public virtual ICollection<StudentModel> Students { get; set; } = [];
    public virtual ICollection<TeacherModel> Teachers { get; set; } = [];
}

// Context.cs additions
internal DbSet<CourseModel> Courses { get; set; }

// OnModelCreating additions
// Partial example: Add this configuration inside your OnModelCreating method
modelBuilder.Entity<CourseModel>(e => {
    e.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
    e.Property(c => c.LastUpdated).HasDefaultValueSql("SYSUTCDATETIME()");
    
    // Many-to-many with students
    e.HasMany(c => c.Students)
     .WithMany(s => s.Courses)
     .UsingEntity("StudentCourse");
     
    // Many-to-many with teachers
    e.HasMany(c => c.Teachers)
     .WithMany(t => t.Courses)
     .UsingEntity("TeacherCourse");
});
```

This guide provides a comprehensive foundation for working with Entity Framework models in the Backend project. Always refer to the existing models for patterns and consistency when creating new entities.

