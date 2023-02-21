using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    //[Authorize] // Wszystkie wywołania będą musiały zostać zautoryzowane
    //api controller sprawi że dla każdego zapytania u którego
    //istnieje walidacja zostanie automatycznie wykonany poniższy kawałek kodu
    //if(!ModelState.IsValid)
    //   {
    //   return BadRequest(ModelState);
    //   }
public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService= restaurantService;
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            
             _restaurantService.Update(id, dto);
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute]int id)
        {
            _restaurantService.Delete(id);

            return NoContent();
        }

        [HttpPost]
        //[Authorize(Roles = "Manager,Admin")]
        //Tylko użytkownicy o roli admin lub manager będą mogli wywołać tą metode
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }
        
        [HttpGet]
        //[Authorize(Policy = "CreatedAtleast2Restaurants")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery]RestaurantQuery query)
        {
            var restaurantsDtos = _restaurantService.GetAll(query);

            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        //[AllowAnonymous]//Omijanie autoryzacji 
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);           

            return Ok(restaurant);
        }
    }
}
