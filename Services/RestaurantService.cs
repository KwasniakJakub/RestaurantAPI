﻿using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Authorization;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            

            var restaurant = _dbContext
               .Restaurants
               .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new ResourceOperationsRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            //jeżeli klient wyśle zapytanie do akcji dla usuwania restauracji
            //zanim się ona wykona zostanie zapisana do pliku z logami
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");
            var restaurant = _dbContext
               .Restaurants
               .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");
            
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new ResourceOperationsRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == id);

            //Dto służy do komunikowania się stricte z klientem
            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }
        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                    //Filtrowanie
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                                                           || r.Description.ToLower()
                                                               .Contains(query.SearchPhrase.ToLower())));
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r => r.Name},
                    {nameof(Restaurant.Description), r => r.Description},
                    {nameof(Restaurant.Category), r => r.Category}
                };

                var selectedColumn = columnsSelector[query.SortBy];

                //Sortowanie
                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurant = baseQuery
               .Skip(query.PageSize * (query.PageNumber -1))//Pomijanie określonej liczby elementów
               .Take(query.PageSize)
               .ToList();

            var totalItemsCount = baseQuery.Count();

            //Zmapowane dane bez danych kontaktowych
            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurant);

            var result = new PagedResult<RestaurantDto>(restaurantsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }
        public int Create(CreateRestaurantDto dto)
        {

            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
