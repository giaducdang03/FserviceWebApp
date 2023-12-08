﻿using Microsoft.Extensions.Options;
using NET1705_FService.API.RunSchedule.Job;
using Quartz;

namespace NET1705_FService.API.RunSchedule.Setup
{
    public class ScanningApartmentPackageSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobkey = JobKey.Create(nameof(ScanningApartmentPackage));
            options.AddJob<ScanningApartmentPackage>(JobBuilder => JobBuilder.WithIdentity(jobkey))
            .AddTrigger(trigger =>
                trigger.ForJob(jobkey).WithCronSchedule("0 * * * * ?"));
        }
    }
}
