using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.API.Processors;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.XPath;

namespace Follower_Analyzer_for_Instagram.Services.InstagramAPI
{
    public class InstagramAPI : IInstagramAPI
    {
        private IInstaApi _instaApi = null;

        private readonly int REQUEST_DELAY_MIN = 2;
        private readonly int REQUEST_DELAY_MAX = 20;

        public bool TryAuthenticate(string username, string password, out byte[] instagramUserCookies)
        {
            bool isAuthenticated = false;

            _instaApi = CreateInstaApi(username, password, REQUEST_DELAY_MIN, REQUEST_DELAY_MAX);

            IResult<InstaLoginResult> logInResult = TryLogIn(_instaApi);

            instagramUserCookies = new byte[] { };

            if (!logInResult.Succeeded)
            {
                isAuthenticated = false;
            }
            else
            {
                instagramUserCookies = ConvertStreamToByteArray(_instaApi.GetStateDataAsStream());
                isAuthenticated = true;
            }

            return isAuthenticated;
        }

        public string GetCurrentUserPrimaryKey()
        {
            string primaryKey = string.Empty;

            Task<IResult<InstaCurrentUser>> currentUserTask = Task.Run(() => _instaApi.GetCurrentUserAsync());
            currentUserTask.Wait();
            IResult<InstaCurrentUser> currentUser = currentUserTask.Result;

            if (currentUser.Succeeded)
            {
                primaryKey = currentUser.Value.Pk.ToString();
            }

            return primaryKey;
        }

        private static IInstaApi CreateInstaApi(string username, string password, int requestDelayMin, int requestDelayMax)
        {
            UserSessionData userSession = new UserSessionData
            {
                UserName = username,
                Password = password
            };

            // if you want to set custom device (user-agent) please check this:
            // https://github.com/ramtinak/InstagramApiSharp/wiki/Set-custom-device(user-agent)

            IRequestDelay delay = RequestDelay.FromSeconds(requestDelayMin, requestDelayMax);

            return InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .SetRequestDelay(delay)
                .Build();
        }

        private static IInstaApi CreateInstaApi(byte[] cookies, int requestDelayMin, int requestDelayMax)
        {
            IRequestDelay delay = RequestDelay.FromSeconds(requestDelayMin, requestDelayMax);

            var instaAPI = InstaApiBuilder.CreateBuilder()
                .SetRequestDelay(delay)
                .Build();

            instaAPI.LoadStateDataFromStream(new MemoryStream(cookies));

            return instaAPI;
        }

        private static byte[] ConvertStreamToByteArray(Stream stream)
        {
            byte[] result = new byte[] { };

            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                result = binaryReader.ReadBytes((int)stream.Length);
            }

            return result;
        }

        private static IResult<InstaLoginResult> TryLogIn(IInstaApi instaApi)
        {
            Task<IResult<InstaLoginResult>> logInTask = Task.Run(() => instaApi.LoginAsync());
            logInTask.Wait();

            return logInTask.Result;
        }

        public List<InstagramPost> GetUserPostsByUsername(string username, byte[] instagramCookies)
        {
            IInstaApi instaApi = CreateInstaApi(instagramCookies, REQUEST_DELAY_MIN, REQUEST_DELAY_MAX);

            List<InstagramPost> posts = new List<InstagramPost>();

            PaginationParameters pageParams = PaginationParameters.Empty;

            Task<IResult<InstaMediaList>> mediaListTask = Task.Run(() => instaApi.UserProcessor.GetUserMediaAsync("_kris_svat_", pageParams));
            mediaListTask.Wait();
            IResult<InstaMediaList> mediaList = mediaListTask.Result;

            if (mediaList.Succeeded)
            {
                Parallel.ForEach(mediaList.Value, media =>
                {
                    InstagramPost post = new InstagramPost();
                    post.CountOfComments = Convert.ToInt32(media.CommentsCount);
                    post.CountOfLikes = Convert.ToInt32(media.LikesCount);
                    post.MediaFileUri = GetUri(media);
                    posts.Add(post);
                });
            }

            return posts;
        }

