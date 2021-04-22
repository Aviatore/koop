using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Koop.models;
using Koop.Models;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private IGenericUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SupplierController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        [HttpGet("allsuppliers")]
        public IActionResult AllSuppliers()
        {
            try
            {
                return Ok(_uow.Repository<SupplierView>().GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [AllowAnonymous]
        [HttpGet("supplier/{supplierId}")]
        public IActionResult Supplier(Guid supplierId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(supplierId));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/edit")]
        public IActionResult EditSupplier(Guid supplierId)
        {
            try
            {
                return Ok(_uow.ShopRepository().GetSupplier(supplierId));
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpPost("supplier/update")]
        public IActionResult UpdateSupplier(SupplierView sup) 
        {
            // //TEST
            // SupplierViewMap sup = new SupplierViewMap()
            // {
            //     SupplierId = Guid.Parse("12414db7-aae9-42a4-afb4-ff4ec756ae29"),
            //     SupplierAbbr = "TESTEDIT",
            //     SupplierName = "Testowy Dostawca",
            //     Description = "Teścik smaczny",
            //     Email = "abc@abc.pl",
            //     Phone = "12323456789",
            //     OrderClosingDate = DateTime.Parse("2021-03-24 00:00:00"),
            //     OproFirstName = "Tadeusz",
            //     OproLastName = "Batko",
            //     Blocked = false,
            //     Available =  false,
            //     Receivables = 0
            // };
            // //test end
            
            try
            {
                Guid? oproId = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName)?.Id;
        
                if (oproId == null)
                {
                    return BadRequest(new {error = "OpRo name is invalid."});
                }

                sup.OproId = (Guid) oproId;
                var supplierMap = _mapper.Map<Supplier>(sup);

                _uow.ShopRepository().UpdateSupplier(supplierMap);
                
                return Ok(new {info = $"The supplier has been updated (supplier ABBR: {sup.SupplierAbbr})."});

            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        
        [Authorize(Roles = "Admin,Koty,Opro")]
        [HttpPost("supplier/add")]
        public async Task<IActionResult> AddSupplier(SupplierView sup)
        {
            // //TEST
            // SupplierViewMap sup = new SupplierViewMap()
            // {
            //     SupplierAbbr = "TEST2",
            //     SupplierName = "Testowany",
            //     Description = "Pycha",
            //     Email = "abc@abc.pl",
            //     Phone = "123234",
            //     OrderClosingDate = DateTime.Parse("2021-03-24 00:00:00"),
            //     OproFirstName = "Henryk",
            //     OproLastName = "Sienkiewicz",
            //     Receivables = 100.50,
            //     Blocked = true,
            //     Available = false,
            //     Picture = null
            // };
            // // test end

            try
            {
                Guid? oproId = _uow.Repository<User>()
                    .GetDetail(u => u.FirstName == sup.OproFirstName && u.LastName == sup.OproLastName)?.Id;
        
                if (oproId == null)
                {
                    return BadRequest(new {error = "OpRo name is invalid."});
                }

                sup.OproId = (Guid) oproId;
                 
                 var supplierMap = _mapper.Map<Supplier>(sup);

                 await _uow.Repository<Supplier>().AddAsync(supplierMap);
        
                 await _uow.SaveChangesAsync();
                 return Ok(new {info = $"The supplier has been added (supplier ABBR: {sup.SupplierAbbr})."});
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/toggleAvail/")]
        public IActionResult ToggleSupplierAvailability(Guid supplierId)
        {
            try
            {
                Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
                _uow.ShopRepository().ToggleSupplierAvailability(supplier);
                return Ok(new {info = "The supplier availability has been changed."});
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }
        
        [Authorize(Roles = "Admin,Koty,OpRo")]
        [HttpGet("supplier/{supplierId}/toggleBlocked/")]
        public IActionResult ToggleSupplierBlocked(Guid supplierId)
        {
            try
            {
                Supplier supplier= _uow.Repository<Supplier>().GetDetail(s => s.SupplierId == supplierId);
                _uow.ShopRepository().ToggleSupplierBlocked(supplier);
                return Ok(new {info = "The supplier blocked status has been changed."});
        
            }
            catch (Exception e)
            {
                return BadRequest(new {error = e.Message, source = e.Source});
            }
        }

    }
}