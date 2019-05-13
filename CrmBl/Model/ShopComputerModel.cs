using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrmBl.Model
{
    public class ShopComputerModel
    {
        Generator generator = new Generator();
        Random rnd = new Random();

        CancellationTokenSource cancellationTokenSource;
        CancellationToken token;
        TaskFactory TaskFactory;
        List<Task> tasks = new List<Task>();
        public List<CashDesk> CashDesks { get; set; } = new List<CashDesk>();
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public List<Check> Checks { get; set; } = new List<Check>();
        public List<Sell> Sells { get; set; } = new List<Sell>();
        public Queue<Seller> Sellers { get; set; } = new Queue<Seller>();
        public int CustomerSpeed { get; set; } = 100;
        public int CashDeskSpeed { get; set; } = 100;
        public ShopComputerModel()
        {
            var sellers = generator.GetNewSellers(20);
            generator.GetNewProducts(1000);
            generator.GetNewCustomers(100);

            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            TaskFactory = new TaskFactory(token);
            foreach (var seller in sellers)
            {
                Sellers.Enqueue(seller);
            }
            for (int i = 0; i < 3; i++)
            {
                CashDesks.Add(new CashDesk(CashDesks.Count, Sellers.Dequeue(), null));
            }
        }
        //public async void Start()
        public void Start()
        {
            //tasks.Add(Task.Run(() => CreateCarts(10, token)));
            tasks.Add(Task.Factory.StartNew(() => CreateCarts(10, token)));
            //await Task.Run(() => CreateCarts(10,1000)); 
            // если будут await/async выполнение метода будет останавливаться  на данном этапе и ждать выполнения метода в отдельном аснхронном потоке
            //т.е. основной поток будет ждать выполнения Task'a
            // Пример: мб использовано при работе с графическим интерфейсом, чтобы форма не зависала на время ожидания работы порожденого потока
            // форма не будет активна, но и не будет висеть мертвым грузом

            //var cashDeskTasks = CashDesks.Select(c => new Task(() => CashDeskWork(c, token)));
            //foreach (var task in cashDeskTasks)
            //{
            //    task.Start();
            //}
            foreach (var cd in CashDesks)
            {
                tasks.Add(Task.Factory.StartNew(() => CashDeskWork(cd, token)));

            }
           // tasks.AddRange(cashDeskTasks);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            Thread.Sleep(100);
            var tmp = TaskFactory.CancellationToken;
        }

        private void CashDeskWork(CashDesk cashDesk, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (cashDesk.Count > 0)
                {
                    cashDesk.Dequeue();
                    Thread.Sleep(CashDeskSpeed);
                }
            }
        }

        private void CreateCarts(int customerCounts, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {

                var customers = generator.GetNewCustomers(10);
                var carts = new Queue<Cart>();

                foreach (var customer in customers)
                {
                    var cart = new Cart(customer);
                    foreach (var product in generator.GetRandomProducts(10,30))
                    {
                        cart.Add(product);
                    }
                    var cash = CashDesks[rnd.Next(CashDesks.Count)];
                    cash.Enqueue(cart);
                }
                Thread.Sleep(CustomerSpeed);
            }
        }
    }
}
