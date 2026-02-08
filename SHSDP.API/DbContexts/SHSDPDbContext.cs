using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SHSDP.API.Entities.Courses;

namespace SHSDP.API.DbContexts;

public partial class SHSDPDbContext : DbContext
{
    public SHSDPDbContext(DbContextOptions<SHSDPDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssignmentRuntime> AssignmentRuntimes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseSyllabus> CourseSyllabi { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignmentRuntime>(entity =>
        {
            entity.HasKey(e => e.AssignId).HasName("assignment_runtime_pk");

            entity.Property(e => e.AssignId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CourseCode).IsFixedLength();

            entity.HasOne(d => d.CourseCodeNavigation).WithMany(p => p.AssignmentRuntimes).HasConstraintName("assignment_runtime_course_course_code_fk");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseCode).HasName("course_pk");

            entity.Property(e => e.CourseCode).IsFixedLength();
        });

        modelBuilder.Entity<CourseSyllabus>(entity =>
        {
            entity.HasKey(e => e.CourseCode).HasName("course_syllabus_pk");

            entity.Property(e => e.CourseCode).IsFixedLength();
            entity.Property(e => e.CsYear).HasDefaultValueSql("EXTRACT(year FROM CURRENT_DATE)");

            entity.HasOne(d => d.CourseCodeNavigation).WithOne(p => p.CourseSyllabus).HasConstraintName("course_syllabus_course_course_code_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
