using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    public class ActivityAnalizer : IActivityAnalizer, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private IRepository _repository;
        private Stack<Task> _tasks;

        private event EventHandler<UserActivity> _postIsLiked;
        private event EventHandler<UserActivity> _commentCreated;

        private readonly TimeSpan TASK_WAITING_TIMEOUT = new TimeSpan(5000);
        private readonly TimeSpan SLEEP_TIMEOUT = new TimeSpan(5000);

        public ActivityAnalizer(IRepository repository)
        {
            _repository = repository;

            _postIsLiked += OnActivityFixed;
            _commentCreated += OnActivityFixed;
        }

        private async void OnActivityFixed(object sender, UserActivity activity)
        {
            await _repository.CreateAsync<UserActivity>(activity);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            while(_tasks.Count > 0)
            {
                var task = _tasks.Pop();
                task.Wait(TASK_WAITING_TIMEOUT);
            }
        }

        public void StartAnalizing(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            CancellationToken cancellationToken = new CancellationToken();

            _tasks = new Stack<Task>();

            Task<IQueryable<ApplicationUser>> task = Task.Run(
                () => _repository.GetListAsync<ApplicationUser>());
            task.Wait();
            List<ApplicationUser> observers = task.Result.ToList();

            foreach (var observer in observers)
            {
                foreach(var observable in observer.ObservableAccounts)
                {
                    Task likesAnalizingTask = Task.Run(
                () => StartLikesAnalizing(observer, observable, cancellationToken));

                    Task commentsAnalizingTask = Task.Run(
                    () => StartCommentsAnalizing(observer, observable, cancellationToken));

                    _tasks.Push(likesAnalizingTask);
                    _tasks.Push(commentsAnalizingTask);
                }
            }
        }

        private void StartCommentsAnalizing(
            ApplicationUser observer, ObservableUser observable, CancellationToken cancellationToken)
        {
            IInstagramAPI instagramAPI = new InstagramAPI.InstagramAPI();
            instagramAPI.SetCookies(observer.StateData);

            foreach(var oservableConnectedUser in observable.ObservableUsers)
            {
                List<InstagramPost> posts =
                    instagramAPI.GetUserPostsByPrimaryKey(oservableConnectedUser.InstagramPK);

                Task commentsAnalizingTask = Task.Run(
                () =>
                {
                    do
                    {
                        List<InstagramPost> newPostsState = 
                        instagramAPI.GetUserPostsByPrimaryKey(oservableConnectedUser.InstagramPK);

                        if(AreDifferencesPresent(posts, newPostsState))
                        {
                            var postsWithDifference = GetDistinctivePosts(posts, newPostsState);
                            
                            foreach(var distinctivePost in postsWithDifference)
                            {
                                CheckForUserComment(observer, observable, oservableConnectedUser, distinctivePost.Key, distinctivePost.Value);
                            }
                        }

                        posts = newPostsState;

                        Sleep(SLEEP_TIMEOUT, cancellationToken);
                    } while (!cancellationToken.IsCancellationRequested);
                });

                _tasks.Push(commentsAnalizingTask);
            }
        }

        private void Sleep(TimeSpan timeout, CancellationToken cancellationToken)
        {
            int delay = 500;

            for(int i = 0; i <= timeout.Ticks; i++)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                Thread.Sleep(delay);
            }
        }

        private void CheckForUserComment(ApplicationUser observer, ObservableUser observable, ObservableUser postOwner, InstagramPost firstPostState, InstagramPost secondPostState)
        {
            if(!firstPostState.Commenters.Contains(observable) 
                && secondPostState.Commenters.Contains(observable))
            {
                var activity = new UserActivity();
                activity.ObserverPrimaryKey = observer.InstagramPK;
                activity.InitiatorPrimaryKey = observable.InstagramPK;
                activity.TargetUserPrimaryKey = postOwner.InstagramPK;
                activity.LinkToMedia = firstPostState.MediaFileUri;
                activity.EventDate = DateTime.Now;
                activity.ActivityType = UserActivityType.Comment;
                _commentCreated?.Invoke(null, activity);
            }
        }

        private void CheckForUserLike(ApplicationUser observer, ObservableUser observable, ObservableUser postOwner, InstagramPost firstPostState, InstagramPost secondPostState)
        {
            if (!firstPostState.Likers.Contains(observable)
                && secondPostState.Likers.Contains(observable))
            {
                var activity = new UserActivity();
                activity.ObserverPrimaryKey = observer.InstagramPK;
                activity.InitiatorPrimaryKey = observable.InstagramPK;
                activity.TargetUserPrimaryKey = postOwner.InstagramPK;
                activity.LinkToMedia = firstPostState.MediaFileUri;
                activity.EventDate = DateTime.Now;
                activity.ActivityType = UserActivityType.Like;

                _postIsLiked?.Invoke(null, activity);
            }
        }

        private Dictionary<InstagramPost, InstagramPost> GetDistinctivePosts(List<InstagramPost> posts, List<InstagramPost> newPostsState)
        {
            var distinctivePosts = new Dictionary<InstagramPost, InstagramPost>();

            do
            {
                if (posts.Count != newPostsState.Count)
                {
                    // XXX: implement this
                }

                for (int i = 0; i < posts.Count; i++)
                {
                    if (posts[i].CountOfLikes != newPostsState[i].CountOfLikes)
                    {
                        distinctivePosts.Add(posts[i], newPostsState[i]);                        
                    }

                    if (posts[i].CountOfComments != newPostsState[i].CountOfComments)
                    {
                        distinctivePosts.Add(posts[i], newPostsState[i]);
                    }
                }
            } while (false);

            return distinctivePosts;
        }

        private bool AreDifferencesPresent(List<InstagramPost> posts, List<InstagramPost> newPostsState)
        {
            bool areDifferences = false;

            do
            {
                if(posts.Count != newPostsState.Count)
                {
                    areDifferences = true;
                    break;
                }

                for(int i = 0; i < posts.Count; i++)
                {
                    if(posts[i].CountOfLikes != newPostsState[i].CountOfLikes)
                    {
                        areDifferences = true;
                        break;
                    }

                    if (posts[i].CountOfComments != newPostsState[i].CountOfComments)
                    {
                        areDifferences = true;
                        break;
                    }
                }
            } while (false);

            return areDifferences;
        }

        private void StartLikesAnalizing(
            ApplicationUser observer, ObservableUser observable, CancellationToken cancellationToken)
        {
            IInstagramAPI instagramAPI = new InstagramAPI.InstagramAPI();
            instagramAPI.SetCookies(observer.StateData);

            foreach (var oservableConnectedUser in observable.ObservableUsers)
            {
                List<InstagramPost> posts =
                    instagramAPI.GetUserPostsByPrimaryKey(oservableConnectedUser.InstagramPK);

                Task likesAnalizingTask = Task.Run(
                () =>
                {
                    do
                    {
                        List<InstagramPost> newPostsState =
                        instagramAPI.GetUserPostsByPrimaryKey(oservableConnectedUser.InstagramPK);

                        if (AreDifferencesPresent(posts, newPostsState))
                        {
                            var postsWithDifference = GetDistinctivePosts(posts, newPostsState);

                            foreach (var distinctivePost in postsWithDifference)
                            {
                                CheckForUserLike(observer, observable, oservableConnectedUser, distinctivePost.Key, distinctivePost.Value);
                            }
                        }

                        posts = newPostsState;

                        Sleep(SLEEP_TIMEOUT, cancellationToken);
                    } while (!cancellationToken.IsCancellationRequested);
                });

                _tasks.Push(likesAnalizingTask);
            }
        }

        public void AddUserForObservation(ApplicationUser observer, ObservableUser observable)
        {
            Task likesAnalizingTask = Task.Run(
            () => StartLikesAnalizing(observer, observable, _cancellationTokenSource.Token));

            Task commentsAnalizingTask = Task.Run(
            () => StartCommentsAnalizing(observer, observable, _cancellationTokenSource.Token));

            _tasks.Push(likesAnalizingTask);
            _tasks.Push(commentsAnalizingTask);
        }

        public void RemoveUserFromObservation(ApplicationUser observer, ObservableUser observable)
        {
            throw new NotImplementedException();
        }

        public void AddTargetUserToObservable(ApplicationUser observer, ObservableUser observable, ObservableUser target)
        {
            observable.ObservableUsers = new List<ObservableUser>();
            observable.ObservableUsers.Add(target);

            Task likesAnalizingTask = Task.Run(
            () => StartLikesAnalizing(observer, observable, _cancellationTokenSource.Token));

            Task commentsAnalizingTask = Task.Run(
            () => StartCommentsAnalizing(observer, observable, _cancellationTokenSource.Token));

            _tasks.Push(likesAnalizingTask);
            _tasks.Push(commentsAnalizingTask);
        }
    }
}