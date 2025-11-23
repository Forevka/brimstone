# Brimstone.Net - C# Bindings for Brimstone JavaScript Engine

This directory contains the C# bindings for the Brimstone JavaScript engine, allowing you to run JavaScript code from .NET applications with full access to the Brimstone engine.

## Project Structure

- **Brimstone.Net/** - The main C# bindings library
- **Brimstone.Net.Examples/** - Example project demonstrating various use cases
- **Brimstone.Net.sln** - Visual Studio solution file

## Getting Started

### Option 1: Download Pre-built Bindings (Recommended)

**The easiest way to get started!** Pre-built bindings are automatically built for every commit to master.

1. Go to [GitHub Actions](../../actions/workflows/build-csharp-bindings.yml)
2. Click the latest successful build
3. Download the artifact for your platform:
   - Linux x64: `brimstone-csharp-linux-x64.tar.gz`
   - Windows x64: `brimstone-csharp-windows-x64.zip`
4. Extract and run!

**Linux:**
```bash
tar -xzf brimstone-csharp-linux-x64.tar.gz
cd brimstone-csharp-linux-x64
./run-examples.sh
```

**Windows:**
```cmd
# Extract the zip, then:
run-examples.bat
```

Each artifact includes everything you need:
- ✅ Native library
- ✅ C# bindings
- ✅ Ready-to-run examples
- ✅ Complete documentation
- ✅ Sample code

### Option 2: Build from Source

If you want to build from source or modify the bindings:

#### Prerequisites

- .NET 8.0 SDK or later
- Rust toolchain (for building the native library)
- Linux, macOS, or Windows

#### Building

#### 1. Build the Brimstone FFI library

From the repository root:

```bash
cargo build -p brimstone_ffi --release
```

This will create the shared library at:
- Linux: `target/release/libbrimstone_ffi.so`
- macOS: `target/release/libbrimstone_ffi.dylib`
- Windows: `target/release/brimstone_ffi.dll`

### 2. Build the C# bindings

From the `csharp` directory:

```bash
dotnet build
```

## Running Examples

From the `csharp` directory:

```bash
dotnet run --project Brimstone.Net.Examples
```

## Usage

### Basic Example

```csharp
using Brimstone.Net;

// Create a new JavaScript engine instance
using var engine = new BrimstoneEngine();

// Execute JavaScript code
engine.Eval("console.log('Hello from JavaScript!');");

// Execute with variables
engine.Eval(@"
    const a = 10;
    const b = 20;
    console.log('Sum:', a + b);
");
```

### Working with Arrays and Objects

```csharp
using var engine = new BrimstoneEngine();

engine.Eval(@"
    const numbers = [1, 2, 3, 4, 5];
    const doubled = numbers.map(n => n * 2);
    console.log('Doubled:', doubled);

    const person = {
        name: 'Alice',
        age: 30,
        greet() {
            return `Hello, I'm ${this.name}`;
        }
    };
    console.log(person.greet());
");
```

### Loading External JavaScript Files

```csharp
using var engine = new BrimstoneEngine();

// Load and execute a JavaScript file
engine.EvalFile("myLibrary.js");

// Use the loaded library
engine.Eval("MyLibrary.doSomething();");
```

### Using JavaScript Libraries

```csharp
using var engine = new BrimstoneEngine();

// Define a JavaScript library
engine.Eval(@"
    const MathUtils = {
        square: (x) => x * x,
        isPrime: (n) => {
            if (n <= 1) return false;
            for (let i = 2; i * i <= n; i++) {
                if (n % i === 0) return false;
            }
            return true;
        }
    };
");

// Use the library
engine.Eval(@"
    console.log('Square of 7:', MathUtils.square(7));
    console.log('Is 17 prime?', MathUtils.isPrime(17));
");
```

### Error Handling

```csharp
using var engine = new BrimstoneEngine();

try
{
    engine.Eval("this is invalid javascript");
}
catch (BrimstoneException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Result code: {ex.ResultCode}");
}
```

## API Reference

### BrimstoneEngine Class

The main class for interacting with the Brimstone JavaScript engine.

#### Methods

- `void Eval(string code, string? filename = null)` - Evaluates JavaScript code
- `string EvalWithResult(string code, string? filename = null)` - Evaluates JavaScript code and returns the result
- `void EvalFile(string filePath)` - Evaluates JavaScript code from a file
- `string EvalFileWithResult(string filePath)` - Evaluates JavaScript code from a file and returns the result
- `void Dispose()` - Disposes the engine instance

#### Static Members

- `string Version` - Gets the Brimstone version string
- `void Initialize()` - Initializes the Brimstone runtime (called automatically)

### BrimstoneException Class

Exception thrown when a Brimstone operation fails.

#### Properties

- `BsResultCode ResultCode` - The result code indicating the type of error

### BsResultCode Enum

Result codes for Brimstone operations:

- `Success` - Operation completed successfully
- `Error` - General error occurred
- `NullPointer` - Null pointer error
- `InvalidUtf8` - Invalid UTF-8 encoding
- `PanicCaught` - A Rust panic was caught

## Features

✅ Full JavaScript ES2024+ support (via Brimstone)
✅ Safe memory management with IDisposable pattern
✅ Comprehensive error handling
✅ Cross-platform support (Linux, macOS, Windows)
✅ Load and execute external JavaScript files
✅ Use JavaScript libraries from C#
✅ Easy-to-use API

## Production Readiness

These bindings provide full access to the Brimstone JavaScript engine and are designed for production use. The FFI layer is safe, with proper error handling and memory management.

### Thread Safety

Each `BrimstoneEngine` instance should be used from a single thread. For multi-threaded applications, create separate engine instances per thread.

### Performance

The bindings use P/Invoke for minimal overhead when crossing the native boundary. The Brimstone engine itself is optimized for performance with:

- Bytecode VM
- Compacting garbage collector
- Custom RegExp engine
- Optimized parser

## Examples

The `Brimstone.Net.Examples` project includes comprehensive examples:

1. **Basic JavaScript Execution** - Simple console output
2. **Arithmetic and Variables** - Working with numbers and variables
3. **Arrays** - Array operations like map, filter, reduce
4. **Objects** - Creating and using JavaScript objects
5. **Functions and Closures** - Function definitions and closures
6. **JS Libraries** - Creating and using JavaScript libraries
7. **JSON Processing** - Working with JSON data
8. **Error Handling** - Handling errors in both JS and C#
9. **External Files** - Loading and executing external JS files

## Contributing

Contributions are welcome! Please see the main Brimstone repository for contribution guidelines.

## License

This project follows the same license as the main Brimstone repository (MIT).
