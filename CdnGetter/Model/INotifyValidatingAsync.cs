namespace CdnGetter.Model;

public interface INotifyValidatingAsync
{
    Task OnValidatingAsync(System.ComponentModel.DataAnnotations.ValidationContext validationContext, Microsoft.EntityFrameworkCore.EntityState state, IServiceProvider serviceProvider);
}
