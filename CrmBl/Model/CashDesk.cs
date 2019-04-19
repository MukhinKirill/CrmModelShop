using System;
using System.Collections;
using System.Collections.Generic;

namespace CrmBl.Model
{
    public class CashDesk // virtual object. Don't will save in BD
    {
        CrmContext db = new CrmContext();
        public int Number { get; set; }
        public Seller Seller { get; set; }
        public Queue<Cart> Queue { get; set; }
        public int MaxQueueLenght { get; set; }
        public int ExitCustomer { get; set; }//счетчик учета человека в очереди, если больше то покупатель ушел
        public bool IsModel { get; set; }
        public int Count => Queue.Count;
        public event EventHandler<Check> CheckClosed;
        public CashDesk(int number, Seller seller)
        {
            Seller = seller;
            Number = number;
            Queue = new Queue<Cart>();
            IsModel = true;
            MaxQueueLenght = 10;
        }
        public void Enqueue(Cart cart)
        {
            if(Queue.Count <= MaxQueueLenght)
            {
                Queue.Enqueue(cart);
            }
            else
            {
                ExitCustomer++;
            }
        }
        public decimal Dequeue()
        {
            decimal sum = 0;
            if(Queue.Count == 0)
            {
                return 0;
            }
            var cart = Queue.Dequeue();
            if(cart !=null)
            {
                var check = new Check()
                {
                    SellerId = Seller.SellerId,
                    Seller = Seller,
                    CustomerId = cart.Customer.CustomerId,
                    Customer = cart.Customer,
                    Created = DateTime.Now
                };
                if(!IsModel)
                {
                    db.Checks.Add(check);
                    db.SaveChanges();
                }
                else
                {
                    check.CheckId = 0;
                }
                var sells = new List<Sell>();
                foreach (Product product in cart)
                {
                    if (product.Count > 0)
                    {
                        var sell = new Sell()
                        {
                            CheckId = check.CheckId,
                            Check = check,
                            ProductId = product.ProductId,
                            Product = product
                        };


                        sells.Add(sell);
                        if (!IsModel)
                        {
                            db.Sells.Add(sell);//добавляем наши продажи в локальную бд
                        }
                        product.Count--;//по-хорошем тут должна быть транзакция
                        sum += product.Price;
                    }
                }
                check.Price = sum;
                if (!IsModel)
                {
                    db.SaveChanges(); //тепеь кидаем их в рабочую бд
                }
                CheckClosed?.Invoke(this, check);// вызов события с проверкой на подписчиков 
                // или
                //if(CheckClosed!=null)
                //{
                //    CheckClosed(this, check);
                //}
            }
            return sum;
        }
        public override string ToString()
        {
            return $"Касса номер: {Number}";
        }
    }
}
