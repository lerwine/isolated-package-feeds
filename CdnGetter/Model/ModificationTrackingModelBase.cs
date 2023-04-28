using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

public abstract class ModificationTrackingModelBase : IValidatableObject
{
    /// <summary>
    /// Gets or sets the date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
    
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        EntityState state = (validationContext.GetService(typeof(EntityEntry)) as EntityEntry)?.State ?? EntityState.Detached;
        Validate(validationContext, state, results);
        return results;
    }

    protected virtual void Validate(ValidationContext validationContext, EntityState state, List<ValidationResult> results)
    {
        switch (state)
        {
            case EntityState.Added:
                ModifiedOn = CreatedOn = DateTime.Now;
                break;
            case EntityState.Modified:
                ModifiedOn = DateTime.Now;
                if ((CreatedOn = CreatedOn.AsLocalDateTime()) > ModifiedOn)
                    results.Add(new ValidationResult("{nameof(CreatedOn)} must be before {ModifiedOn}", new[] { nameof(CreatedOn), nameof(ModifiedOn) }));
                break;
        }
    }
    
    protected static void OnBuildModificationTrackingModel<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : ModificationTrackingModelBase
    {
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
    }
}
