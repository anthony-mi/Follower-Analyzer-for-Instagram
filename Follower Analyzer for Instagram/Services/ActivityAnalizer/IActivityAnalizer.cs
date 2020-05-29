using Follower_Analyzer_for_Instagram.Models;
using System.Threading;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    public interface IActivityAnalizer
    {
        void StartAnalizing(CancellationTokenSource cancellationTokenSource);
        void AddUserForObservation(ApplicationUser observer, ObservableUser observable);
        void AddTargetUserToObservable(ApplicationUser observer, ObservableUser observable, ObservableUser target);
        void RemoveUserFromObservation(ApplicationUser observer, ObservableUser observable);
    }
}
