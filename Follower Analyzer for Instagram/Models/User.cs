using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class User
    {
        public string InstagramPK { get; set; }
        // Здесь мы храним инстаграм кукис
        public byte[] StateData { get; set; }
        // Список подписчиков пользователя
        public List<ApplicationUser> Followers { get; set; }
        // Количество подписчиков пользователя (думаю, что это поле можно и убрать)
        public int FollowersCount { get; set; }

        // Конструктор по умолчанию
        public ApplicationUser()
        {
            Followers = new List<ApplicationUser>();
            SetFollowersCount();
        }
        // Установка значения поля FollowersCount 
        public void SetFollowersCount()
        {
            FollowersCount = Followers.Count;
        }
    }
}