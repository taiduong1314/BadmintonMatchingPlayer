using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class PostServices : IPostServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public PostServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public int CreatePost(int user_id, NewPostInfo info)
        {
            DateTime startTime = DateTime.UtcNow;
            var startH = string.Empty;
            if (info.StartTime != null)
            {
                foreach (var item in info.StartTime.Split(":"))
                {
                    if (startH == string.Empty)
                    {
                        startH = item.Replace(":", "");
                    }
                    else
                    {
                        startTime = new DateTime(info.Year, info.Month, info.Day, int.Parse(startH), int.Parse(item), 0);
                    }
                }
            }
            DateTime endTime = DateTime.UtcNow;
            var endH = string.Empty;
            if (info.EndTime != null)
            {
                foreach (var item in info.EndTime.Split(":"))
                {
                    if (endH == string.Empty)
                    {
                        endH = item.Replace(":", "");
                    }
                    else
                    {
                        endTime = new DateTime(info.Year, info.Month, info.Day, int.Parse(startH), int.Parse(item), 0);
                    }
                }
            }
            var newPost = new Post
            {
                Title = info.Title,
                AddressSlot = info.Address,
                TimeStart = startTime,
                TimeEnd = endTime,
                PriceSlot = decimal.Parse(info.Price),
                QuantitySlot = info.AvailableSlot,
                ContentPost = info.Description
            };
            _repositoryManager.Post.Create(newPost);
            _repositoryManager.SaveAsync().Wait();
            return newPost.Id;
        }

        public List<string?> GetAllPlayGround()
        {
            var res = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                    && x.QuantitySlot > 0
                    && x.TimeStart > DateTime.UtcNow.AddHours(12), true)
                .Select(x => x.AddressSlot).ToList();
            return res;
        }

        public List<PostInfomation> GetPostByPlayGround(string play_ground)
        {
            var res = new List<PostInfomation>();
            res = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                && x.AddressSlot == play_ground
                && x.QuantitySlot > 0
                && x.TimeStart > DateTime.UtcNow.AddHours(12)
                    , true)
                .Include(x => x.IdUserToNavigation)
                .Select(x => new PostInfomation
                {
                    Address = x.AddressSlot,
                    AvailableSlot = x.QuantitySlot,
                    PostId = x.Id,
                    PostImgUrl = x.ImgUrl,
                    SortDescript = x.ContentPost,
                    Time = $"{x.TimeStart} - {x.TimeEnd}",
                    UserId = x.IdUserTo,
                    UserImgUrl = x.IdUserToNavigation.ImgUrl,
                    UserName = x.IdUserToNavigation.UserName
                }).ToList();
            return res;
        }

        public List<PostInfomation> GetSuggestionPost(int user_id)
        {
            var res = new List<PostInfomation>();
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, true).FirstOrDefault();
            if (user != null && user.PlayingArea != null)
            {
                res = _repositoryManager.Post.FindByCondition(
                    x => x.AddressSlot != null
                    && user.PlayingArea.Contains(x.AddressSlot)
                    && x.QuantitySlot > 0
                    && x.TimeStart > DateTime.UtcNow.AddHours(12)
                        , true)
                    .Include(x => x.IdUserToNavigation)
                    .Select(x => new PostInfomation
                    {
                        Address = x.AddressSlot,
                        AvailableSlot = x.QuantitySlot,
                        PostId = x.Id,
                        PostImgUrl = x.ImgUrl,
                        SortDescript = x.ContentPost,
                        Time = $"{x.TimeStart} - {x.TimeEnd}",
                        UserId = x.IdUserTo,
                        UserImgUrl = x.IdUserToNavigation.ImgUrl,
                        UserName = x.IdUserToNavigation.UserName
                    }).ToList();
            }
            return res;
        }
    }
}
