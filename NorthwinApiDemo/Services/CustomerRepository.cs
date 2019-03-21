using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindApiDemo.EFModels;

namespace NorthwindApiDemo.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private NorthwindContext _context;

        public CustomerRepository(NorthwindContext context)
        {
            _context = context;
        }

        public IEnumerable<Customers> GetCustomers()
        {
            return _context.Customers.OrderBy(c => c.CompanyName).ToList();
        }

        public Customers GetCustomer(string customerId, bool includeOrders)
        {
            if(includeOrders)
            {
                return _context.Customers
                    .Include(c => c.Orders)
                    .Where(c => c.CustomerId == customerId)
                    .FirstOrDefault();
            }

            return _context.Customers
                      .Where(c => c.CustomerId == customerId)
                      .FirstOrDefault();
        }

        public Orders GetOrder(string customerId, int orderId)
        {
            return _context.Orders
                 .Where(c => c.CustomerId == customerId && c.OrderId == orderId)
                 .FirstOrDefault();
        }

        public IEnumerable<Orders> GetOrders(string customerId)
        {
            return _context.Orders
                .Where(c => c.CustomerId == customerId)
                .ToList();
        }

        public bool CustomerExists(string customerId)
        {
            return _context.Customers.Any(c => c.CustomerId == customerId);
        }

        public void AddOrder(string customerId, Orders order)
        {
            var customer = GetCustomer(customerId, false);
            customer.Orders.Add(order);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);//Si se actualizan 1 o mas registros esto es true
        }

        public void DeleteOrder(Orders order)
        {
            _context.Orders
                  .Remove(order);
        }
    }
}
