using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.InstagramAPI
{
    public interface IInstagramAPI
    {
        string GetCurrentUserPrimaryKey();
        List<User> GetUserFollowersByUsername(string username);
        Task<List<User>> GetUserFollowersByUsernameAsync(string username);
        List<User> GetUserSubscriptionsByUsername(string username);
        Task<List<User>> GetUserSubscriptionsByUsernameAsync(string username);
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
