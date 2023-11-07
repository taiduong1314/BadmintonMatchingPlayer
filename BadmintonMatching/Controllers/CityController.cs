using Entities.ResponseObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonMatching.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CityController : ControllerBase
    {

        private List<City> cities = new List<City>
    {
        new City { Id = 1, Name = "Thành Phố Hồ Chí Minh" },
        new City { Id = 2, Name = "Thành Phố Thủ Đức" },
        // Thêm thông tin về các thành phố khác
    };
        [HttpGet]
        public IActionResult GetCities()
        {
            return Ok(new SuccessObject { Data = cities, Message = Message.SuccessMsg });
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var city = cities.FirstOrDefault(c => c.Id == id);
            if (city == null)
            {
                return Ok(new SuccessObject
                {
                    Message = "No cities found!"
                });
            }
            return Ok(new SuccessObject { Data = city, Message = Message.SuccessMsg});
        }
    }
}
