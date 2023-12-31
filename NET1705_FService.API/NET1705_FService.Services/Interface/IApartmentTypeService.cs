﻿using NET1705_FService.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1715_FService.Service.Inteface
{
    public interface IApartmentTypeService
    {
        public Task<List<ApartmentTypeModel>> GetAllApartmentTypesAsync(int? buildingId);
    }
}
