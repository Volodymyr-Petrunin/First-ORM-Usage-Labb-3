using SqlLabbThree.Data;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Services;

public class GroupService(SchoolContext context)
{
    public List<Group> GetAllGroups()
    {
        return context.Groups.ToList();
    }
}