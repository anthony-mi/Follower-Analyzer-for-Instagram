using Follower_Analyzer_for_Instagram.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    public interface IActivityAnalizer
    {
        void StartAnalizing(CancellationTokenSource cancellationTokenSource);
        Task StartObservationAsync(ApplicationUser observer, ObservableUser observable, ObservableUser targetUser);
        Task RemoveUserFromObservationAsync(ApplicationUser observer, ObservableUser observable);
        Task RemoveTargetUserFromObservationAsync(ApplicationUser observer, ObservableUser observable, User targetUser);
    }
}
