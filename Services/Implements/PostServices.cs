using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using Services.Util;

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
            var newPost = new Post
            {
                Title = info.Title,
                AddressSlot = info.Address,
                Days = $"{info.Day}:{info.Month}:{info.Year}",
                StartTime = info.StartTime,
                EndTime = info.EndTime,
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
            var posts = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                    && x.QuantitySlot > 0
                    , true)
                .ToList();

            var res = new List<string?>();
            foreach (var item in posts)
            {
                if (ServicesUtil.IsInTimePost(item))
                {
                    res.Add(item.AddressSlot);
                }
            }

            return res;
        }

        public List<PostInfomation> GetPostByPlayGround(string play_ground)
        {
            var targetTime = DateTime.UtcNow.AddHours(2);
            var res = new List<PostInfomation>();
            var posts = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                && x.AddressSlot == play_ground
                && x.QuantitySlot > 0
                , true)
                .Include(x => x.IdUserToNavigation).ToList();

            foreach (var item in posts)
            {
                if (ServicesUtil.IsInTimePost(item))
                {
                    res.Add(new PostInfomation
                    {
                        Address = item.AddressSlot,
                        AvailableSlot = item.QuantitySlot,
                        PostId = item.Id,
                        PostImgUrl = item.ImgUrl,
                        SortDescript = item.ContentPost,
                        Time = $"{item.StartTime} - {item.EndTime}",
                        UserId = item.IdUserTo,
                        UserImgUrl = item.IdUserToNavigation.ImgUrl,
                        UserName = item.IdUserToNavigation.UserName
                    });
                }
            }

            return res;
        }

        public List<PostInfomation> GetSuggestionPost(int user_id)
        {
            var res = new List<PostInfomation>();
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, true).FirstOrDefault();
            if (user != null && user.PlayingArea != null)
            {
                var posts = _repositoryManager.Post.FindByCondition(
                    x => x.AddressSlot != null
                    && user.PlayingArea.Contains(x.AddressSlot)
                    && x.QuantitySlot > 0
                    && ServicesUtil.IsInTimePost(x)
                        , true)
                    .Include(x => x.IdUserToNavigation)
                    .ToList();

                foreach (var item in posts)
                {
                    if (ServicesUtil.IsInTimePost(item))
                    {
                        res.Add(new PostInfomation
                        {
                            Address = item.AddressSlot,
                            AvailableSlot = item.QuantitySlot,
                            PostId = item.Id,
                            PostImgUrl = item.ImgUrl,
                            SortDescript = item.ContentPost,
                            Time = $"{item.StartTime} - {item.EndTime}",
                            UserId = item.IdUserTo,
                            UserImgUrl = item.IdUserToNavigation.ImgUrl,
                            UserName = item.IdUserToNavigation.UserName
                        });
                    }
                }
            }
            return res;
        }
    }
}
