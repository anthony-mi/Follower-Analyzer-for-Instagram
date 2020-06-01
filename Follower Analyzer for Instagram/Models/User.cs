namespace Follower_Analyzer_for_Instagram.Models
{
    public class User
    {
        public string InstagramPK { get; set; }
        public string Username { get; set; }

        public static bool operator ==(User obj1, User obj2)
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

        public static bool operator !=(User obj1, User obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(User other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return InstagramPK == other.InstagramPK;
        }
    }
}