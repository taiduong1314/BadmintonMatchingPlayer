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
                ContentPost = info.Description,
                SavedDate = DateTime.UtcNow
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

        public List<PostOptional> GetListOptionalPost()
        {
            return _repositoryManager.Post.FindByCondition(x => x.QuantitySlot > 0,true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Select(x=> new PostOptional
                {
                    AddressSlot = x.AddressSlot,
                    ContentPost = x.ContentPost,
                    Days = x.Days,
                    ImgUrlPost = x.ImgUrl,                   
                    EndTime = x.EndTime,
                    StartTime = x.StartTime,
                    QuantitySlot = x.QuantitySlot,
                    FullName = x.IdUserToNavigation.FullName,
                    
                    UserImgUrl = x.IdUserToNavigation.ImgUrl

                }).ToList();
        }

        public List<PostInfomation> GetManagedPost(int user_id)
        {
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Select(x => new PostInfomation
            {
                Address = x.AddressSlot,
                AvailableSlot = x.QuantitySlot,
                PostId = x.Id,
                PostImgUrl = x.ImgUrl,
                SortDescript = x.ContentPost,
                Time = $"{x.StartTime} - {x.EndTime}",
                UserId = x.IdUserTo,
                UserImgUrl = x.IdUserToNavigation.ImgUrl,
                UserName = x.IdUserToNavigation.UserName
            }).ToList();
        }

        public List<PostInfomation> GetManagedPostAdmin(int user_id)
        {
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Select(x => new PostInfomation
                {
                    Address = x.AddressSlot,
                    AvailableSlot = x.QuantitySlot,
                    PostId = x.Id,
                    PostImgUrl = x.ImgUrl,
                    SortDescript = x.ContentPost,
                    Time = $"{x.StartTime} - {x.EndTime}",
                    UserId = x.IdUserTo,
                    UserImgUrl = x.IdUserToNavigation.ImgUrl,
                    UserName = x.IdUserToNavigation.UserName
                }).ToList();
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

        public PostDetail GetPostDetail(int id_post)
        {
            var res = _repositoryManager.Post.FindByCondition(x => x.Id == id_post, false).Include(x => x.IdUserToNavigation)
                .Select(x => new PostDetail
                {
                    AddressSlot = x.AddressSlot,
                    CategorySlot = x.CategorySlot,
                    ContentPost = x.ContentPost,
                    Days = x.Days,
                    EndTime = x.EndTime,
                    FullName = x.IdUserToNavigation.FullName,
                    ImgUrl = x.ImgUrl,
                    LevelSlot = x.LevelSlot,
                    PriceSlot = x.PriceSlot,
                    QuantitySlot = x.QuantitySlot,
                    ImgUrlUser = x.IdUserToNavigation.ImgUrl,
                    SortProfile = x.IdUserToNavigation.SortProfile,
                    StartTime = x.StartTime,
                    TotalRate = x.IdUserToNavigation.TotalRate
                }).FirstOrDefault();
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
