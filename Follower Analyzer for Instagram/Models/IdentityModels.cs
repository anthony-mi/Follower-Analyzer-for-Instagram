using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Follower_Analyzer_for_Instagram.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // Поле для ID пользователя в Instagram (так как остальные данные могут изменяться, целесообразным посчитали хранить именно ID)
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

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class FollowerAnalyzerDbContext : IdentityDbContext<ApplicationUser>
    {
        public FollowerAnalyzerDbContext()
            : base("FollowerAnalyzerConnection", throwIfV1Schema: false)
        {
        }

        public static FollowerAnalyzerDbContext Create()
        {
            return new FollowerAnalyzerDbContext();
        }
    }
}