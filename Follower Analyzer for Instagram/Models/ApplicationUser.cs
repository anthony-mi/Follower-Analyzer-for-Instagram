using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ApplicationUser : User
    {
        public int Id { get; set; }
        // Здесь мы храним инстаграм кукис
        public byte[] StateData { get; set; }
        // Список подписчиков пользователя
        public List<ApplicationUser> Subscriptions { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public ApplicationUser()
        {
            Subscriptions = new List<ApplicationUser>();
        }
    }
}