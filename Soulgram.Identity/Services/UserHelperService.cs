using soulgram.identity.Models;

namespace soulgram.identity.Services;

public static class UserHelperService
{
    public static void CheckThenUpdate(
        this ApplicationUser userToUpdate,
        ApplicationUser dataToUpdate)
    {
        userToUpdate.ProfileImg = dataToUpdate.ProfileImg ?? userToUpdate.ProfileImg;
        userToUpdate.Hobbies = dataToUpdate.Hobbies ?? userToUpdate.Hobbies;
        userToUpdate.Fullname = dataToUpdate.Fullname ?? userToUpdate.Fullname;
    }
}