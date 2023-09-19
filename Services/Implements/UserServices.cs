using Entities.RequestObject;
using Entities.ResponseObject;
using Repositories.Intefaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class UserServices : IUserServices
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IJwtSupport _jwtSupport;

        public UserServices(IRepositoryManager repositoryManager, IJwtSupport jwtSupport)
        {
            _repositoryManager = repositoryManager;
            _jwtSupport = jwtSupport;
        }

        public UserInformation GetExistUser(LoginInformation info)
        {
            var res = new UserInformation();
            var user = _repositoryManager.User.FindByCondition(x => x.Email == info.Email && x.UserPassword == info.Password, false).FirstOrDefault();
            if (user != null)
            {
                res = new UserInformation
                {
                    Avatar = user.ImgUrl,
                    Id = user.Id,
                    UserName = user.FullName,
                    Token = _jwtSupport.CreateToken(1, user.Id)
                };
            }
            return res;
        }
    }
}
