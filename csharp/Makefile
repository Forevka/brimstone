# Makefile for building Brimstone C# bindings

.PHONY: all build-ffi build-csharp run clean help

all: build

help:
	@echo "Brimstone C# Bindings - Build Targets"
	@echo "======================================"
	@echo "  make build       - Build both FFI library and C# bindings"
	@echo "  make build-ffi   - Build only the Rust FFI library"
	@echo "  make build-csharp- Build only the C# bindings"
	@echo "  make run         - Build and run examples"
	@echo "  make clean       - Clean build artifacts"
	@echo "  make help        - Show this help message"

build: build-ffi build-csharp

build-ffi:
	@echo "Building Rust FFI library..."
	cd .. && cargo build -p brimstone_ffi --release
	@echo "✓ FFI library built successfully"

build-csharp: build-ffi
	@echo "Building C# bindings..."
	dotnet build -c Release
	@echo "✓ C# bindings built successfully"

run: build
	@echo "Running examples..."
	dotnet run --project Brimstone.Net.Examples -c Release

clean:
	@echo "Cleaning C# build artifacts..."
	dotnet clean
	@echo "Cleaning Rust FFI artifacts..."
	cd .. && cargo clean -p brimstone_ffi
	@echo "✓ Clean complete"
