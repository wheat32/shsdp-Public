using SHSDP.API.Entities.Courses;

namespace SHSDP.API.Mappers.Courses;

public static class AssignmentRuntimeMapper
{
    public static SHSDP.Shared.Models.API.AssignmentRuntime ToApiModel(this AssignmentRuntime assignmentRuntime)
    {
        return new SHSDP.Shared.Models.API.AssignmentRuntime(
            AssignId: assignmentRuntime.AssignId,
            CourseCode: assignmentRuntime.CourseCode,
            AssignName: assignmentRuntime.AssignName,
            Disabled: assignmentRuntime.AssignDisabled,
            AssignUrl: assignmentRuntime.AssignUrl
        );
    }
    
    public static AssignmentRuntime ToEntityModel(this SHSDP.Shared.Models.API.AssignmentRuntime assignmentRuntime)
    {
        return new AssignmentRuntime
        {
            AssignId = assignmentRuntime.AssignId,
            CourseCode = assignmentRuntime.CourseCode,
            AssignName = assignmentRuntime.AssignName,
            AssignDisabled = assignmentRuntime.Disabled,
            AssignUrl = assignmentRuntime.AssignUrl
        };
    }
}

