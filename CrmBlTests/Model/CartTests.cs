using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrmBl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBl.Model.Tests
{
    [TestClass()]
    public class CartTests
    {
        //модульный тест для проверки корзины
        [TestMethod()]
        public void CartTest()
        {
            //паттерн (разделение кода на три части)
            // arange //подача на вход
            var customer = new Customer()
            {
                CustomerId = 1,
                Name = "testUser"
            };
            var product1 = new Product()
            {
                ProductId = 1,
                Name = "pr1",
                Price = 100,
                Count = 10
            };
            var product2 = new Product()
            {
                ProductId = 2,
                Name = "pr2",
                Price = 200,
                Count = 20
            };
            var cart = new Cart(customer);
            var expectedResult = new List<Product>()
            {
                product1, product1, product2
            };

            // act //действие
            cart.Add(product1);
            cart.Add(product1);
            cart.Add(product2);

            var cartResult = cart.GetAll();
            //assert //сравнение результатов
            Assert.AreEqual(expectedResult.Count, cart.GetAll().Count);
            for (int i = 0; i < expectedResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], cartResult[i]);
            }
        }

    }
}