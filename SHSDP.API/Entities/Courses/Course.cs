using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SHSDP.API.Entities.Courses;

[Table("course", Schema = "courses")]
public partial class Course
{
    [Key]
    [Column("course_code")]
    [StringLength(5)]
    public string CourseCode { get; set; } = null!;

    [Column("course_name")]
    public string CourseName { get; set; } = null!;

    [Column("course_disabled")]
    public bool CourseDisabled { get; set; }

    [InverseProperty("CourseCodeNavigation")]
    public virtual ICollection<AssignmentRuntime> AssignmentRuntimes { get; set; } = new List<AssignmentRuntime>();

    [InverseProperty("CourseCodeNavigation")]
    public virtual CourseSyllabus? CourseSyllabus { get; set; }
}
