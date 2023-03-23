
Два приложения общаются друг с другом через транспорт, реализуя расчет чисел Фибоначчи.
**Логика расчета одной последовательности такая:
Первое инициализирует расчет
Первое отправляет второму N(i)
Второе вычисляет N(i+1) = N(i-1) + N(i). Результат расчета передается в первое приложение.
Логика повторяется симметрично
И так до остановки приложений.
**Особенности
**Функциональные
Первое приложение при старте получает параметр – целое число, сколько асинхронных расчетов начать. Все расчеты 
идут параллельно.
Передача данных от 1 к 2 идет через REST WebApi
Передача данных от 2 к 1 идет посредством MessageBus.


# Getting started
Edit the `${.env}` file- set the degree of parallelism, go to the "Fibonacci\src" folder and run the command: 
```sh
docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml" -p dockercompose7797495582345043695 --ansi never up -d
```

# Experimental branch
**Branch name: (Experimental_branch_for_zero_memory_traffic)**

In this branch, I'm trying to achieve a zero  memory traffic.
For example, this is a memory graph of
```
var integrationEvent = JsonSerializer.Deserialize<NextNumberInFibonacciSequenceCalculatedIntegrationEvent>(context.Message.AsSpan(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
```
![alt text](https://raw.githubusercontent.com/RFanil/Fibonacci/main/images/WithoutCustomDeserializer.png)

and this is the memory traffic with a **custom** "Deserializer" that uses hardcoded indexes to parse the json, so not as flexible as **JsonSerializer.Deserialize** but has **zero memory traffic**.

![alt text](https://raw.githubusercontent.com/RFanil/Fibonacci/main/images/WithCustomDeserializer.png)
