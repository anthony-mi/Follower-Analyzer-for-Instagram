using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
{
    public class SubscriptionsStatisticsViewModel
    {
        public List<User> NewSubscriptions { get; set; }
        public List<User> UnsubscribedSubscriptions { get; set; }
    }
}