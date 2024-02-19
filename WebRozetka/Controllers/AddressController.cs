using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data;
using WebRozetka.Services;
using WebRozetka.Models.Address;
using Microsoft.EntityFrameworkCore;
using WebRozetka.Data.Entities.Addres;
using Microsoft.AspNetCore.Authorization;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AddressController : ControllerBase
    {
        private readonly AppEFContext _context;
        private readonly IMapper _mapper;

        public AddressController(AppEFContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("areas")]
        public async Task<IActionResult> GetAreas()
        {
            var areas = _mapper.Map<List<AreaViewModel>>(await _context.Areas.ToListAsync());

            return Ok(areas);
        }

        [HttpGet("settlements")]
        public async Task<IActionResult> GetSettlements([FromQuery] int areaId, string search = "")
        {
            var entities = await _context.Settlements
               .Where(x => x.AreaId == areaId && (string.IsNullOrEmpty(search) || x.Description.ToLower().StartsWith(search.ToLower())))
               .OrderBy(x => x.Description)
               .Take(20)
               .ToListAsync();

            var settlements = _mapper.Map<List<SettlementViewModel>>(entities);

            return Ok(settlements);
        }


        [HttpGet("warehouses")]
        public async Task<IActionResult> GetWarehouses([FromQuery] int settlementId, string search = "")
        {
            var entities = _context.Warehouses
                .Where(x => x.SettlementId == settlementId && (string.IsNullOrEmpty(search) || x.Description.ToLower().Contains(search.ToLower())))
                .OrderBy(x => x.Number)
                .Take(20)
                .ToList();

            var warehouses = _mapper.Map<List<WarehouseViewModel>>(entities);

            return Ok(warehouses);
        }


        [HttpGet("warehouse-detail")]
        public async Task<IActionResult> GetWarehousesDetail([FromQuery] int warehouseId)
        {



            var entities = _context.Warehouses
                .Where(x => x.Id == warehouseId)
                .SingleOrDefault();


            var warehouse = _mapper.Map<WarehouseFullViewModel>(entities);

            return Ok(warehouse);
        }
    }
}
