using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;

namespace Follower_Analyzer_for_Instagram.Services.InstagramAPI
{
    public class InstagramAPI : IInstagramAPI
    {
        private IInstaApi _instaApi = null;

        public bool TryAuthenticate(string username, string password, out byte[] instagramUserCookies)
        {
            bool isAuthenticated = false;

            _instaApi = CreateInstaApi(username, password, 2, 20);

            //const string stateFile = "state.bin";
            //try
            //{
            //    if (File.Exists(stateFile))
            //    {
            //        Console.WriteLine("Loading state from file");
            //        using (var fs = File.OpenRead(stateFile))
            //        {
            //            InstaApi.LoadStateDataFromStream(fs);
            //            // in .net core or uwp apps don't use LoadStateDataFromStream
            //            // use this one:
            //            // _instaApi.LoadStateDataFromString(new StreamReader(fs).ReadToEnd());
            //            // you should pass json string as parameter to this function.
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

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
            //delay.Disable();
            Task<IResult<InstaLoginResult>> logInTask = Task.Run(() => instaApi.LoginAsync());
            logInTask.Wait();
            //delay.Enable();

            return logInTask.Result;
        }
    }
}