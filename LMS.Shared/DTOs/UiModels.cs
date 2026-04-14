using System;
using System.Collections.Generic;
using System.Text;

// UiModels.cs temporarely added while playing around with LMS.Blazor.Client/Components/StudentDashboard
namespace LMS.Shared.DTOs
{
    public record ActivityItem(
        string Title,
        string Meta,
        string? Badge = null
        );

    public record SubmissionItem(
        string Title,
        string DueText,
        string? Badge = null
        );

    public record ProgressItem(
        string Name,
        int Percent
        );
}
