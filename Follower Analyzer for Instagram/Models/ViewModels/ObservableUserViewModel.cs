using System.ComponentModel.DataAnnotations;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ObservableUserViewModel
    {
        [Required]
        public string UserName { get; set; }
    }
}