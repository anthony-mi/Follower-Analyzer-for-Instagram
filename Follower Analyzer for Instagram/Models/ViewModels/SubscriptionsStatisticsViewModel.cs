using System.Collections.Generic;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class SubscriptionsStatisticsViewModel
    {
        public List<User> NewSubscriptions { get; set; }
        public List<User> UnsubscribedSubscriptions { get; set; }

        public SubscriptionsStatisticsViewModel()
        {
            NewSubscriptions = new List<User>();
            UnsubscribedSubscriptions = new List<User>();
        }
    }
}