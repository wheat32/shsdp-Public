using SHSDP.API.Entities.Courses;

namespace SHSDP.API.Mappers.Courses;

public static class CourseSyllabusMapper
{
    public static SHSDP.Shared.Models.API.CourseSyllabus ToApiModel(this CourseSyllabus courseSyllabus)
    {
        return new SHSDP.Shared.Models.API.CourseSyllabus(
            CourseCode: courseSyllabus.CourseCode,
            SyllabusFilePath: courseSyllabus.CsFilePath,
            SyllabusYear: courseSyllabus.CsYear,
            Disabled: courseSyllabus.CsDisabled
        );
    }
    
    public static CourseSyllabus ToEntityModel(this SHSDP.Shared.Models.API.CourseSyllabus courseSyllabus)
    {
        return new CourseSyllabus
        {
            CourseCode = courseSyllabus.CourseCode,
            CsFilePath = courseSyllabus.SyllabusFilePath,
            CsYear = courseSyllabus.SyllabusYear,
            CsDisabled = courseSyllabus.Disabled
        };
    }
}

