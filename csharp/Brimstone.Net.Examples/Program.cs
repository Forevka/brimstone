using Brimstone.Net;

namespace Brimstone.Net.Examples;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine("Brimstone.Net Examples");
        Console.WriteLine($"Version: {BrimstoneEngine.Version}");
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine();

        // Example 1: Basic JavaScript execution
        Example1_BasicExecution();

        // Example 2: Arithmetic and variables
        Example2_Arithmetic();

        // Example 3: Working with arrays
        Example3_Arrays();

        // Example 4: Working with objects
        Example4_Objects();

        // Example 5: Functions and closures
        Example5_Functions();

        // Example 6: Using a simple JS library (math utilities)
        Example6_JsLibrary();

        // Example 7: JSON processing
        Example7_JsonProcessing();

        // Example 8: Error handling
        Example8_ErrorHandling();

        // Example 9: Loading and running an external JS file
        Example9_ExternalFile();

        Console.WriteLine();
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine("All examples completed successfully!");
        Console.WriteLine("=".PadRight(60, '='));
    }

    static void Example1_BasicExecution()
    {
        PrintExampleHeader("Example 1: Basic JavaScript Execution");

        using var engine = new BrimstoneEngine();
        engine.Eval("console.log('Hello from Brimstone!');");
        engine.Eval("console.log('This is C# calling JavaScript');");

        Console.WriteLine("✓ Basic execution successful");
        Console.WriteLine();
    }

    static void Example2_Arithmetic()
    {
        PrintExampleHeader("Example 2: Arithmetic and Variables");

        using var engine = new BrimstoneEngine();

        engine.Eval(@"
            const a = 10;
            const b = 20;
            const sum = a + b;
            const product = a * b;
            console.log('a =', a, ', b =', b);
            console.log('sum =', sum);
            console.log('product =', product);
        ");

        Console.WriteLine("✓ Arithmetic operations successful");
        Console.WriteLine();
    }

    static void Example3_Arrays()
    {
        PrintExampleHeader("Example 3: Working with Arrays");

        using var engine = new BrimstoneEngine();

        engine.Eval(@"
            const numbers = [1, 2, 3, 4, 5];
            const doubled = numbers.map(n => n * 2);
            const sum = numbers.reduce((acc, n) => acc + n, 0);

            console.log('Original:', numbers.join(', '));
            console.log('Doubled:', doubled.join(', '));
            console.log('Sum:', sum);
        ");

        Console.WriteLine("✓ Array operations successful");
        Console.WriteLine();
    }

    static void Example4_Objects()
    {
        PrintExampleHeader("Example 4: Working with Objects");

        using var engine = new BrimstoneEngine();

        engine.Eval(@"
            const person = {
                name: 'Alice',
                age: 30,
                city: 'New York',
                greet() {
                    return `Hello, I'm ${this.name} from ${this.city}`;
                }
            };

            console.log('Person:', JSON.stringify(person));
            console.log('Greeting:', person.greet());
        ");

        Console.WriteLine("✓ Object operations successful");
        Console.WriteLine();
    }

    static void Example5_Functions()
    {
        PrintExampleHeader("Example 5: Functions and Closures");

        using var engine = new BrimstoneEngine();

        engine.Eval(@"
            function fibonacci(n) {
                if (n <= 1) return n;
                return fibonacci(n - 1) + fibonacci(n - 2);
            }

            function createCounter() {
                let count = 0;
                return {
                    increment: () => ++count,
                    decrement: () => --count,
                    getValue: () => count
                };
            }

            console.log('Fibonacci(10):', fibonacci(10));

            const counter = createCounter();
            counter.increment();
            counter.increment();
            counter.increment();
            console.log('Counter value:', counter.getValue());
        ");

        Console.WriteLine("✓ Functions and closures successful");
        Console.WriteLine();
    }

    static void Example6_JsLibrary()
    {
        PrintExampleHeader("Example 6: Using a Simple JS Library");

        using var engine = new BrimstoneEngine();

        // Define a simple math utilities library
        engine.Eval(@"
            const MathUtils = {
                square: (x) => x * x,
                cube: (x) => x * x * x,
                isPrime: (n) => {
                    if (n <= 1) return false;
                    for (let i = 2; i * i <= n; i++) {
                        if (n % i === 0) return false;
                    }
                    return true;
                },
                factorial: (n) => {
                    if (n <= 1) return 1;
                    return n * MathUtils.factorial(n - 1);
                },
                range: (start, end) => {
                    const result = [];
                    for (let i = start; i <= end; i++) {
                        result.push(i);
                    }
                    return result;
                }
            };
        ");

        // Use the library
        engine.Eval(@"
            console.log('MathUtils.square(7):', MathUtils.square(7));
            console.log('MathUtils.cube(3):', MathUtils.cube(3));
            console.log('MathUtils.isPrime(17):', MathUtils.isPrime(17));
            console.log('MathUtils.factorial(5):', MathUtils.factorial(5));
            console.log('MathUtils.range(1, 5):', MathUtils.range(1, 5));

            const primes = MathUtils.range(1, 20).filter(MathUtils.isPrime);
            console.log('Primes from 1-20:', primes);
        ");

        Console.WriteLine("✓ JS library integration successful");
        Console.WriteLine();
    }

    static void Example7_JsonProcessing()
    {
        PrintExampleHeader("Example 7: JSON Processing");

        using var engine = new BrimstoneEngine();

        engine.Eval(@"
            const data = {
                users: [
                    { id: 1, name: 'Alice', role: 'admin' },
                    { id: 2, name: 'Bob', role: 'user' },
                    { id: 3, name: 'Charlie', role: 'user' }
                ],
                timestamp: Date.now()
            };

            const json = JSON.stringify(data, null, 2);
            console.log('JSON Data:', json);

            const parsed = JSON.parse(json);
            const admins = parsed.users.filter(u => u.role === 'admin');
            console.log('Admins:', JSON.stringify(admins));
        ");

        Console.WriteLine("✓ JSON processing successful");
        Console.WriteLine();
    }

    static void Example8_ErrorHandling()
    {
        PrintExampleHeader("Example 8: Error Handling");

        using var engine = new BrimstoneEngine();

        // Valid JavaScript with try-catch
        engine.Eval(@"
            try {
                const result = JSON.parse('{valid: json}');
            } catch (error) {
                console.log('Caught JS error:', error.message);
            }
        ");

        // Test C# error handling
        try
        {
            engine.Eval("this is not valid javascript!");
        }
        catch (BrimstoneException ex)
        {
            Console.WriteLine($"✓ Caught error in C#: {ex.Message}");
            Console.WriteLine($"  Result code: {ex.ResultCode}");
        }

        Console.WriteLine("✓ Error handling successful");
        Console.WriteLine();
    }

    static void Example9_ExternalFile()
    {
        PrintExampleHeader("Example 9: Loading External JS File");

        // Create a sample JS file
        const string jsFilePath = "sample_library.js";
        File.WriteAllText(jsFilePath, @"
// Sample JavaScript Library
const DataProcessor = {
    transform: (data) => {
        return data.map(item => ({
            ...item,
            processed: true,
            timestamp: Date.now()
        }));
    },

    filter: (data, predicate) => {
        return data.filter(predicate);
    },

    aggregate: (data, key) => {
        const result = {};
        data.forEach(item => {
            const value = item[key];
            result[value] = (result[value] || 0) + 1;
        });
        return result;
    }
};

// Test the library
const testData = [
    { id: 1, category: 'A' },
    { id: 2, category: 'B' },
    { id: 3, category: 'A' }
];

const processed = DataProcessor.transform(testData);
console.log('Processed data:', JSON.stringify(processed));

const aggregated = DataProcessor.aggregate(testData, 'category');
console.log('Aggregated by category:', JSON.stringify(aggregated));
");

        try
        {
            using var engine = new BrimstoneEngine();
            engine.EvalFile(jsFilePath);

            Console.WriteLine($"✓ External file '{jsFilePath}' loaded and executed successfully");
        }
        finally
        {
            // Clean up
            if (File.Exists(jsFilePath))
            {
                File.Delete(jsFilePath);
            }
        }

        Console.WriteLine();
    }

    static void PrintExampleHeader(string title)
    {
        Console.WriteLine(title);
        Console.WriteLine("-".PadRight(title.Length, '-'));
    }
}
