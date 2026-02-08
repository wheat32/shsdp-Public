using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SHSDP.API.Entities.Courses;

[Table("assignment_runtime", Schema = "courses")]
public partial class AssignmentRuntime
{
    [Key]
    [Column("assign_id")]
    public Guid AssignId { get; set; }

    [Column("course_code")]
    [StringLength(5)]
    public string CourseCode { get; set; } = null!;

    [Column("assign_name")]
    public string AssignName { get; set; } = null!;

    [Column("assign_disabled")]
    public bool AssignDisabled { get; set; }

    [Column("assign_url")]
    public string AssignUrl { get; set; } = null!;

    [ForeignKey("CourseCode")]
    [InverseProperty("AssignmentRuntimes")]
    public virtual Course CourseCodeNavigation { get; set; } = null!;
}
