// Simple JavaScript test script for Brimstone
console.log("Hello from Brimstone!");
console.log("=".repeat(40));

// Basic arithmetic
const a = 10;
const b = 20;
console.log(`Addition: ${a} + ${b} = ${a + b}`);
console.log(`Multiplication: ${a} * ${b} = ${a * b}`);

// Array operations
const numbers = [1, 2, 3, 4, 5];
const doubled = numbers.map(n => n * 2);
console.log(`Original array: [${numbers.join(", ")}]`);
console.log(`Doubled array: [${doubled.join(", ")}]`);

// Object creation
const person = {
  name: "Alice",
  age: 30,
  greet() {
    return `Hello, I'm ${this.name} and I'm ${this.age} years old.`;
  }
};
console.log(person.greet());

// Function demonstration
function fibonacci(n) {
  if (n <= 1) return n;
  return fibonacci(n - 1) + fibonacci(n - 2);
}
console.log(`Fibonacci(10) = ${fibonacci(10)}`);

// Modern JavaScript features
const square = x => x * x;
console.log(`Square of 7 = ${square(7)}`);

console.log("=".repeat(40));
console.log("All tests completed successfully!");
