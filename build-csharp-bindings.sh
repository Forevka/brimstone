#!/bin/bash

# Build script for Brimstone C# bindings
set -e

echo "========================================="
echo "Building Brimstone C# Bindings"
echo "========================================="
echo

# Step 1: Build the Rust FFI library
echo "Step 1: Building Rust FFI library..."
cargo build -p brimstone_ffi --release
echo "✓ Rust FFI library built successfully"
echo

# Step 2: Build the C# bindings
echo "Step 2: Building C# bindings..."
cd csharp
dotnet build -c Release
echo "✓ C# bindings built successfully"
echo

# Step 3: Run examples
echo "Step 3: Running examples..."
dotnet run --project Brimstone.Net.Examples -c Release
echo

echo "========================================="
echo "Build completed successfully!"
echo "========================================="
echo
echo "FFI Library location: target/release/libbrimstone_ffi.so"
echo "C# Bindings: csharp/Brimstone.Net/bin/Release/net8.0/"
echo
echo "To run examples:"
echo "  cd csharp && dotnet run --project Brimstone.Net.Examples"
