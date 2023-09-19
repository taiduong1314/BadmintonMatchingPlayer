using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        public UserInformation GetExistUser(LoginInformation info);
    }
}
