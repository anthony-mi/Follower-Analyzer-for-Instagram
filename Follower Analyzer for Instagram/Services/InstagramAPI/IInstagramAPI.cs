using Follower_Analyzer_for_Instagram.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.InstagramAPI
{
    public interface IInstagramAPI
    {
        string GetPrimaryKeyByUsername(string username);
        string GetCurrentUserPrimaryKey();
        List<ApplicationUser> GetUserFollowersByUsername(string username);
        Task<List<ApplicationUser>> GetUserFollowersByUsernameAsync(string username);
        List<ApplicationUser> GetUserSubscriptionsByUsername(string username);
        Task<List<ApplicationUser>> GetUserSubscriptionsByUsernameAsync(string username);
        List<InstagramPost> GetUserPostsByUsername(string username);
        List<InstagramPost> GetUserPostsByUsername(string username, byte[] instagramCookies);
        List<InstagramPost> GetUserPostsByPrimaryKey(string primaryKey);
        Task<List<InstagramPost>> GetUserPostsByPrimaryKeyAsync(string primaryKey);
        string GetUserProfilePictureUriByPrimaryKey(string primaryKey);
        Task<string> GetUserProfilePictureUriByPrimaryKeyAsync(string primaryKey);
        Task<bool> LogoutAsync();
        void SetCookies(byte[] instagramCookies);
        bool TryAuthenticate(string username, string password, out byte[] instagramUserCookies);
    }
}
