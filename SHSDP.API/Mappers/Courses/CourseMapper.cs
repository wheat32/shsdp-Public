namespace SHSDP.API.Mappers.Courses;

public static class CourseMapper
{
    public static Shared.Models.API.Course ToApiModel(this Entities.Courses.Course course)
    {
        return new Shared.Models.API.Course(
            CourseCode: course.CourseCode,
            CourseName: course.CourseName,
            CourseDisabled: course.CourseDisabled
        );
    }
    
    public static Entities.Courses.Course ToEntityModel(this Shared.Models.API.Course course)
    {
        return new Entities.Courses.Course
        {
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            CourseDisabled = course.CourseDisabled
        };
    }
}