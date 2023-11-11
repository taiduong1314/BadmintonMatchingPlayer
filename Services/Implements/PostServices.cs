using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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

        public async Task<string> HandleImg(string base64encodedstring)
        {
            var revertbase64 = base64encodedstring.Replace("data:image/jpeg;base64,", "");
            try
            {
                var bytes = Convert.FromBase64String(revertbase64);
                var contents = new StreamContent(new MemoryStream(bytes));
                Account account = new Account(
                    "dbjvirvym",
                    "487892318776179",
                    "txx6fF8ZVsT72id6ySvqNqwrN0E");
                Cloudinary cloudinary = new Cloudinary(account);

                string publicId = Guid.NewGuid().ToString();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(publicId, new MemoryStream(bytes)),
                    PublicId = publicId
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                return uploadResult.SecureUrl.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return string.Empty;
            }

        }


        public async Task<int> CreatePost(int user_id, NewPostInfo info)
        {
            
            var urls = "";
            foreach (var url in info.ImgUrls)
            {
                urls += $"{HandleImg(url).Result};";
            }
            var newPost = new Post
            {
                CategorySlot = info.CategorySlot,
                LevelSlot = info.LevelSlot,
                Title = info.Title,
                AddressSlot = info.Address,
                Days = $"{info.Day}:{info.Month}:{info.Year}",
                StartTime = info.StartTime,
                EndTime = info.EndTime,
                PriceSlot = decimal.Parse(info.Price),
                QuantitySlot = info.AvailableSlot,
                ContentPost = info.Description,
                SavedDate = DateTime.UtcNow.AddHours(7),
                ImgUrl = await HandleImg(info.HighlightUrl),
                ImageUrls = urls,
                IdUserTo = user_id
            };
            _repositoryManager.Post.Create(newPost);
            _repositoryManager.SaveAsync().Wait();
            return newPost.Id;
        }

        public bool DeletePost(int post_id)
        {
            var post = _repositoryManager.Post.FindByCondition(x => x.Id == post_id && !x.IsDeleted, true).FirstOrDefault();
            if (post != null)
            {
                post.IsDeleted = true;
                _repositoryManager.SaveAsync();
                return true;
            }
            return false;
        }

        public List<string?> GetAllPlayGround()
        {
            var posts = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                    && x.QuantitySlot > 0
                    && !x.IsDeleted
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

        public async Task<List<Post>> GetAllPost()
        {
            var res = await _repositoryManager.Post.FindAll(false)
                .Select(x => new Post
                {
                    Id = x.Id,
                    Title = x.Title,
                    AddressSlot = x.AddressSlot,
                    CategorySlot = x.CategorySlot,
                    ContentPost = x.ContentPost,
                    SavedDate = DateTime.UtcNow.AddHours(7),
                    Days = x.Days,
                    EndTime = x.EndTime,
                    StartTime = x.StartTime,
                    IdType = x.IdType,
                    IdTypeNavigation = x.IdTypeNavigation,
                    IdUserTo = x.IdUserTo,
                    IdUserToNavigation = x.IdUserToNavigation,
                    ImgUrl = x.ImgUrl,
                    LevelSlot = x.LevelSlot,
                    PriceSlot = x.PriceSlot,
                    QuantitySlot = x.QuantitySlot,
                    Slots = x.Slots,
                    Status = x.Status
                }).ToListAsync();
            return res;
        }

        public List<PostOptional> GetListOptionalPost()
        {
            return _repositoryManager.Post.FindByCondition(x => x.QuantitySlot > 0 && !x.IsDeleted, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Select(x => new PostOptional
                {
                    IdPost = x.Id,
                    Title = x.Title,
                    AddressSlot = x.AddressSlot,
                    ContentPost = x.ContentPost,
                    Days = x.Days,
                    ImgUrlPost = x.ImgUrl,
                    EndTime = x.EndTime,
                    StartTime = x.StartTime,
                    QuantitySlot = x.QuantitySlot,
                    FullName = x.IdUserToNavigation.FullName,
                    UserImgUrl = x.IdUserToNavigation.ImgUrl,
                    HighlightUrl = x.ImgUrl,
                    Price = x.PriceSlot
                }).ToList();
        }

        public List<ListPostByAdmin> GetListPostByAdmin()
        {
            return _repositoryManager.Post.FindAll(true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation).ThenInclude(x => x.UserRoleNavigation)
                .Select(x => new ListPostByAdmin
                {
                    CreatedDate = x.SavedDate,
                    FullName = x.IdUserToNavigation.FullName,
                    IdUser = x.IdUserTo,
                    RoleUser = x.IdUserToNavigation.UserRoleNavigation.RoleName,
                    Status = x.Status,
                    TotalViewer = x.TotalViewer

                }).ToList();
        }

        public List<PostInfomation> GetManagedPost(int user_id)
        {
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id && !x.IsDeleted, true)
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
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id && !x.IsDeleted, true)
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
                && !x.IsDeleted
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

            var x = _repositoryManager.Post
                .FindByCondition(x =>
                x.Id == id_post
                && !x.IsDeleted, false)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .FirstOrDefault();

            var postDetail = new PostDetail
            {
                AddressSlot = x.AddressSlot,
                CategorySlot = x.CategorySlot,
                ContentPost = x.ContentPost,
                Days = x.Days,
                EndTime = x.EndTime,
                FullName = x.IdUserToNavigation.FullName,
                HightLightImage = x.ImgUrl,
                LevelSlot = x.LevelSlot,
                PriceSlot = x.PriceSlot,
                QuantitySlot = x.QuantitySlot,
                ImgUrlUser = x.IdUserToNavigation.ImgUrl,
                SortProfile = x.IdUserToNavigation.SortProfile,
                StartTime = x.StartTime,
                TotalRate = x.IdUserToNavigation.TotalRate,
                UserId = x.IdUserTo.Value,
                Title = x.Title
            };
            postDetail.AvailableSlot = GetAvailableSlot(x);
            postDetail.ImageUrls = x.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

            return postDetail;
        }

        private List<string> GetAvailableSlot(Post post)
        {
            //Get list date dd/mm/yy
            if (post.Days != null)
            {
                var days = post.Days.Split(":");
                var month = string.Empty;
                var year = string.Empty;
                var ds = new List<string>();
                foreach (var day in days)
                {
                    if (day.Contains(';'))
                    {
                        foreach (var d in day.Split(";"))
                        {
                            ds.Add(d);
                        }
                    }
                    else if (month == string.Empty)
                    {
                        month = day;
                    }
                    else if (year == string.Empty)
                    {
                        year = day;
                    }
                }

                for (var i = 0; i < ds.Count; i++)
                {
                    var item = ds[i];
                    if (item != null)
                    {
                        item = $"{item}/{month}/{year}";
                        ds[i] = item;
                    }
                }

                var res = new List<string>();
                //Get list by date
                foreach(var item in ds)
                {
                    var slots = _repositoryManager.Slot.FindByCondition(x => x.IdPost == post.Id && x.ContentSlot == item && !x.IsDeleted, false).Count();
                    var avai = post.QuantitySlot - slots;
                    res.Add($"{item}:{avai}");
                }
                return res;
            }
            return new List<string>();
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
                    && !x.IsDeleted
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
