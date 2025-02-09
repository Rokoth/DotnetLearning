// See https://aka.ms/new-console-template for more information
//+ 1. Создать классы, описывающие структуру каждой из приведённых выше таблиц
//2. Создать экзепляры объектов из таблицы. Вы можете создавать объекты с помощью new, прямо в коде определив значения из приложенных таблиц.
//   По желанию, можно реализовать чтение и десериализацию из json-файлов (потребуется продумать структуру и подготовить json файлы с указанными в таблицах данными)
//3. Реализовать функции, возвращающие результат согласно комментариям
//- если Вы начинающий программист, используйте циклы, условные операторы и оператор switch
//- eсли Вы больше уверены в своих навыках, используйте LINQ для более лаконичных запросов при фильтрации, выборе или суммировании значений
//4. Осуществить вывод в консоль всех резервуаров, включая имена цеха и фабрики, в которых они числятся
//5. Вывести общую сумму загрузки всех резервуаров
//6. * Осуществить возможность поиска по наименованию в коллекции, например через ввод в консоли
//7. ** Придумать структуру и выгрузить все объекты в json файл
//8. *** Считать данные таблиц Excel напрямую, используя любую библиотеку


using Lesson1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        var tanks = GetTanks();
        var units = GetUnits();
        var factories = GetFactories();
        Console.WriteLine($"Количество резервуаров: {tanks.Length}, установок: {units.Length}");

        var foundUnit = FindUnit(units, tanks, "Резервуар 2");
        var factory = FindFactory(factories, foundUnit);

        Console.WriteLine($"Резервуар 2 принадлежит установке {foundUnit?.Name} и заводу {factory?.Name}");

        var totalVolume = GetTotalVolume(tanks);
        Console.WriteLine($"Общий объем резервуаров: {totalVolume}");

        Console.WriteLine("  {tank.tank.Id}  {tank.tank.Name}  {tank.tank.Description}  {tank.tank.Volume}  {tank.tank.MaxVolume}  {tank.unit.Name}  {tank.factory.Name}");
       
        foreach (var tank in tanks
        .Join(units, s=>s.UnitId, s=>s.Id, (tank, unit) => new { tank, unit})
        .Join(factories, s=>s.unit.FactoryId, s=>s.Id, (model, factory) => new { model.tank, model.unit, factory }))
        {
            Console.WriteLine($"  {tank.tank.Id}  {tank.tank.Name}  {tank.tank.Description}  {tank.tank.Volume}  {tank.tank.MaxVolume}  {tank.unit.Name}  {tank.factory.Name}");
        }

        Console.WriteLine();
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Введите режим работы: 1 - поиск резервура, 2 - поиск цеха, 3 - поиск фабрики, 0 - выход");
            var key = Console.ReadKey();
            
            switch (key.Key)
            {
                case ConsoleKey.D0:
                    exit = true;
                    break;
                case ConsoleKey.D1:
                    var tankName = GetStringToFind();
                    var findedTanks = FindTank(tanks, tankName);
                    Console.WriteLine($"Найдено {findedTanks.Count()} записей:");
                    foreach (var tank in findedTanks)
                    {
                        Console.WriteLine($"{tank.Id}  {tank.Name}  {tank.Description}  {tank.Volume}  {tank.MaxVolume}");
                    }
                    break;
                case ConsoleKey.D2:
                    var unitName = GetStringToFind();
                    var findedUnits = FindUnit(units, unitName);
                    Console.WriteLine($"Найдено {findedUnits.Count()} записей:");
                    foreach (var unit in findedUnits)
                    {
                        Console.WriteLine($"{unit.Id}  {unit.Name}  {unit.Description}");
                    }
                    break;
                case ConsoleKey.D3:
                    var factoryName = GetStringToFind();
                    var findedFactory = FindFactory(factories, factoryName);
                    Console.WriteLine($"Найдено {findedFactory.Count()} записей:");
                    foreach (var unit in findedFactory)
                    {
                        Console.WriteLine($"{unit.Id}  {unit.Name}  {unit.Description}");
                    }
                    break;
            }
        }
    }

    private static string? GetStringToFind()
    {
        Console.WriteLine("Введите строку поиска: ");
        return Console.ReadLine();
    }

    // реализуйте этот метод, чтобы он возвращал массив резервуаров, согласно приложенным таблицам
    // можно использовать создание объектов прямо в C# коде через new, или читать из файла (на своё усмотрение)
    public static Tank[] GetTanks()
    {
        using var reader = new StreamReader("Data\\tanks.json");
        return JsonConvert.DeserializeObject<Tank[]>(reader.ReadToEnd()) ?? Array.Empty<Tank>();   
    }
    // реализуйте этот метод, чтобы он возвращал массив установок, согласно приложенным таблицам
    public static Unit[] GetUnits()
    {
        using var reader = new StreamReader("Data\\units.json");
        return JsonConvert.DeserializeObject<Unit[]>(reader.ReadToEnd()) ?? Array.Empty<Unit>();
    }
    // реализуйте этот метод, чтобы он возвращал массив заводов, согласно приложенным таблицам
    public static Factory[] GetFactories()
    {
        using var reader = new StreamReader("Data\\factories.json");
        return JsonConvert.DeserializeObject<Factory[]>(reader.ReadToEnd()) ?? Array.Empty<Factory>();
    }

    // реализуйте этот метод, чтобы он возвращал установку (Unit), которой
    // принадлежит резервуар (Tank), найденный в массиве резервуаров по имени
    // учтите, что по заданному имени может быть не найден резервуар
    public static Unit? FindUnit(Unit[] units, Tank[] tanks, string unitName)
    {
        return units.FirstOrDefault(s=>s.Id == tanks.FirstOrDefault(s=>s.Name == unitName)?.UnitId);
    }

    // реализуйте этот метод, чтобы он возвращал объект завода, соответствующий установке
    public static Factory? FindFactory(Factory[] factories, Unit? unit)
    {
        if(unit != null)
            return factories.FirstOrDefault(s=>s.Id == unit?.FactoryId);
        return null;
    }

    // реализуйте этот метод, чтобы он возвращал суммарный объем резервуаров в массиве
    public static decimal GetTotalVolume(Tank[] units)
    {
        return units.Sum(s=>s.Volume);
    }

    public static IEnumerable<Unit> FindUnit(Unit[] units, string? unitName)
    {
        return units.Where(s => s.Name.Contains(unitName ?? "", StringComparison.InvariantCultureIgnoreCase));
    }

    public static IEnumerable<Tank> FindTank(Tank[] tanks, string? unitName)
    {
        return tanks.Where(s => s.Name.Contains(unitName ?? "", StringComparison.InvariantCultureIgnoreCase));
    }

    public static IEnumerable<Factory> FindFactory(Factory[] factories, string? unitName)
    {
        return factories.Where(s => s.Name.Contains(unitName ?? "", StringComparison.InvariantCultureIgnoreCase));
    }
}