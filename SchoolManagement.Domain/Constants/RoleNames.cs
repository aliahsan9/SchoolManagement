namespace SchoolManagement.Domain.Constants;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";

    public static readonly string[] All = [Admin, Teacher, Student, Parent];
}
