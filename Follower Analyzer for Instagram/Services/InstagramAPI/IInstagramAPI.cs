using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.InstagramAPI
{
    public interface IInstagramAPI
    {
        bool TryAuthenticate(string username, string password, out byte[] instagramUserCookies);
        string GetCurrentUserPrimaryKey();
    }
}
