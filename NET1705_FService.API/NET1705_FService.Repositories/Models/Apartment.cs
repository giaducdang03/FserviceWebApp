﻿using System;
using System.Collections.Generic;

namespace NET1705_FService.Repositories.Models;

public partial class Apartment
{
    public int Id { get; set; }

    public int? FloorId { get; set; }

    public string RoomNo { get; set; }

    public int TypeId { get; set; }

    public string? AccountId { get; set; }

    public virtual ICollection<ApartmentPackage> ApartmentPackages { get; } = new List<ApartmentPackage>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual Accounts Account { get; set; }

    public virtual Floor Floor { get; set; }

    public virtual ApartmentType Type { get; set; }
}

public class ApartmentModel
{
    public int Id { get; set; }

    public int? FloorId { get; set; }

    public string RoomNo { get; set; }

    public int TypeId { get; set; }

    public string? AccountId { get; set; }

    public virtual ApartmentType Type { get; set; }
}
