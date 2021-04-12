# C library minimal

Minimal example of a C shared library that can be called from C#.

# How to build

1. Generate Make files using CMake: `cmake -S . -B cmake-build-release -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release`
2. Build using Make: `make -C ./cmake-build-release`
3. Inspect the symbols of the dynamic link library build artifact to ensure that function(s) are there.
    - macOS: `nm -g ./lib/*.dylib`
4. Copy the dynamic link library to sit next to your C# build artifacts.

# How to generate bindings

```bash
-i
"/PATH/TO/sokol/sokol_gfx.h"
-o
"./sokol_gfx.cs"
-u
-l
"sokol_gfx"
```