namespace SHSDP.Shared.Models.API;

public record Course(String CourseCode, String CourseName, bool CourseDisabled);

public record GetCourseRequest(String CourseCode);
public record GetCourseResponse(Course? Course);

public record GetCoursesRequest();
public record GetCoursesResponse(IEnumerable<Course> Courses);


public record CourseSyllabus(String CourseCode, String SyllabusFilePath, short SyllabusYear, bool Disabled);

public record GetCourseSyllabusRequest(String CourseCode);
public record GetCourseSyllabusResponse(CourseSyllabus? CourseSyllabus);

public record GetCourseSyllabiRequest();
public record GetCourseSyllabiResponse(IEnumerable<CourseSyllabus> CourseSyllabi);


public record AssignmentRuntime(Guid AssignId, String CourseCode, String AssignName, bool Disabled, String AssignUrl);

public record GetAssignmentRuntimeRequest(Guid AssignId);
public record GetAssignmentRuntimeResponse(AssignmentRuntime? AssignmentRuntime);

public record GetAssignmentRuntimeByCourseRequest(String CourseCode);
public record GetAssignmentRuntimeByCourseResponse(IEnumerable<AssignmentRuntime> AssignmentRuntimes);

public record GetCourseConfigurationRequest(String CourseCode);
public record GetCourseConfigurationResponse(Course Course, CourseSyllabus Syllabus, List<AssignmentRuntime> AssignmentRuntime);

public record GetAllCourseConfigurationsRequest();
public record GetAllCourseConfigurationsResponse(IEnumerable<GetCourseConfigurationResponse> Configurations);
