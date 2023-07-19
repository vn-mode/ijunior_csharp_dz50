using System;
using System.Collections.Generic;

public class Program
{
    private const int DisplayStatusChoice = 1;
    private const int RepairCarChoice = 2;
    private const int ExitChoice = 3;

    private const string DisplayStatusMessage = "1. Показать статус автосервиса";
    private const string RepairCarMessage = "2. Починить автомобиль";
    private const string ExitMessage = "3. Выход";
    private const string UserChoicePrompt = "Выберите действие: ";
    private const string NoCarsMessage = "Нет автомобилей для ремонта.";
    private const string WrongChoiceMessage = "Неправильный выбор. Попробуйте снова.";
    private const string WrongInputMessage = "Неправильный ввод. Пожалуйста, введите номер действия.";

    static void Main(string[] args)
    {
        CarService carService = new CarService(50000m);

        Part part1 = new Part("Деталь 1", 1000m, 500m);
        Part part2 = new Part("Деталь 2", 2000m, 1000m);

        carService.AddInventoryItem(part1, 10);
        carService.AddInventoryItem(part2, 5);

        Car car1 = new Car(part1, 500m);
        Car car2 = new Car(part2, 1000m);

        List<Car> cars = new List<Car> { car1, car2 };

        bool isWorking = true;
        while (isWorking)
        {
            Console.WriteLine();
            Console.WriteLine(DisplayStatusMessage);
            Console.WriteLine(RepairCarMessage);
            Console.WriteLine(ExitMessage);
            Console.Write(UserChoicePrompt);

            string input = Console.ReadLine();
            int choice;
            bool isNumber = Int32.TryParse(input, out choice);

            if (isNumber)
            {
                switch (choice)
                {
                    case DisplayStatusChoice:
                        carService.DisplayStatus();
                        break;
                    case RepairCarChoice:
                        if (cars.Count > 0)
                        {
                            carService.RepairCar(cars[0]);
                            cars.RemoveAt(0);
                        }
                        else
                        {
                            Console.WriteLine(NoCarsMessage);
                        }
                        break;
                    case ExitChoice:
                        isWorking = false;
                        break;
                    default:
                        Console.WriteLine(WrongChoiceMessage);
                        break;
                }
            }
            else
            {
                Console.WriteLine(WrongInputMessage);
            }
        }
    }
}

public class CarService
{
    private const string OutOfStockMessage = "Деталь не в наличии. Клиенту отказано.";
    private const string RepairCompletedMessage = "Ремонт завершен. Получено {0} рублей.";
    private const string CurrentBalanceMessage = "Текущий баланс автосервиса: {0} рублей";
    private const string PartsInInventoryMessage = "Детали на складе:";
    private const string PartDetailMessage = "Название: {0}, Количество: {1}, Стоимость: {2} рублей";

    private decimal _balance;
    private List<InventoryItem> _inventory;

    public CarService(decimal initialBalance)
    {
        _balance = initialBalance;
        _inventory = new List<InventoryItem>();
    }

    public void AddInventoryItem(Part part, int quantity)
    {
        _inventory.Add(new InventoryItem(part, quantity));
    }

    public void RepairCar(Car car)
    {
        var requiredPart = car.BrokenPart;
        var inventoryItem = _inventory.Find(item => item.Part.Name == requiredPart.Name);

        if (inventoryItem == null || inventoryItem.Quantity == 0)
        {
            Console.WriteLine(OutOfStockMessage);
            _balance -= requiredPart.PenaltyForNoPart;
        }
        else
        {
            _balance += car.RepairCost;
            Console.WriteLine(String.Format(RepairCompletedMessage, car.RepairCost));
            inventoryItem.Quantity--;
        }
    }

    public void DisplayStatus()
    {
        Console.Clear();
        Console.WriteLine(String.Format(CurrentBalanceMessage, _balance));
        Console.WriteLine(PartsInInventoryMessage);

        foreach (var item in _inventory)
        {
            Console.WriteLine(String.Format(PartDetailMessage, item.Part.Name, item.Quantity, item.Part.Cost));
        }
    }
}

public class Car
{
    public Part BrokenPart { get; private set; }
    public decimal RepairCost { get; private set; }

    public Car(Part brokenPart, decimal laborCost)
    {
        BrokenPart = brokenPart;
        RepairCost = brokenPart.Cost + laborCost;
    }
}

public class Part
{
    public string Name { get; private set; }
    public decimal Cost { get; private set; }
    public decimal PenaltyForNoPart { get; private set; }

    public Part(string name, decimal cost, decimal penaltyForNoPart)
    {
        Name = name;
        Cost = cost;
        PenaltyForNoPart = penaltyForNoPart;
    }
}

public class InventoryItem
{
    public Part Part { get; private set; }
    public int Quantity { get; set; }

    public InventoryItem(Part part, int quantity)
    {
        Part = part;
        Quantity = quantity;
    }
}
