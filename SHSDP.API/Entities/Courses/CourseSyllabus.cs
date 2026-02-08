using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SHSDP.API.Entities.Courses;

[Table("course_syllabus", Schema = "courses")]
public partial class CourseSyllabus
{
    [Key]
    [Column("course_code")]
    [StringLength(5)]
    public string CourseCode { get; set; } = null!;

    [Column("cs_file_path")]
    public string CsFilePath { get; set; } = null!;

    [Column("cs_year")]
    public short CsYear { get; set; }

    [Column("cs_disabled")]
    public bool CsDisabled { get; set; }

    [ForeignKey("CourseCode")]
    [InverseProperty("CourseSyllabus")]
    public virtual Course CourseCodeNavigation { get; set; } = null!;
}
