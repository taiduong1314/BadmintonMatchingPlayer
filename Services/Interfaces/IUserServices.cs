using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        void AddPlayingArea(int user_id, NewPlayingArea info);
        public UserInformation GetExistUser(LoginInformation info);
        public bool ExistUserId(int userId);
        bool IsUserExist(string? email);
        int RegistUser(RegisInfomation info);
        void AddPlayingLevel(int user_id, NewPlayingLevel info);
        void AddPlayingWay(int user_id, NewPlayingWay info);
        List<string> GetUserAreas(int user_id);
        List<UserSuggestion> FindUserByArea(List<string> areas);
        int GetUserSkill(int user_id);
        List<UserSuggestion> FindUserBySkill(int skill, List<UserSuggestion> res);
        List<string> GetUserPlayWay(int user_id);
        List<UserSuggestion> FindUserByPlayWays(List<string> ways, List<UserSuggestion> res);
        string CreateVerifyToken(string? email);
        bool CheckRemoveVefToken(UserVerifyToken info);
        bool UpdatePassword(string email, UpdatePassword info);
        List<CommentInfos> GetComments(int user_id);
        int SaveComment(int user_id, int user_id_receive_comment, AddCommentToUser comment);
        List<BandedUsers> GetBandedUsers(int user_id);
    }
}
