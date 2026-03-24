using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Components.Modules;

public sealed class ModuleFormModel
{
    public Guid? ModuleId { get; set; }

    [Required(ErrorMessage = "Välj en kurs.")]
    public Guid? CourseId { get; set; }

    [Required(ErrorMessage = "Modulnamn är obligatoriskt.")]
    [StringLength(120, ErrorMessage = "Modulnamnet får vara högst 120 tecken.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
    public DateOnly? EndDate { get; set; }

    [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken.")]
    public string Description { get; set; } = string.Empty;

    public bool IsEditMode => ModuleId.HasValue;
}
