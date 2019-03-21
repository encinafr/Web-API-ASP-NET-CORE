using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NorthwindApiDemo.EFModels;
using NorthwindApiDemo.Models;
using NorthwindApiDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindApiDemo.Controllers
{
    //api/customers/23/orders/1
    [Route("api/customers")]
    public class OrdersController : Controller
    {
        private ICustomerRepository _customerRepository;

        public OrdersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet("{customerId}/orders")]
        public IActionResult GetOrders(string customerId)
        {
            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }
            var orders = _customerRepository.GetOrders(customerId);

            var ordersResult = Mapper.Map<IEnumerable<OrdersDTO>>(orders);

            //var customer = Repository.Instance.Customers.FirstOrDefault(c => c.Id == customerId);
            //if(customer == null)
            //{
            //    return NotFound();
            //}
            return Ok(ordersResult);
        }

        [HttpGet("{customerId}/orders/{id}",Name = "GetOrder")]
        public IActionResult GetOrder(string customerId, int id)
        {

            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }


            var order = _customerRepository.GetOrder(customerId, id);

            if(order == null)
            {
                return NotFound();
            }

            var orderResult = Mapper.Map<OrdersDTO>(order);

            return Ok(orderResult);
        }


        [HttpPost("{customerId}/orders")]
        public IActionResult CreateOrder(string customerId, [FromBody]OrdersForCreationDTO order)// FromBody desde el cuerpo de la petición
        {
            if(order == null)
            {
                return BadRequest();//solictud erronea
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }

            var finalOrder = Mapper.Map<Orders>(order);//como vamos a enviar datos ala db mapeamos order al dbset Orders

            _customerRepository.AddOrder(customerId, finalOrder);

            if(!_customerRepository.Save())
            {
                return StatusCode(500, "Please verify your data");
            }

            var customerCreated = Mapper.Map<OrdersDTO>(finalOrder);

            return CreatedAtRoute("GetOrder",
               new
               {
                   //Parametros del metodo GetOrder
                   customerId = customerId,
                   id = customerCreated.OrderId
               }, customerCreated);
        }

        [HttpPut("{customerId}/orders/{id}")]
        public IActionResult UpdateOrder(string customerId, int id,[FromBody] OrdersForUpdateDTO order)
        {
            if (order == null)
            {
                return BadRequest();//solictud erronea
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }

            var existingOrder = _customerRepository.GetOrder(customerId, id);

            if(existingOrder == null)
            {
                return NotFound();
            }

            Mapper.Map(order, existingOrder);//El primer parametro es la nueva información y el segundo es el registro que teniamos anteriormente

            if (!_customerRepository.Save())
            {
                return StatusCode(500, "Please verify your data");
            }

           

            return NoContent();

        }

        [HttpPatch("{customerId}/orders/{id}")]
        public IActionResult UpdateOrder(string customerId, int id,
          [FromBody] JsonPatchDocument<OrdersForUpdateDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();//solictud erronea
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }

            var existingOrder = _customerRepository.GetOrder(customerId, id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            var orderToUpdate = Mapper.Map<OrdersForUpdateDTO>(existingOrder);

            patchDocument.ApplyTo(orderToUpdate, ModelState);//indicamos la instancia que queremos actualizar

            TryValidateModel(orderToUpdate);//validación del modelo actual

            if(!ModelState.IsValid)//Validación del modelo actual
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(orderToUpdate, existingOrder);

            if (!_customerRepository.Save())
            {
                return StatusCode(500, "Please verify your data");
            }

            return NoContent();

        }

        [HttpDelete("{customerId}/orders/{id}")]
        public ActionResult DeleteOrder(string customerId, int id)
        {
            if (!_customerRepository.CustomerExists(customerId))
            {
                return NotFound();
            }

            var existingOrder = _customerRepository.GetOrder(customerId, id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            _customerRepository.DeleteOrder(existingOrder);

            if (!_customerRepository.Save())
            {
                return StatusCode(500, "Please verify your data");
            }

            return NoContent();
        }
    }
}
