using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Services.ActivityAnalizer;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin;
using Owin;
using System.Threading;

[assembly: OwinStartupAttribute(typeof(Follower_Analyzer_for_Instagram.Startup))]
namespace Follower_Analyzer_for_Instagram
{
    public partial class Startup
    {
        public static CancellationTokenSource ActivityAnalizingCancellationTokenSource { get; set; }
        public static IActivityAnalizer @ActivityAnalizer { get; set; }

        public void Configuration(IAppBuilder app)
        {
            try
            {
                ActivityAnalizingCancellationTokenSource = new CancellationTokenSource();
                ActivityAnalizer = new ActivityAnalizer(new FollowerAnalyzerRepository());
                ActivityAnalizer.StartAnalizing(ActivityAnalizingCancellationTokenSource);
            }
            catch { }
        }
    }
}
