# GitHub Actions Workflows

## build-csharp-bindings.yml

This workflow automatically builds the Brimstone C# bindings for multiple platforms on every push to master.

### Triggers

- Push to `master` or `main` branch
- Pull requests to `master` or `main` branch
- Manual workflow dispatch

### Platforms

- **Linux x64** - Built on Ubuntu latest
- **Windows x64** - Built on Windows latest

### Build Process

For each platform:

1. **Checkout repository** - Gets the latest code
2. **Setup Rust** - Installs stable Rust toolchain
3. **Setup .NET** - Installs .NET 8.0 SDK
4. **Cache dependencies** - Caches Cargo registry, index, and build artifacts
5. **Build FFI library** - Compiles `brimstone_ffi` in release mode
6. **Build C# bindings** - Compiles the C# library and examples
7. **Run examples** - Tests that the bindings work correctly
8. **Package artifacts** - Creates ready-to-use distribution packages
9. **Upload artifacts** - Makes artifacts available for download

### Artifacts

Each build produces downloadable artifacts:

#### Linux x64 (`brimstone-csharp-linux-x64.tar.gz`)
Contains:
- `libbrimstone_ffi.so` - Native library
- `Brimstone.Net.dll` - C# bindings
- `Examples/` - Compiled examples with source
- `run-examples.sh` - Quick run script
- Documentation (README.md, CSHARP_BINDINGS.md, GETTING_STARTED.md)

#### Windows x64 (`brimstone-csharp-windows-x64.zip`)
Contains:
- `brimstone_ffi.dll` - Native library
- `Brimstone.Net.dll` - C# bindings
- `Examples/` - Compiled examples with source
- `run-examples.bat` - Quick run script
- Documentation (README.md, CSHARP_BINDINGS.md, GETTING_STARTED.md)

#### Release Info (`RELEASE_INFO.md`)
Contains:
- Artifact descriptions
- Quick start guides
- Usage instructions
- Build information

### Artifact Retention

Artifacts are kept for 90 days after each build.

### Downloading Artifacts

1. Go to the [Actions](../../actions) tab
2. Click on the latest successful workflow run
3. Scroll to the "Artifacts" section
4. Download the artifact for your platform
5. Extract and run!

### Using Pre-built Bindings

#### Linux
```bash
# Download and extract
tar -xzf brimstone-csharp-linux-x64.tar.gz
cd brimstone-csharp-linux-x64

# Run examples
./run-examples.sh

# Or use in your project
cp libbrimstone_ffi.so /path/to/your/project
cp Brimstone.Net.dll /path/to/your/project
```

#### Windows
```cmd
# Download and extract brimstone-csharp-windows-x64.zip

# Run examples
run-examples.bat

# Or copy to your project
copy brimstone_ffi.dll C:\path\to\your\project
copy Brimstone.Net.dll C:\path\to\your\project
```

### Local Testing

To test the workflow locally, you can use [act](https://github.com/nektos/act):

```bash
# Install act
# Then run:
act push
```

### Troubleshooting

If the build fails:

1. **Check Rust version** - Ensure stable Rust is being used
2. **Check .NET version** - Ensure .NET 8.0+ is installed
3. **Check dependencies** - Verify all Cargo dependencies resolve
4. **Check C# build** - Verify .csproj files are valid
5. **Check logs** - Review the workflow logs for specific errors

### Customization

To modify the workflow:

1. Edit `.github/workflows/build-csharp-bindings.yml`
2. Test changes with `act` or by pushing to a branch
3. Verify artifacts are created correctly
4. Merge to master

### Adding More Platforms

To add macOS support:

```yaml
build-macos:
  name: Build macOS x64
  runs-on: macos-latest
  steps:
    # Similar to Linux build
    # Use .dylib extension for native library
```

To add ARM64 support:

```yaml
build-linux-arm64:
  name: Build Linux ARM64
  runs-on: ubuntu-latest
  steps:
    - name: Setup Rust
      uses: actions-rs/toolchain@v1
      with:
        toolchain: stable
        target: aarch64-unknown-linux-gnu
    # Cross-compile steps...
```

### Performance

Build times (approximate):

- **Linux**: 10-15 minutes
- **Windows**: 12-18 minutes
- **Total**: ~25-30 minutes for both platforms

Caching significantly reduces subsequent build times:

- **Cached Linux**: 3-5 minutes
- **Cached Windows**: 4-6 minutes

### Security

- All dependencies are cached securely
- Artifacts are scanned automatically
- No secrets are required for this workflow
- All builds run in isolated environments

### Questions?

See the main [CSHARP_BINDINGS.md](../../CSHARP_BINDINGS.md) for detailed documentation.
