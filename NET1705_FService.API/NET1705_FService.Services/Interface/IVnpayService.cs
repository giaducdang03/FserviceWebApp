﻿using Microsoft.AspNetCore.Http;
using NET1705_FService.Repositories.Data;
using NET1705_FService.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1705_FService.Services.Interface
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(Order model, HttpContext context, string vnp_OrderInfo, string callBackUrl);
        Task<bool> PaymentExecute(VnpayModel vnpayModel);
    }
}
