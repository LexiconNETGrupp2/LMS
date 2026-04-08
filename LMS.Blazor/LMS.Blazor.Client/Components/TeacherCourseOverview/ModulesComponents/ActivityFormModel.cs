using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Components.TeacherCourseOverview.ModulesComponents;

public sealed class ActivityFormModel
{
    public Guid? ActivityId { get; set; }

    [Required]
    public Guid ModuleId { get; set; }

    public string ModuleName { get; set; } = string.Empty;
    public string ModuleStartDateTimeMin { get; set; } = string.Empty;
    public string ModuleEndDateTimeMax { get; set; } = string.Empty;

    [Required(ErrorMessage = "Aktivitetsnamn är obligatoriskt.")]
    [StringLength(120, ErrorMessage = "Aktivitetsnamnet får vara högst 120 tecken.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning är obligatorisk.")]
    [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
    public string StartDateTimeLocal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
    public string EndDateTimeLocal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Välj en aktivitetstyp.")]
    public string TypeName { get; set; } = string.Empty;

    public bool IsEditMode => ActivityId.HasValue;
}
