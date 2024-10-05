using System;
using System.Collections.Generic;

namespace IMJunior
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderForm orderForm = new OrderForm();
            PaymentHandler paymentHandler = new PaymentHandler();
            IPaymentSystem chosenPaymentSystem = null;

            List<IPaymentSystem> paymentSystems = new List<IPaymentSystem>
            {
                new Qiwi("QIWI", "Перевод на страницу QIWI...", "Проверка платежа через QIWI..."),
                new WebMoney("WebMoney", "Вызов API WebMoney...", "Проверка платежа через WebMoney..."),
                new Card("Card", "Вызов API банка эмитера карты Card...", "Проверка платежа через Card...")
            };

            string systemId = orderForm.ShowForm();

            foreach (IPaymentSystem paymentSystem in paymentSystems)
            {
                if (paymentSystem.Id.ToLower() == systemId.ToLower())
                {
                    chosenPaymentSystem = paymentSystem;

                    paymentSystem.ShowProcess(paymentSystem.TransactionBeginning);

                    break;
                }
            }

            if (chosenPaymentSystem == null)
                throw new ArgumentNullException(nameof(chosenPaymentSystem));

            paymentHandler.ShowPaymentResult(chosenPaymentSystem);
        }
    }

    public class OrderForm
    {
        public string ShowForm()
        {
            Console.WriteLine($"Мы принимаем: {nameof(Qiwi)}, {nameof(WebMoney)}, {nameof(Card)}");

            //симуляция веб интерфейса
            Console.WriteLine("Какое системой вы хотите совершить оплату?");
            return Console.ReadLine();
        }
    }

    public class PaymentHandler
    {
        public void ShowPaymentResult(IPaymentSystem paymentSystem)
        {
            Console.WriteLine($"Вы оплатили с помощью {paymentSystem.Id}");
            Console.WriteLine($"{paymentSystem.TransactionEnding}\n");

            Console.WriteLine("Оплата прошла успешно!"); 
        }
    }

    public class Card : IPaymentSystem
    {
        public Card(string id, string transactionBeginning, string transactionEnding)
        {
            Id = id;
            TransactionBeginning = transactionBeginning;
            TransactionEnding = transactionEnding;
        }

        public string Id { get; private set; }

        public string TransactionBeginning { get; private set; }

        public string TransactionEnding { get; private set; }

        public void ShowProcess(string process) => 
            Console.WriteLine(process);
    }

    public class WebMoney : IPaymentSystem
    {
        public WebMoney(string id, string transactionBeginning, string transactionEnding)
        {
            Id = id;
            TransactionBeginning = transactionBeginning;
            TransactionEnding = transactionEnding;
        }

        public string Id { get; private set; }

        public string TransactionBeginning { get; private set; }

        public string TransactionEnding { get; private set; }

        public void ShowProcess(string process) => 
            Console.WriteLine(process);
    }

    public class Qiwi : IPaymentSystem
    {
        public Qiwi(string id, string transactionBeginning, string transactionEnding)
        {
            Id = id;
            TransactionBeginning = transactionBeginning;
            TransactionEnding = transactionEnding;
        }

        public string Id { get; private set; }

        public string TransactionBeginning { get; private set; }

        public string TransactionEnding { get; private set; }

        public void ShowProcess(string process) => 
            Console.WriteLine(process);
    }

    public interface IPaymentSystem
    {
        string Id { get; }
        string TransactionBeginning { get; }
        string TransactionEnding { get; }

        void ShowProcess(string process);
    }
}
