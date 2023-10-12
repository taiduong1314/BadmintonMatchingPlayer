using Entities.ResponseObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonMatching.Controllers
{
    [Route("api/districts")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private List<District> districts = new List<District>
    {
        new District { Id = 1, Name = "Quận 1", CityId = 1},
        new District { Id = 2, Name = "Quận 3", CityId = 1},
        new District { Id = 3, Name = "Quận 4", CityId = 1},
        new District { Id = 4, Name = "Quận 5", CityId = 1},
        new District { Id = 5, Name = "Quận 6", CityId = 1},
        new District { Id = 6, Name = "Quận 7", CityId = 1},
        new District { Id = 7, Name = "Quận 8", CityId = 1},
        new District { Id = 8, Name = "Quận 10", CityId = 1},
        new District { Id = 9, Name = "Quận 11", CityId = 1},
        new District { Id = 10, Name = "Quận 12", CityId = 1},
        new District { Id = 11, Name = "Quận Bình Thạnh", CityId = 1},
        new District { Id = 12, Name = "Quận Bình Tân", CityId = 1},
        new District { Id = 13, Name = "Quận Tân Bình", CityId = 1},
        new District { Id = 14, Name = "Quận Tân Phú", CityId = 1},
        new District { Id = 15, Name = "Quận Gò Vấp", CityId = 1},
        new District { Id = 16, Name = "Quận Phú Nhuận", CityId = 1},
        new District { Id = 17, Name = "Hóc Môn", CityId = 1},
        new District { Id = 18, Name = "Củ Chi", CityId = 1},
        new District { Id = 19, Name = "Nhà Bè", CityId = 1},
        new District { Id = 20, Name = "Bình Chánh", CityId = 1},
        new District { Id = 21, Name = "Cần Giờ", CityId = 1},
        new District { Id = 22, Name = "Quận Thủ Đức", CityId = 2},
        new District { Id = 23, Name = "Quận 9", CityId = 2},
        new District { Id = 24, Name = "Quận 2", CityId = 2},
        // Thêm thông tin về các quận khác
    };
        [HttpGet]
        public IActionResult GetDistricts()
        {
            return Ok(districts);
        }

        [HttpGet("city/{cityId}")]
        public IActionResult GetDistrictsInCity(int cityId)
        {
            // Lọc danh sách quận dựa trên cityId
            var cityDistricts = districts.Where(d => d.CityId == cityId).ToList();
            return Ok(cityDistricts);
        }
    }
}
