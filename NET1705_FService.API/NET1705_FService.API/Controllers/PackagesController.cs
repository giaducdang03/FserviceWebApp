﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NET1705_FService.Repositories.Models;
using NET1715_FService.Service.Inteface;

namespace NET1705_FService.API.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;
        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return Ok(packages);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageAsync(int id)
        {
            try
            {
                var package = await _packageService.GetPackageAsync(id);
                return package != null ? Ok(package) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddPackageAsync(Package newPackage)
        {
            try
            {
                var result = await _packageService.AddPackageAsync(newPackage);
                if (result.Status.Equals("Success"))
                {
                    var package = await _packageService.GetPackageAsync(int.Parse(result.Message));
                    return Ok(package);
                }
                return BadRequest(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackageAsync(int id, Package updatePackage)
        {
            try
            {
                var result = await _packageService.UpdatePackageAsync(id, updatePackage);
                if (result.Status.Equals("Success"))
                {
                    var package = await _packageService.GetPackageAsync(int.Parse(result.Message));
                    return Ok(package);
                }
                return NotFound(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackageAsync(int id)
        {
            try
            {
                var result = await _packageService.DeletePackageAsync(id);
                if (result.Status.Equals("Success"))
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
