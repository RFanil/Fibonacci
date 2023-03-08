# Fibonacci Service 

Edit the `${.env}` file, go to the "Fibonacci-Service\src" folder and run the command: 
```sh
docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml" -p dockercompose7797495582345043695 --ansi never up -d
```

# Experimental branch
**Branch name: (Experimental_branch_for_zero_memory_traffic)**

In this branch, I'm trying to achieve zero memory traffic.
For example, this is a memory graph
```
var integrationEvent = JsonSerializer.Deserialize(context.Message.AsSpan(), eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as NextNumberInFibonacciSequenceCalculatedIntegrationEvent;
```
![alt text](https://raw.githubusercontent.com/RFanil/Fibonacci/main/images/WithoutCustomDeserializer.png)

and this is with a **custom** "Deserializer" that uses hardcoded indexes to parse the json, so not as flexible as **JsonSerializer.Deserialize** but has **zero memory traffic**.

![alt text](https://raw.githubusercontent.com/RFanil/Fibonacci/main/images/WithCustomDeserializer.png)
