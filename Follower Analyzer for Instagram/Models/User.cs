using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class User
    {
        [Key]
        public string InstagramPK { get; set; }
        public string Username { get; set; }
        // Здесь мы храним инстаграм кукис
        public byte[] StateData { get; set; }
        // Список подписчиков пользователя
        public virtual List<User> Followers { get; set; }
        // Количество подписчиков пользователя (думаю, что это поле можно и убрать)
        public int FollowersCount { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public User()
        {
            Followers = new List<User>();
        }
    }
}