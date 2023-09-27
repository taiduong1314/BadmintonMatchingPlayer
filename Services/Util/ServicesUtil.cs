using Entities.Models;

namespace Services.Util
{
    public class ServicesUtil
    {
        public static bool IsInTimePost(Post post)
        {
            foreach(var item in GetDaysOfPost(post))
            {
                if(item > DateTime.UtcNow.AddHours(2))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<DateTime> GetDaysOfPost(Post post)
        {
            var res = new List<DateTime>();
            var month = GetMonthOfPost(post);
            var year = GetYearOfPost(post);
            var startH = 0;
            var startM = 0;
            if (post.StartTime != null)
            {
                startH = GetHours(post.StartTime);
                startM = GetMinute(post.StartTime);
            }
            if (post != null && post.Days != null)
            {
                foreach (var item in post.Days.Split(":"))
                {
                    if (item.Contains(";"))
                    {
                        foreach (var item2 in item.Split(";"))
                        {
                            res.Add(new DateTime(year, month, int.Parse(item2.Replace(";", "")), startH, startM, 0));
                        }
                        break;
                    }
                }
            }
            return res;
        }

        public static int GetMonthOfPost(Post post)
        {
            var res = 0;
            if (post != null && post.Days != null)
            {
                var index = 1;
                foreach (var item in post.Days.Split(":"))
                {
                    if (index == 2)
                    {
                        res = int.Parse(item.Replace(":", ""));
                        break;
                    }
                    index++;
                }
            }
            return res;
        }

        public static int GetYearOfPost(Post post)
        {
            var res = 0;
            if (post != null && post.Days != null)
            {
                var index = 1;
                foreach (var item in post.Days.Split(":"))
                {
                    if (index == 3)
                    {
                        res = int.Parse(item.Replace(":", ""));
                        break;
                    }
                    index++;
                }
            }
            return res;
        }

        public static int GetMinute(string time)
        {
            return int.Parse(time.Substring(time.IndexOf(":") + 1));
        }

        public static int GetHours(string time)
        {
            return int.Parse(time.Remove(time.IndexOf(":")));
        }
    }
}
