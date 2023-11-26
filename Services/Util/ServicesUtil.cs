﻿using Entities.Models;
using Entities.RequestObject;
using Repositories.Intefaces;

namespace Services.Util
{
    public class ServicesUtil
    {
        public static bool IsInTimePost(Post post, IRepositoryManager repositoryManager)
        {
            if(post.SlotsInfo == null)
                return false;

            foreach(var item in post.SlotsInfo.Split(";"))
            {
                var info = new SlotInfo(item);
                if(info.StartTime >= DateTime.UtcNow.AddHours(2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
