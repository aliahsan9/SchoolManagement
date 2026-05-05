namespace SchoolManagement.Application.Common.Authorization;

public static class PolicyNames
{
    public const string AdminOnly = "AdminOnly";
    public const string TeacherOnly = "TeacherOnly";
    public const string StudentOnly = "StudentOnly";
    public const string AdminOrTeacher = "AdminOrTeacher";
}
