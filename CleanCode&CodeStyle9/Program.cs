using System;
using System.Collections.Generic;

namespace IMJunior
{
    class Program
    {
        static void Main(string[] args)
        {
            IPaymentSystemRespository paymentSystemRespository = new PaymentSystemRespository();
            OrderForm orderForm = new OrderForm(paymentSystemRespository.GetSystemFactoryIdNames());

            orderForm.ShowForm();

            string systemId = orderForm.ChoosePaymentSystemId();
            IPaymentSystemFactory paymentSystemFactory = paymentSystemRespository.FindFactory(systemId);
            PaymentHandler paymentHandler = new PaymentHandler(paymentSystemFactory);

            paymentHandler.AcceptPayment();
            paymentHandler.ShowPaymentResult();
        }
    }

    public static class PaymentSystemId
    {
        public const string Qiwi = "QIWI";
        public const string WebMoney = "WebMoney";
        public const string Card = "Card";
    }

    public interface IPaymentSystemRespository
    {
        IPaymentSystemFactory FindFactory(string systemId);

        IEnumerable<string> GetSystemFactoryIdNames();
    }

    public class PaymentSystemRespository : IPaymentSystemRespository
    {
        private Dictionary<string, IPaymentSystemFactory> _factories;

        public PaymentSystemRespository()
        {
            _factories = CreateFactories();
        }

        public IPaymentSystemFactory FindFactory(string systemId)
        {
            if (systemId == null)
                throw new ArgumentNullException(nameof(systemId));

            if (systemId.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(systemId));

            systemId = FindPaymentSystemIdName(systemId);

            return _factories[systemId];
        }

        public IEnumerable<string> GetSystemFactoryIdNames()
        {
            return _factories.Keys;
        }

        private Dictionary<string, IPaymentSystemFactory> CreateFactories()
        {
            return new Dictionary<string, IPaymentSystemFactory>
            {
                { PaymentSystemId.Qiwi, new QiwiPaymentFactory() },
                { PaymentSystemId.WebMoney, new WebMoneyPaymentFactory() },
                { PaymentSystemId.Card, new CardPaymentFactory() }
            };
        }

        private string FindPaymentSystemIdName(string systemId)
        {
            systemId = systemId.ToLower();

            foreach (string systemName in _factories.Keys)
            {
                if (systemName.ToLower() == systemId)
                    return systemName;
            }

            throw new InvalidOperationException($"The '{systemId}' payment system was not found.");
        }
    }

    public class OrderForm
    {
        private IEnumerable<string> _paymentSystemIds;

        public OrderForm(IEnumerable<string> paymentSystemIds) => 
            _paymentSystemIds = paymentSystemIds ?? throw new ArgumentNullException(nameof(paymentSystemIds));

        public void ShowForm()
        {
            string form = "Мы принимаем: ";

            foreach (string paymentSystemId in _paymentSystemIds)
                form += $"{paymentSystemId}, ";

            Console.WriteLine(form);                      
        }

        public string ChoosePaymentSystemId()
        {
            Console.WriteLine("Какое системой вы хотите совершить оплату?");

            return Console.ReadLine();
        }
    }    

    public class PaymentHandler
    {
        private IPaymentSystem _paymentSystem;

        public PaymentHandler(IPaymentSystemFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _paymentSystem = factory.Create();
        }

        public void AcceptPayment() => 
            _paymentSystem.AcceptPayment();

        public void ShowPaymentResult()
        {
            Console.WriteLine($"Вы оплатили с помощью {_paymentSystem.Id}");

            _paymentSystem.CheckPayment();

            Console.WriteLine("Оплата прошла успешно!");
        }
    }

    public class CardPaymentFactory : IPaymentSystemFactory
    {
        public IPaymentSystem Create() =>
            new CardPaymentSystem();
    }

    public class CardPaymentSystem : IPaymentSystem
    {
        public CardPaymentSystem() => Id = PaymentSystemId.Card;

        public string Id { get; private set; }

        public void AcceptPayment() =>
            Console.WriteLine("Вызов API банка эмитера карты Card...");

        public void CheckPayment() => 
            Console.WriteLine("Проверка платежа через Card...");
    }

    public class WebMoneyPaymentFactory : IPaymentSystemFactory
    {
        public IPaymentSystem Create() => 
            new WebMoneyPaymentSystem();
    }

    public class WebMoneyPaymentSystem : IPaymentSystem
    {
        public WebMoneyPaymentSystem() => 
            Id = PaymentSystemId.WebMoney;

        public string Id { get; private set; }

        public void AcceptPayment() => 
            Console.WriteLine("Вызов API WebMoney...");

        public void CheckPayment() => 
            Console.WriteLine("Проверка платежа через WebMoney...");
    }

    public class QiwiPaymentFactory : IPaymentSystemFactory
    {
        public IPaymentSystem Create() => 
            new QiwiPaymentSystem();
    }

    public class QiwiPaymentSystem : IPaymentSystem
    {
        public QiwiPaymentSystem() => 
            Id = PaymentSystemId.Qiwi;

        public string Id { get; private set; }

        public void CheckPayment() => 
            Console.WriteLine("Проверка платежа через QIWI...");

        public void AcceptPayment() => 
            Console.WriteLine("Перевод на страницу QIWI...");
    }    

    public interface IPaymentSystemFactory
    {
        IPaymentSystem Create();
    }

    public interface IPaymentSystem
    {
        string Id { get; }

        void CheckPayment();

        void AcceptPayment();
    }
}
