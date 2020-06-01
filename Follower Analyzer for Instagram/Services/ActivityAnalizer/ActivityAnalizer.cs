using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    public class ActivityAnalizer : IActivityAnalizer, IDisposable
    {
        private IRepository _repository;
        private List<AnalizationTask> _tasks; 

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
            await _repository.CreateAsync(activity);
        }

        public void Dispose()
        {
            foreach(var analizationTask in _tasks)
            {
                analizationTask.CancellationTokenSource.Cancel();

                foreach (var task in analizationTask.Tasks)
                {
                    task.Wait(TASK_WAITING_TIMEOUT);
                }
            }
        }

        public void StartAnalizing(CancellationTokenSource cancellationTokenSource)
        {
            _tasks = new List<AnalizationTask>();

            Task<IQueryable<ApplicationUser>> task = Task.Run(
                () => _repository.GetListAsync<ApplicationUser>());

            task.Wait();

            List<ApplicationUser> observers = task.Result.ToList();

            foreach (var observer in observers)
            {
                foreach(var observable in observer.ObservableAccounts)
                {
                    StartLikesAnalizingAsync(observer, observable);
                    StartCommentsAnalizingAsync(observer, observable);
                }
            }
        }

        private void TryAddToTasks(
            ApplicationUser observer, 
            ObservableUser observable,
            User targetUser,
            Task task,
            CancellationTokenSource cancellationTokenSource)
        {
            var analizationTask = _tasks.Where(at => at.Observer.Equals(observer) &&
                at.Observable.Equals(observable) &&
                at.TargetUser.Equals(targetUser)).FirstOrDefault();

            if(analizationTask == null) // Task not created yet.
            {
                var newAnalizationTask = 
                    new AnalizationTask(observer, observable, targetUser, task, cancellationTokenSource);
                _tasks.Add(newAnalizationTask);
            }

            analizationTask.AddTask(task);
        }

        private static List<User> CreateKey(
            ApplicationUser observer,
            ObservableUser observable,
            ObservableUser targetUser)
        {
            return new List<User> { observer, observable, targetUser };
        }

        private async Task StartCommentsAnalizingAsync(
            ApplicationUser observer, ObservableUser observable)
        {
            Parallel.ForEach(observable.ObservableUsers, targetUser =>
            {
                StartCommentsAnalizingAsync(observer, observable, targetUser);
            });
        }

        private async Task StartCommentsAnalizingAsync(
            ApplicationUser observer, ObservableUser observable, User targetUser)
        {
            IInstagramAPI instagramAPI = new InstagramAPI.InstagramAPI();
            instagramAPI.SetCookies(observer.StateData);

            List<InstagramPost> posts =
                    instagramAPI.GetUserPostsByPrimaryKey(targetUser.InstagramPK);

            var cancellationTokenSource = new CancellationTokenSource();

            Task commentsAnalizingTask = Task.Run(
            () =>
            {
                do
                {
                    List<InstagramPost> newPostsState =
                    instagramAPI.GetUserPostsByPrimaryKey(targetUser.InstagramPK);

                    if (AreDifferencesPresent(posts, newPostsState))
                    {
                        var postsWithDifference = GetDistinctivePosts(posts, newPostsState);

                        foreach (var distinctivePost in postsWithDifference)
                        {
                            CheckForUserComment(observer, observable, targetUser, distinctivePost.Key, distinctivePost.Value);
                        }
                    }

                    posts = newPostsState;

                    try
                    {
                        Sleep(SLEEP_TIMEOUT, cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        break;
                    }
                } while (!cancellationTokenSource.Token.IsCancellationRequested);
            });

            TryAddToTasks(observer, observable, targetUser, commentsAnalizingTask, cancellationTokenSource);
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

        private void CheckForUserComment(ApplicationUser observer, ObservableUser observable, User postOwner, InstagramPost firstPostState, InstagramPost secondPostState)
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

        private void CheckForUserLike(ApplicationUser observer, ObservableUser observable, User postOwner, InstagramPost firstPostState, InstagramPost secondPostState)
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

        private Dictionary<InstagramPost, InstagramPost> GetDistinctivePosts(
            List<InstagramPost> posts, List<InstagramPost> newPostsState)
        {
            var distinctivePosts = new Dictionary<InstagramPost, InstagramPost>();

            do
            {
                if (posts.Count != newPostsState.Count)
                {
                    var newPosts = newPostsState.Except(posts).ToList();

                    foreach(var newPost in newPosts)
                    {
                        distinctivePosts.Add(new InstagramPost(), newPost);
                    }
                }

                for (int i = 0; i < posts.Count; i++)
                {
                    if (posts[i].CountOfLikes != newPostsState[i].CountOfLikes)
                    {
                        distinctivePosts.Add(posts[i], newPostsState[i]);
                        continue;
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

        private async Task StartLikesAnalizingAsync(
            ApplicationUser observer, ObservableUser observable)
        {
            foreach(var targetUser in observable.ObservableUsers)
            {
                StartLikesAnalizingAsync(observer, observable, targetUser);
            }
        }

        private async Task StartLikesAnalizingAsync(
            ApplicationUser observer, ObservableUser observable, User targetUser)
        {
            IInstagramAPI instagramAPI = new InstagramAPI.InstagramAPI();
            instagramAPI.SetCookies(observer.StateData);

            List<InstagramPost> posts =
                    instagramAPI.GetUserPostsByPrimaryKey(targetUser.InstagramPK);

            var cancellationTokenSource = new CancellationTokenSource();

            Task likesAnalizingTask = Task.Run(
            () =>
            {
                do
                {
                    List<InstagramPost> newPostsState =
                    instagramAPI.GetUserPostsByPrimaryKey(targetUser.InstagramPK);

                    if (AreDifferencesPresent(posts, newPostsState))
                    {
                        var postsWithDifference = GetDistinctivePosts(posts, newPostsState);

                        foreach (var distinctivePost in postsWithDifference)
                        {
                            CheckForUserLike(observer, observable, targetUser, distinctivePost.Key, distinctivePost.Value);
                        }
                    }

                    posts = newPostsState;

                    try
                    {
                        Sleep(SLEEP_TIMEOUT, cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        break;
                    }
                } while (!cancellationTokenSource.Token.IsCancellationRequested);
            });

            TryAddToTasks(observer, observable, targetUser, likesAnalizingTask, cancellationTokenSource);
        }


        public async Task RemoveUserFromObservationAsync(ApplicationUser observer, ObservableUser observable)
        {
            var observableTasks = _tasks.Where(at => at.Observer.Equals(observer) &&
                at.Observable.Equals(observable)).ToList();

            if(observableTasks.Count == 0) // User is not under observation yet.
            {
                return; 
            }

            foreach(var aTask in observableTasks)
            {
                aTask.CancellationTokenSource.Cancel();
            }

            foreach (var aTask in observableTasks)
            {
                foreach(var task in aTask.Tasks)
                {
                    task.Wait();
                }
            }

            _tasks = _tasks.Except(observableTasks).ToList();
        }

        public async Task RemoveTargetUserFromObservationAsync(ApplicationUser observer, ObservableUser observable, User targetUser)
        {
            var aTask = _tasks.Where(at => at.Observer.Equals(observer) &&
                at.Observable.Equals(observable) &&
                at.TargetUser.Equals(targetUser)).FirstOrDefault();

            if (aTask == null) // User is not under observation.
            {
                return;
            }

            aTask.CancellationTokenSource.Cancel();

            foreach (var task in aTask.Tasks)
            {
                task.Wait();
            }

            _tasks.Remove(aTask);
        }

        public async Task StartObservationAsync(ApplicationUser observer, ObservableUser observable, ObservableUser targetUser)
        {
            StartLikesAnalizingAsync(observer, observable, targetUser);
            StartCommentsAnalizingAsync(observer, observable, targetUser);
        }
    }
}