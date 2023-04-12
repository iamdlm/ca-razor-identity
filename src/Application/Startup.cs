﻿using Application.Interfaces.Persistence;
using Application.Mapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class Startup
    {
        public static void AddApplicationMappingProfiles(this IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            services.AddSingleton(mappingConfig.CreateMapper());
        }
    }
}
