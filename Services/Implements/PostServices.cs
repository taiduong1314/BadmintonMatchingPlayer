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
                return base64encodedstring;
            }

        }

        public async Task<int> CreatePost(int user_id, NewPostInfo info)
        {
            
            var urls = "";
            foreach (var url in info.ImgUrls)
            {
                urls += $"{await HandleImg(url)};";
            }
            var newPost = new Post
            {
                CategorySlot = info.CategorySlot,
                LevelSlot = info.LevelSlot,
                Title = info.Title,
                AddressSlot = info.Address,
                ContentPost = info.Description,
                SavedDate = DateTime.UtcNow.AddHours(7),
                ImgUrl = await HandleImg(info.HighlightUrl),
                ImageUrls = urls,
                IdUserTo = user_id,
                SlotsInfo = info.SlotsToString()
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
                    SavedDate = DateTime.UtcNow,
                    IdType = x.IdType,
                    IdTypeNavigation = x.IdTypeNavigation,
                    IdUserTo = x.IdUserTo,
                    IdUserToNavigation = x.IdUserToNavigation,
                    ImgUrl = x.ImgUrl,
                    LevelSlot = x.LevelSlot,
                    SlotsInfo = x.SlotsInfo,
                    Slots = x.Slots,
                    Status = x.Status
                })
                .ToListAsync();
            return res;
        }

        public List<PostOptional> GetListOptionalPost()
        {
            var optList = _repositoryManager.Post.FindByCondition(x => !x.IsDeleted, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Select(x => new PostOptional
                {
                    IdPost = x.Id,
                    Title = x.Title,
                    AddressSlot = x.AddressSlot,
                    ContentPost = x.ContentPost,
                    ImgUrlPost = x.ImgUrl,
                    FullName = x.IdUserToNavigation.FullName,
                    UserImgUrl = x.IdUserToNavigation.ImgUrl,
                    HighlightUrl = x.ImgUrl
                }).ToList();

            for(var i = 0; i < optList.Count(); i++)
            {
                var cPost = optList[i];
                var post = _repositoryManager.Post.FindByCondition(x => x.Id == cPost.IdPost, false).Include(x => x.Slots).FirstOrDefault();
                if(post != null)
                {
                    GetPostOptional(post, ref cPost);
                }
                optList[i] = cPost;
            }
            return optList;
        }

        private void GetPostOptional(Post post, ref PostOptional optPost)
        {
            var finalInfo = new SlotInfo();
            int joinedSlot = 0;
            foreach (var slot in post.SlotsInfo.Split(";"))
            {
                if(slot != string.Empty)
                {
                    var slotInfo = new SlotInfo(slot);
                    var joinSlot = _repositoryManager.Slot
                        .FindByCondition(x =>
                        !x.IsDeleted &&
                        x.ContentSlot == slotInfo.StartTime.Value.ToString("dd/MM/yyyy") &&
                        x.IdPost == post.Id, false).Count();
                    if (slotInfo.AvailableSlot - joinSlot >= finalInfo.AvailableSlot)
                    {
                        finalInfo = slotInfo;
                        joinedSlot = joinSlot;
                    }
                }
            }
            optPost.Days = finalInfo.StartTime.Value.ToString("dd/MM/yyyy");
            optPost.StartTime = finalInfo.StartTime.Value.ToString("HH:mm");
            optPost.EndTime = finalInfo.EndTime.Value.ToString("HH:mm");
            optPost.QuantitySlot = finalInfo.AvailableSlot - joinedSlot;
            optPost.Price = finalInfo.Price;
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
                .Include(x => x.Slots)
                .Select(x => new PostInfomation(x)).ToList();
        }

        public List<PostInfomation> GetManagedPostAdmin(int user_id)
        {
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id && !x.IsDeleted, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .Select(x => new PostInfomation(x)).ToList();
        }

        public List<PostInfomation> GetPostByPlayGround(string play_ground)
        {
            var targetTime = DateTime.UtcNow.AddHours(2);
            var res = new List<PostInfomation>();
            var posts = _repositoryManager.Post.FindByCondition(
                x => x.AddressSlot != null
                && x.AddressSlot == play_ground
                && !x.IsDeleted
                , true)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .ToList();

            foreach (var item in posts)
            {
                if (ServicesUtil.IsInTimePost(item))
                {
                    res.Add(new PostInfomation(item));
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
            if(x != null)
            {
                var postDetail = new PostDetail(x);
                postDetail.ImageUrls = x.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                return postDetail;
            }
            return new PostDetail();
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
                    && !x.IsDeleted
                        , true)
                    .Include(x => x.IdUserToNavigation)
                    .Include(x => x.Slots)
                    .ToList();

                foreach (var item in posts)
                {
                    if (ServicesUtil.IsInTimePost(item))
                    {
                        res.Add(new PostInfomation(item));
                    }
                }
            }
            return res;
        }
    }
}
