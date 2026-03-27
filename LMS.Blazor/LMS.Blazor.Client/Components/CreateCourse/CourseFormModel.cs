using LMS.Blazor.Client.Components.Modules;
using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Components.CreateCourse;

[ValidatableType]
public class CourseFormModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Kursnamnet är obligatoriskt.")]
    [StringLength(120, ErrorMessage = "Kursnamnet får vara högst 120 tecken.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken.")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
    public DateOnly? StartDate { get; set; }
    
    [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
    public DateOnly? EndDate { get; set; }
    

    public List<ModuleFormModel> Modules { get; set; } = [];
}