        private string GetUri(InstaMedia media)
        {
            string uri = string.Empty;

            switch (media.MediaType)
            {
                case InstaMediaType.Carousel:
                    InstaCarouselItem item = media.Carousel.First();
                    uri = GetUri(item);
                    break;

                case InstaMediaType.Video:
                    uri = media.Videos.First().Uri;
                    break;

                case InstaMediaType.Image:
                    uri = media.Images.First().Uri;
                    break;
            }

            return uri;
        }

        private string GetUri(InstaCarouselItem item)
        {
            string uri = string.Empty;

            switch (item.MediaType)
            {
                case InstaMediaType.Image:
                    uri = item.Images.First().Uri;
                    break;

                case InstaMediaType.Video:
                    uri = item.Videos.First().Uri;
                    break;
            }

            return uri;
        }

        public List<InstagramPost> GetUserPostsByPrimaryKey(string primaryKey, byte[] instagramCookies)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LogoutAsync()
        {
            bool isLoggedOut = false;

            if (_instaApi == null)
            {
                throw new NullReferenceException();
            }

            IResult<bool> logout = await _instaApi.LogoutAsync();
            isLoggedOut = logout.Succeeded;

            return isLoggedOut;
        }

        public void SetCookies(byte[] instagramCookies)
        {
            _instaApi = CreateInstaApi(instagramCookies, REQUEST_DELAY_MIN, REQUEST_DELAY_MAX);
        }

        public List<InstagramPost> GetUserPostsByUsername(string username)
        {
            if (_instaApi == null)
            {
                throw new NullReferenceException();
            }

            List<InstagramPost> posts = new List<InstagramPost>();

            PaginationParameters pageParams = PaginationParameters.Empty;

            Task<IResult<InstaMediaList>> mediaListTask = Task.Run(() => _instaApi.UserProcessor.GetUserMediaAsync("_kris_svat_", pageParams));
            mediaListTask.Wait();
            IResult<InstaMediaList> mediaList = mediaListTask.Result;

            if (mediaList.Succeeded)
            {
                Parallel.ForEach(mediaList.Value, media =>
                {
                    InstagramPost post = new InstagramPost();
                    post.CountOfComments = Convert.ToInt32(media.CommentsCount);
                    post.CountOfLikes = Convert.ToInt32(media.LikesCount);
                    post.MediaFileUri = GetUri(media);
                    posts.Add(post);
                });
            }

            return posts;
        }

        public List<InstagramPost> GetUserPostsByPrimaryKey(string primaryKey)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUserFollowersByUsername(string username)
        {
            if(_instaApi == null)
            {
                throw new NullReferenceException();
            }

            List<User> followersResult = new List<User>();

            PaginationParameters pageParams = PaginationParameters.Empty;

            Task<IResult<InstaUserShortList>> getFollowersTask = Task.Run(
                () => _instaApi.UserProcessor.GetUserFollowersAsync(username, pageParams));
            getFollowersTask.Wait();

            IResult<InstaUserShortList> followers = getFollowersTask.Result;;

            if (followers.Succeeded)
            {
                Parallel.ForEach(followers.Value, (follower) =>
                {
                    User newUser = new User
                    {
                        InstagramPK = follower.Pk.ToString(),
                        Username = follower.UserName
                    };

                    followersResult.Add(newUser);
                });
            }

            return followersResult;
        }

        public async Task<List<User>> GetUserFollowersByUsernameAsync(string username)
        {
            if (_instaApi == null)
            {
                throw new NullReferenceException();
            }

            List<User> followersResult = new List<User>();

            PaginationParameters pageParams = PaginationParameters.Empty;

            IResult<InstaUserShortList> followers = await _instaApi.UserProcessor.GetUserFollowersAsync(username, pageParams);

            if (followers.Succeeded)
            {
                Parallel.ForEach(followers.Value, (follower) =>
                {
                    User newUser = new User
                    {
                        InstagramPK = follower.Pk.ToString(),
                        Username = follower.UserName
                    };

                    followersResult.Add(newUser);
                });
            }

            return followersResult;
        }

        public List<User> GetUserFollowersByPrimaryKey(string primaryKey)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUserFollowersByPrimaryKeyAsync(string primaryKey)
        {
            throw new NotImplementedException();
        }
    }
}