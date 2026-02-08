using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHSDP.API.DbContexts;
using SHSDP.API.Mappers.Courses;
using SHSDP.Shared.Models.API;
using Course = SHSDP.API.Entities.Courses.Course;
using CourseSyllabus = SHSDP.API.Entities.Courses.CourseSyllabus;
using AssignmentRuntime = SHSDP.API.Entities.Courses.AssignmentRuntime;

namespace SHSDP.API.Controllers;

[Authorize(AuthenticationSchemes = "LoginTokenScheme")]
[ApiController]
[Route("api/v1/[controller]")]
public class CourseController : ControllerBase
{
    private SHSDPDbContext ShsdpDbContext { get; init; }
    private ILogger<CourseController> Logger { get; init; }
    
    public CourseController(SHSDPDbContext context, ILogger<CourseController> logger)
    {
        ShsdpDbContext = context;
        Logger = logger;
    }

    [HttpGet("GetCourse")]
    [ProducesResponseType(typeof(GetCourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(String), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourse([Required] [FromQuery] GetCourseRequest req)
    {
        try
        {
            Logger.LogInformation("GetCourse called with CourseCode: {CourseCode}", req.CourseCode);
            
            Course? course = await ShsdpDbContext.Courses.FirstOrDefaultAsync(course => course.CourseCode.Equals(req.CourseCode));

            if (course == null)
            {
                Logger.LogWarning("Course not found: {CourseCode}", req.CourseCode);
                return NotFound("Course not found");
            }
            
            Logger.LogInformation("Course found: {CourseCode}", req.CourseCode);
            return Ok(new GetCourseResponse(
                    Course: course.ToApiModel()
                ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting course: {CourseCode}", req.CourseCode);
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetCourses")]
    [ProducesResponseType(typeof(GetCoursesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourses([FromQuery] GetCoursesRequest req)
    {
        try
        {
            Logger.LogInformation("GetCourses called");
            
            List<Course> courses = await ShsdpDbContext.Courses.ToListAsync();
            
            Logger.LogInformation("Retrieved {Count} courses", courses.Count);
            return Ok(new GetCoursesResponse(
                Courses: courses.Select(cs => cs.ToApiModel()).ToList()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting courses");
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetCourseSyllabus")]
    [ProducesResponseType(typeof(GetCourseSyllabusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(String), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourseSyllabus([Required] [FromQuery] GetCourseSyllabusRequest req)
    {
        try
        {
            Logger.LogInformation("GetCourseSyllabus called with CourseCode: {CourseCode}", req.CourseCode);
            
            CourseSyllabus? courseSyllabus = await ShsdpDbContext.CourseSyllabi
                .FirstOrDefaultAsync(cs => cs.CourseCode.Equals(req.CourseCode));

            if (courseSyllabus == null)
            {
                Logger.LogWarning("Course syllabus not found: {CourseCode}", req.CourseCode);
                return NotFound("Course syllabus not found");
            }
            
            Logger.LogInformation("Course syllabus found: {CourseCode}", req.CourseCode);
            return Ok(new GetCourseSyllabusResponse(
                CourseSyllabus: courseSyllabus.ToApiModel()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting course syllabus: {CourseCode}", req.CourseCode);
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetCourseSyllabi")]
    [ProducesResponseType(typeof(GetCourseSyllabiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourseSyllabi([FromQuery] GetCourseSyllabiRequest req)
    {
        try
        {
            Logger.LogInformation("GetCourseSyllabi called");
            
            List<CourseSyllabus> courseSyllabi = await ShsdpDbContext.CourseSyllabi.ToListAsync();
            
            Logger.LogInformation("Retrieved {Count} course syllabi", courseSyllabi.Count);
            return Ok(new GetCourseSyllabiResponse(
                CourseSyllabi: courseSyllabi.Select(cs => cs.ToApiModel()).ToList()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting course syllabi");
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAssignmentRuntime")]
    [ProducesResponseType(typeof(GetAssignmentRuntimeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAssignmentRuntime([Required] [FromQuery] GetAssignmentRuntimeRequest req)
    {
        try
        {
            Logger.LogInformation("GetAssignmentRuntime called with AssignId: {AssignId}", req.AssignId);
            
            AssignmentRuntime? assignmentRuntime = await ShsdpDbContext.AssignmentRuntimes
                .FirstOrDefaultAsync(ar => ar.AssignId.Equals(req.AssignId));

            if (assignmentRuntime == null)
            {
                Logger.LogWarning("Assignment runtime not found: {AssignId}", req.AssignId);
                return NotFound("Assignment runtime not found");
            }
            
            Logger.LogInformation("Assignment runtime found: {AssignId}", req.AssignId);
            return Ok(new GetAssignmentRuntimeResponse(
                AssignmentRuntime: assignmentRuntime.ToApiModel()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting assignment runtime: {AssignId}", req.AssignId);
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAssignmentRuntimeByCourse")]
    [ProducesResponseType(typeof(GetAssignmentRuntimeByCourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(String), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAssignmentRuntimeByCourse([Required] [FromQuery] GetAssignmentRuntimeByCourseRequest req)
    {
        try
        {
            Logger.LogInformation("GetAssignmentRuntimeByCourse called with CourseCode: {CourseCode}", req.CourseCode);
            
            List<AssignmentRuntime> assignmentRuntimes = await ShsdpDbContext.AssignmentRuntimes
                .Where(ar => ar.CourseCode.Equals(req.CourseCode))
                .ToListAsync();
            
            if (assignmentRuntimes.Count == 0)
            {
                Logger.LogWarning("No assignment runtimes found for course: {CourseCode}", req.CourseCode);
                return NotFound("No assignment runtimes found for the specified course");
            }
            
            Logger.LogInformation("Retrieved {Count} assignment runtimes for course: {CourseCode}", 
                assignmentRuntimes.Count, req.CourseCode);
            return Ok(new GetAssignmentRuntimeByCourseResponse(
                AssignmentRuntimes: assignmentRuntimes.Select(ar => ar.ToApiModel()).ToList()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting assignment runtimes by course: {CourseCode}", req.CourseCode);
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetCourseConfiguration")]
    [ProducesResponseType(typeof(GetCourseConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(String), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourseConfiguration([Required] [FromQuery] GetCourseConfigurationRequest req)
    {
        try
        {
            Logger.LogInformation("GetCourseConfiguration called with CourseCode: {CourseCode}", req.CourseCode);
            
            // Fetch all three entities
            Course? course = await ShsdpDbContext.Courses
                .FirstOrDefaultAsync(c => c.CourseCode.Equals(req.CourseCode));
            CourseSyllabus? syllabus = await ShsdpDbContext.CourseSyllabi
                .FirstOrDefaultAsync(cs => cs.CourseCode.Equals(req.CourseCode));
            List<AssignmentRuntime> runtimes = await ShsdpDbContext.AssignmentRuntimes
                .Where(ar => ar.CourseCode.Equals(req.CourseCode))
                .ToListAsync();

            if (course == null)
            {
                Logger.LogWarning("Course not found: {CourseCode}", req.CourseCode);
                return NotFound("Course not found");
            }

            if (syllabus == null)
            {
                Logger.LogWarning("Course syllabus not found: {CourseCode}", req.CourseCode);
                return NotFound("Course syllabus not found");
            }
            
            Logger.LogInformation("Course configuration retrieved for: {CourseCode} with {Count} assignment runtimes", 
                req.CourseCode, runtimes.Count);
            
            return Ok(new GetCourseConfigurationResponse(
                Course: course.ToApiModel(),
                Syllabus: syllabus.ToApiModel(),
                AssignmentRuntime: runtimes.Select(ar => ar.ToApiModel()).ToList()
            ));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting course configuration: {CourseCode}", req.CourseCode);
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAllCourseConfigurations")]
    [ProducesResponseType(typeof(GetAllCourseConfigurationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCourseConfigurations([FromQuery] GetAllCourseConfigurationsRequest req)
    {
        try
        {
            Logger.LogInformation("GetAllCourseConfigurations called");
            
            // Fetch all entities in parallel
            List<Course> courses = await ShsdpDbContext.Courses.ToListAsync();
            List<CourseSyllabus> syllabi = await ShsdpDbContext.CourseSyllabi.ToListAsync();
            List<AssignmentRuntime> runtimes = await ShsdpDbContext.AssignmentRuntimes.ToListAsync();
            
            // Group assignment runtimes by course code
            var runtimesByCourse = runtimes
                .GroupBy(ar => ar.CourseCode)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build configurations by joining courses with their syllabi and runtimes
            List<GetCourseConfigurationResponse> configurations = courses
                .Select(course =>
                {
                    CourseSyllabus? syllabus = syllabi.FirstOrDefault(s => s.CourseCode.Equals(course.CourseCode));
                    List<AssignmentRuntime> courseRuntimes = runtimesByCourse
                        .TryGetValue(course.CourseCode, out List<AssignmentRuntime>? rts)
                        ? rts
                        : [];

                    return new GetCourseConfigurationResponse(
                        Course: course.ToApiModel(),
                        Syllabus: syllabus?.ToApiModel() ?? new SHSDP.Shared.Models.API.CourseSyllabus(
                            CourseCode: course.CourseCode,
                            SyllabusFilePath: "",
                            SyllabusYear: 0,
                            Disabled: true
                        ),
                        AssignmentRuntime: courseRuntimes.Select(ar => ar.ToApiModel()).ToList()
                    );
                })
                .ToList();
            
            Logger.LogInformation("Retrieved {Count} course configurations", configurations.Count);
            return Ok(new GetAllCourseConfigurationsResponse(Configurations: configurations));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting all course configurations");
#if DEBUG
            Console.Error.WriteLine(ex);
            Debugger.Break();
#endif
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}