using Follower_Analyzer_for_Instagram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    public class AnalizationTask
    {
        #region Constructors
        public AnalizationTask(
            ApplicationUser observer,
            ObservableUser observable,
            ObservableUser targetUser)
        {
            Observer = observer;
            Observable = observable;
            TargetUser = targetUser;
            Tasks = new List<Task>();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public AnalizationTask(
            ApplicationUser observer,
            ObservableUser observable,
            ObservableUser targetUser,
            Task task,
            CancellationTokenSource cancellationTokenSource)
        {
            Observer = observer;
            Observable = observable;
            TargetUser = targetUser;
            Tasks = new List<Task> { task  };
            CancellationTokenSource = cancellationTokenSource;
        }

        public AnalizationTask(
            ApplicationUser observer,
            ObservableUser observable,
            ObservableUser targetUser,
            IEnumerable<Task> tasks,
            CancellationTokenSource cancellationTokenSource)
        {
            Observer = observer;
            Observable = observable;
            TargetUser = targetUser;
            Tasks = new List<Task>(tasks);
            CancellationTokenSource = cancellationTokenSource;
        }
        #endregion

        public ApplicationUser Observer { get; private set; }
        public ObservableUser Observable { get; private set; }
        public ObservableUser TargetUser { get; private set; }
        public List<Task> Tasks { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }

        #region Operators
        public static bool operator ==(AnalizationTask obj1, AnalizationTask obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }

            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(AnalizationTask obj1, AnalizationTask obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(AnalizationTask other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (Observer.Equals(other.Observer) &&
                Observable.Equals(other.Observable) &&
                TargetUser.Equals(other.TargetUser));
        }
        #endregion
    }
}