using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NorthwindApiDemo.Models;
using NorthwindApiDemo.Services;

namespace NorthwindApiDemo.Controllers
{
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet()]
        public IActionResult GetCustomer()
        {
            //return new JsonResult(Repository.Instance.Customers);
            ////    {
            ////        new { CustomerID = 1 ,ContactName = "Anderson"},
            ////         new { CustomerID = 2 ,ContactName = "Pepe"},
            ////    });
            ////}
            ///
            var customers = _customerRepository.GetCustomers();
            var results = Mapper.Map < IEnumerable<CustomerWithoutOrders>>(customers);//Mapeo de customers a CustomerWithoutOrders

            return new JsonResult(results);
        }

        [HttpGet("{id}")]//El nombre debe ser igual al parametro del metodo
        public IActionResult GetCustomer(string id, bool includeOrders = false)
        {
            //var result = Repository.Instance.Customers
            //            .FirstOrDefault(c => c.Id == id);
            var customer = _customerRepository.GetCustomer(id, includeOrders);

            if (customer == null)
            {
                return NotFound();//Retorno un 404
            }

            if (includeOrders)
            {
                var customerResult =
                    Mapper.Map<CustomerDTO>(customer);//hago el mapeo ya que CustomerDTO contiene la propiedad que devuele todas las ordenes
                return Ok(customerResult);
            }

            var customerResultOnly = Mapper.Map<CustomerWithoutOrders>(customer);

            return Ok(customerResultOnly);
        }
    }
}
