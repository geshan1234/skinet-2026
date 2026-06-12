using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Core.Entities;

namespace Core.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Maps incoming client data to your database entity
            // AutoMapper ignores 'Id' because it doesn't exist on the DTO
            CreateMap<ProductCreationDto, Product>();

            // Maps database entity back to a read-only DTO for the client
            CreateMap<Product, ProductCreationDto>(); 
        }
    }
}