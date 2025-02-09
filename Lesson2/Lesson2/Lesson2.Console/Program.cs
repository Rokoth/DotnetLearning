// See https://aka.ms/new-console-template for more information
using Services;

Console.WriteLine("Hello, World!");

var inn = "7713076301";
var dataService = new DadataService();
var result = await dataService.GetSuggestionAsync(inn);

Console.WriteLine(result?.Suggestions?.FirstOrDefault()?.value);

