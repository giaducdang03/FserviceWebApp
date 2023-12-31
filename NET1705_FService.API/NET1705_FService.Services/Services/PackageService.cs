﻿using NET1705_FService.Repositories.Data;
using NET1705_FService.Repositories.Helper;
using NET1705_FService.Repositories.Models;
using NET1715_FService.API.Repository.Inteface;
using NET1715_FService.Service.Inteface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1715_FService.Service.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _repo;

        public PackageService(IPackageRepository repo)
        {
            _repo = repo;
        }
        public async Task<ResponseModel> AddPackageAsync(Package package)
        {
            var result = await _repo.AddPackageAsync(package);
            if (result != 0)
            {
                return new ResponseModel { Status = "Success", Message = result.ToString() };
            }
            return new ResponseModel { Status = "Error", Message = "Error! Try again." };
        }

        public async Task<ResponseModel> DeletePackageAsync(int id)
        {
            var typeId = 1;
            var deletePackage = await _repo.GetPackageAsync(id, typeId);
            if (deletePackage == null)
            {
                return new ResponseModel { Status = "Error", Message = $"Not found Package Id {id}" };
            }
            var result = await _repo.DeletePackageAsync(id);
            if (result == 0)
            {
                return new ResponseModel { Status = "Error", Message = $"Can not delete package {deletePackage.Name}" };
            }
            return new ResponseModel { Status = "Success", Message = $"Delete successfully package {deletePackage.Name}" };
        }

        public async Task<PagedList<Package>> GetAllPackagesAsync(PaginationParameter paginationParameter)
        {
            var packages = await _repo.GetAllPackagesAsync(paginationParameter);
            return packages;
        }

        public async Task<Package> GetPackageAsync(int id, int typeId)
        {
            var package = await _repo.GetPackageAsync(id, typeId);
            return package;
        }

        public async Task<ResponseModel> UpdatePackageAsync(int id, Package package)
        {
            if (id == package.Id)
            {
                var result = await _repo.UpdatePackageAsync(id, package);
                if (result != 0)
                {
                    return new ResponseModel { Status = "Success", Message = result.ToString() };
                }
                return new ResponseModel { Status = "Error", Message = "Update error." };
            }
            return new ResponseModel { Status = "Error", Message = "Id invalid" };
        }
    }
}
