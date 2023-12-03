using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using System.Xml.Linq;

namespace Services.Implements
{
    public enum PostType
    {
        MatchingPost = 1,
        Blog = 2
    }

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
                throw new Exception();
            }

        }

        public async Task<int> CreatePost(int user_id, NewPostInfo info)
        {
            var urls = "";
            foreach (var url in info.ImgUrls)
            {
                try
                {
                    urls += $"{await HandleImg(url)};";
                }
                catch
                {
                    return -1;
                }
            }

            try
            {
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
                    SlotsInfo = info.SlotsToString(),
                    IdType = (int)PostType.MatchingPost
                };
                _repositoryManager.Post.Create(newPost);
                _repositoryManager.SaveAsync().Wait();

                foreach (var item in info.Slots)
                {
                    var roomEnt = new Entities.Models.ChatRoom
                    {
                        Code = $"{newPost.Id}_{item.StartTime.Value.ToString("dd/MM/yyyy")}",
                        Name = $"{item.StartTime.Value.ToString("dd/MM/yyyy")}",
                        CoverImage = newPost.ImgUrl,
                        UpdateTime = DateTime.UtcNow.AddHours(7),
                    };
                    _repositoryManager.ChatRoom.Create(roomEnt);
                    _repositoryManager.SaveAsync().Wait();

                    _repositoryManager.ChatRoomUser.Create(new Entities.Models.UserChatRoom
                    {
                        RoomId = roomEnt.Id,
                        UserId = user_id
                    });
                    await _repositoryManager.SaveAsync();
                }

                return newPost.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool DeletePost(int post_id)
        {
            var post = _repositoryManager.Post.FindByCondition(x => x.Id == post_id && !x.IsDeleted && x.IdType == (int)PostType.MatchingPost, true).FirstOrDefault();
            if (post != null)
            {
                post.IsDeleted = true;
                _repositoryManager.SaveAsync();
                return true;
            }
            return false;
        }

        public List<PostOptional> GetListOptionalPost()
        {
            var optList = _repositoryManager.Post.FindByCondition(x => !x.IsDeleted && x.IdType == (int)PostType.MatchingPost, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .ToList();
            var res = new List<PostOptional>();
            foreach (var post in optList)
            {
                if (IsPostValid(post))
                {
                    res.Add(new PostOptional
                    {
                        IdPost = post.Id,
                        Title = post.Title,
                        AddressSlot = post.AddressSlot,
                        ContentPost = post.ContentPost,
                        ImgUrlPost = post.ImgUrl,
                        FullName = post.IdUserToNavigation?.FullName,
                        UserImgUrl = post.IdUserToNavigation?.ImgUrl,
                        HighlightUrl = post.ImgUrl,
                        UserId = post.IdUserTo
                    });
                }
            }

            for (var i = 0; i < res.Count(); i++)
            {
                var cPost = res[i];
                var post = _repositoryManager.Post.FindByCondition(x => x.Id == cPost.IdPost, false).Include(x => x.Slots).FirstOrDefault();
                if (post != null)
                {
                    GetPostOptional(post, ref cPost);
                }
                res[i] = cPost;
            }
            return res;
        }
        public async Task<List<PostOptional>> GetAllPost()
        {
            var listpost = await _repositoryManager.Post.FindByCondition(x => !x.IsDeleted && x.IdType == (int)PostType.MatchingPost, false).Include(x => x.IdUserToNavigation).ToListAsync();
            var res = new List<PostOptional>();
            foreach (var post in listpost)
            {
                if (IsPostValid(post))
                {
                    res.Add(new PostOptional
                    {
                        IdPost = post.Id,
                        Title = post.Title,
                        AddressSlot = post.AddressSlot,
                        ContentPost = post.ContentPost,
                        ImgUrlPost = post.ImgUrl,
                        FullName = post.IdUserToNavigation?.FullName,
                        UserImgUrl = post.IdUserToNavigation?.ImgUrl,
                        HighlightUrl = post.ImgUrl,
                        UserId = post.IdUserTo
                    });
                }
            }

            for (var i = 0; i < res.Count(); i++)
            {
                var cPost = res[i];
                var post = _repositoryManager.Post.FindByCondition(x => x.Id == cPost.IdPost, false).Include(x => x.Slots).FirstOrDefault();
                if (post != null)
                {
                    GetPostOptional(post, ref cPost);
                }
                res[i] = cPost;
            }
            return res;
        }

        private bool IsPostValid(Post post)
        {
            var lastSlot = new SlotInfo(post.SlotsInfo.Split(";")[0]);
            foreach (var slot in post.SlotsInfo.Split(";"))
            {
                if (slot != string.Empty)
                {
                    var slotInfo = new SlotInfo(slot);
                    var joinSlot = _repositoryManager.Slot
                        .FindByCondition(x =>
                        !x.IsDeleted &&
                        x.ContentSlot == slotInfo.StartTime.Value.ToString("dd/MM/yyyy") &&
                        x.IdPost == post.Id, false).Count();
                    if (slotInfo.AvailableSlot - joinSlot <= 0)
                    {
                        return false;
                    }
                    if(slotInfo.StartTime > lastSlot.StartTime)
                    {
                        lastSlot = slotInfo;
                    }
                }
            }
            if(lastSlot.StartTime < DateTime.UtcNow.AddHours(-7))
            {
                return false;
            }
            return true;
        }

        private void GetPostOptional(Post post, ref PostOptional optPost)
        {
            var finalInfo = new SlotInfo();
            int joinedSlot = 0;
            foreach (var slot in post.SlotsInfo.Split(";"))
            {
                if (slot != string.Empty)
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
            return _repositoryManager.Post.FindByCondition(x => x.IdType == (int)PostType.MatchingPost, true)
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
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id && !x.IsDeleted && x.IdType == (int)PostType.MatchingPost, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .Select(x => new PostInfomation(x)).ToList();
        }

        public List<PostInfomation> GetManagedPostAdmin(int user_id)
        {
            return _repositoryManager.Post.FindByCondition(x => x.IdUserTo == user_id && !x.IsDeleted && x.IdType == (int)PostType.MatchingPost, true)
                .OrderByDescending(x => x.SavedDate)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .Select(x => new PostInfomation(x)).ToList();
        }

        public List<PostOptional> GetPostByPlayGround(string play_ground)
        {
            var listpost = _repositoryManager.Post.FindByCondition(x => x.AddressSlot != null
                && x.AddressSlot.Contains(play_ground)
                && !x.IsDeleted
                && x.IdType == (int)PostType.MatchingPost, false)
                .Include(x => x.IdUserToNavigation)
                .OrderByDescending(x => x.Id)
                .Select(x => new PostOptional
                {
                    IdPost = x.Id,
                    Title = x.Title,
                    AddressSlot = x.AddressSlot,
                    ContentPost = x.ContentPost,
                    ImgUrlPost = x.ImgUrl,
                    FullName = x.IdUserToNavigation.FullName,
                    UserImgUrl = x.IdUserToNavigation.ImgUrl,
                    HighlightUrl = x.ImgUrl,
                    UserId = x.IdUserTo
                })
                .ToList();

            var res = new List<PostOptional>();
            for (var i = 0; i < listpost.Count(); i++)
            {
                var cPost = listpost[i];
                var post = _repositoryManager.Post.FindByCondition(x => x.Id == cPost.IdPost, false).Include(x => x.Slots).FirstOrDefault();
                if (post != null)
                {
                    GetPostOptional(post, ref cPost);
                }
                res.Add(cPost);
            }
            return res;
        }

        public PostDetail GetPostDetail(int id_post)
        {
            var x = _repositoryManager.Post
                .FindByCondition(x =>
                x.Id == id_post
                && !x.IsDeleted
                && x.IdType == (int)PostType.MatchingPost, false)
                .Include(x => x.IdUserToNavigation)
                .Include(x => x.Slots)
                .FirstOrDefault();
            if (x != null)
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
                    && x.IdType == (int)PostType.MatchingPost
                        , true)
                    .Include(x => x.IdUserToNavigation)
                    .Include(x => x.Slots)
                    .ToList();

                foreach (var item in posts)
                {
                    if (IsPostValid(item))
                    {
                        res.Add(new PostInfomation(item));
                    }
                }
            }
            return res;
        }

        public List<JoinedPost> GetJoined(int user_id)
        {
            var res = new List<JoinedPost>();

            var transactions = _repositoryManager.Transaction
                .FindByCondition(x => x.IdUser == user_id, false)
                .OrderByDescending(x => x.Id)
                .ToList();
            if (transactions == null || transactions.Count == 0)
            {
                return res;
            }

            foreach (var item in transactions)
            {
                var slot = _repositoryManager.Slot.FindByCondition(x => x.TransactionId == item.Id, false)
                    .FirstOrDefault();
                if (slot == null)
                    continue;

                var post = _repositoryManager.Post.FindByCondition(x => x.Id == slot.IdPost && x.IdType == (int)PostType.MatchingPost, false)
                    .Include(x => x.Slots)
                    //.ThenInclude(x => x.User)
                    .FirstOrDefault();

                var finalInfo = new SlotInfo();
                var firstInfo = new SlotInfo()
                {
                    StartTime = DateTime.MaxValue
                };
                int joinedSlot = 0;
                var bookedInfos = new List<BookedSlotInfo>();
                foreach (var infoStr in post.SlotsInfo.Split(';'))
                {
                    if (infoStr != string.Empty)
                    {
                        var info = new SlotInfo(infoStr);
                        bookedInfos.Add(new BookedSlotInfo
                        {
                            BookedSlot = post.Slots.Where(x => x.ContentSlot == info.StartTime.Value.ToString("dd/MM/yyyy")).Count(),
                            CreateSlot = info.AvailableSlot,
                            //ImageUrls = post.Slots.Where(x => x.ContentSlot == info.StartTime.Value.ToString("dd/MM/yyyy")).Select(x => x.User.ImgUrl).ToList()
                        });

                        var joinSlot = _repositoryManager.Slot
                            .FindByCondition(x =>
                            !x.IsDeleted &&
                            x.ContentSlot == info.StartTime.Value.ToString("dd/MM/yyyy") &&
                            x.IdPost == post.Id, false).Count();

                        if(info.StartTime < firstInfo.StartTime)
                        {
                            firstInfo = info;
                        }
                        if (info.AvailableSlot - joinSlot >= finalInfo.AvailableSlot)
                        {
                            finalInfo = info;
                            joinedSlot = joinSlot;
                        }
                    }
                }

                var isCancel = firstInfo.StartTime.Value.AddHours(-24) >= DateTime.UtcNow.AddHours(7);
                var roomId = _repositoryManager.ChatRoom
                    .FindByCondition(x => x.Code == $"{post.Id}_{finalInfo.StartTime.Value.ToString("dd/MM/yyyy")}", false)
                    .Select(x => x.Id).FirstOrDefault();


                res.Add(new JoinedPost
                {
                    AreaName = post.AddressSlot,
                    MoneyPaid = item.MoneyTrans,
                    TransacionId = item.Id,
                    BookedInfos = bookedInfos,
                    PostId = post.Id,
                    Status = ((TransactionStatus)item.Status).ToString(),
                    PostTitle = post.Title,
                    AvailableSlot = (finalInfo.AvailableSlot - joinedSlot).ToString(),
                    EndTime = finalInfo.EndTime.Value.ToString("dd/MM/yyyy hh:mm:ss tt"),
                    StartTime = finalInfo.StartTime.Value.ToString("dd/MM/yyyy hh:mm:ss tt"),
                    CoverImage = post.ImgUrl,
                    CanReport = DateTime.UtcNow.AddHours(7) < finalInfo.EndTime.Value,
                    ChatRoomUrl = $"https://badminton-matching-24832d1c4b03.herokuapp.com/chatHub?id={roomId}",
                    ChatRoomId = roomId,
                    IsCancel = isCancel
                });
            }
            return res;
        }

        public async Task<List<Room>> GetChatRooms(int post_id)
        {
            var post = await _repositoryManager.Post.FindByCondition(x => x.Id == post_id && x.IdType == (int)PostType.MatchingPost, false).FirstOrDefaultAsync();

            if (post == null)
            {
                throw new NotImplementedException();
            }

            var res = new List<Room>();
            foreach (var infoStr in post.SlotsInfo.Split(';'))
            {
                var info = new SlotInfo(infoStr);
                var room = await _repositoryManager.ChatRoom
                    .FindByCondition(x => 
                    x.Name == $"{info.StartTime.Value.ToString("dd/MM/yyyy")}" &&
                    x.Code == $"{post.Id}_{info.StartTime.Value.ToString("dd/MM/yyyy")}"
                    , false)
                    .Select(x => new Room
                    {
                        Id = x.Id,
                        PlayDate = x.Name
                    }).FirstOrDefaultAsync();

                if (room != null)
                res.Add(room);
            }
            return res;
        }

        public async Task<bool> CreateBlog(int user_id, NewBlogInfo info)
        {
            var post = new Post
            {
                Title = info.Title,
                ContentPost = info.Description,
                IdUserTo = user_id,
                IdType = (int)PostType.Blog,
                SavedDate = DateTime.UtcNow.AddHours(7),
                IsDeleted = false
            };

            _repositoryManager.Post.Create(post);
            await _repositoryManager.SaveAsync();
            return true;
        }

        public async Task<List<BlogInList>> GetAllBlogs()
        {
            var blogs = await _repositoryManager.Post
                .FindByCondition(x => x.IdType == (int)PostType.Blog && !x.IsDeleted, false)
                .Select(x => new BlogInList 
                {
                    Id = x.Id,
                    CreateTime = x.SavedDate.ToString("dd/MM/yyyy HH:mm"),
                    ShortDescription = x.ContentPost.Substring(0, 100),
                    Title = x.Title,
                    UserCreateName = x.IdUserToNavigation.FullName
                })
                .ToListAsync();

            return blogs;
        }

        public async Task<BlogDetail> GetBlogDetail(int blog_id)
        {
            var blog = await _repositoryManager.Post
                .FindByCondition(x => x.IdType == (int)PostType.Blog && x.Id == blog_id && !x.IsDeleted, false)
                .Select(x => new BlogDetail
                {
                    Id = x.Id,
                    CreateTime = x.SavedDate.ToString("dd/MM/yyyy HH:mm"),
                    Description = x.ContentPost,
                    Title = x.Title,
                    UserCreateName = x.IdUserToNavigation.FullName
                }).FirstOrDefaultAsync();

            return blog;
        }
    }
}
